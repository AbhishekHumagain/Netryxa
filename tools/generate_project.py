"""Create a renamed ZIP from a fixed, reviewed source tree. Python 3.10+."""
from __future__ import annotations
import argparse
import io
import re
from pathlib import Path
from zipfile import ZipFile, ZIP_DEFLATED

ROOT = Path(__file__).resolve().parents[1]
# Explicit allowlist: unreviewed local files can never silently enter a public download.
PROJECTS = {p.name for p in ROOT.glob("Static.*") if p.is_dir()}
ROOT_FILES = {"Static.sln", "Directory.Packages.props", "global.json", "nuget.config",
              "Dockerfile", "docker-compose.yml", ".dockerignore", ".editorconfig",
              ".gitignore", ".env.example", "README.md", "LICENSE", "CONTRIBUTING.md",
              "SECURITY.md", "common.proto", ".gitlab-ci.yml"}
EXTENSIONS = {".cs", ".csproj", ".resx", ".json", ".proto", ".html", ".css", ".js",
              ".md", ".py", ".yml", ".yaml", ".props", ".targets", ".svg", ".png", ".ico"}
SKIP = {"bin", "obj", ".git", ".idea", ".vs", "node_modules", "__pycache__", "Logs", "logs"}
# Restrict to a single C# identifier. This also prevents path and header injection.
RESERVED = {"static", "class", "namespace", "public", "private", "internal", "protected",
            "void", "int", "string", "object", "new", "base", "this", "event", "default",
            "using", "return", "var", "dynamic", "record", "required", "file", "global",
            "system", "microsoft", "con", "prn", "aux", "nul"}

def validate_name(name: str) -> str:
    if (not re.fullmatch(r"[A-Z][A-Za-z0-9]{1,49}", name) or name.lower() in RESERVED
            or re.fullmatch(r"(?:COM|LPT)[0-9]", name, re.I)):
        raise ValueError("Use 2–50 letters/digits, starting with an uppercase letter; reserved names are disallowed.")
    return name

def source_files(root: Path):
    for path in sorted(root.rglob("*")):
        rel = path.relative_to(root)
        if path.is_symlink() or not path.is_file() or any(p in SKIP for p in rel.parts):
            continue
        if path.name.startswith(".env") and path.name != ".env.example":
            continue
        if len(rel.parts) == 1:
            allowed = path.name in ROOT_FILES
        else:
            allowed = (rel.parts[0] in PROJECTS | {"keycloak", "docs", ".github", "tools", "portal", "tests", ".template.config"}
                       and (path.suffix in EXTENSIONS or path.name == "Dockerfile"))
        if allowed:
            yield path, rel

def rename(value: str, name: str) -> str:
    # Word boundaries preserve framework members such as IsStatic and StaticFiles.
    value = value.replace("Static", name).replace("Use" + name + "Files", "UseStaticFiles")
    return value

def transform(value: str, name: str) -> str:
    value = rename(value, name)
    # Lower-case service/realm IDs only in configuration, never the C# static keyword.
    return value

def generate(name: str, root: Path = ROOT) -> bytes:
    validate_name(name)
    output = io.BytesIO()
    with ZipFile(output, "w", ZIP_DEFLATED) as archive:
        for path, rel in source_files(root):
            dest = str(rel).replace("Static", name).replace("static", name.lower())
            data = path.read_bytes()
            if path.suffix not in {".png", ".ico"}:
                try:
                    content = transform(data.decode("utf-8-sig"), name)
                    if path.suffix in {".json", ".yml", ".yaml"} or path.name == ".env.example":
                        content = re.sub(r"\bstatic\b", name.lower(), content)
                    data = content.encode("utf-8")
                except UnicodeDecodeError:
                    raise ValueError(f"Unexpected non-UTF8 template file: {rel}")
            archive.writestr(f"{name}/{dest}", data)
    return output.getvalue()

if __name__ == "__main__":
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("name")
    parser.add_argument("--output", type=Path)
    args = parser.parse_args()
    try:
        result = generate(args.name)
    except ValueError as error:
        parser.error(str(error))
    output = args.output or Path(f"{args.name}.zip")
    output.parent.mkdir(parents=True, exist_ok=True)
    output.write_bytes(result)
    print(output.resolve())
