"""Local generator API. In production use a TLS reverse proxy with rate/concurrency limits."""
import argparse
from http.server import BaseHTTPRequestHandler, HTTPServer
from urllib.parse import parse_qs, urlsplit
from generate_project import generate

class Handler(BaseHTTPRequestHandler):
    def do_GET(self):
        url = urlsplit(self.path)
        if url.path != "/api/boilerplate/download":
            self.send_error(404)
            return
        params = parse_qs(url.query, keep_blank_values=True)
        names = params.get("name", [])
        if set(params) != {"name"} or len(names) != 1:
            self.send_error(400, "Supply exactly one project name")
            return
        try:
            data = generate(names[0])
        except ValueError as error:
            self.send_error(400, str(error))
            return
        self.send_response(200)
        self.send_header("Content-Type", "application/zip")
        self.send_header("Content-Disposition", f'attachment; filename="{names[0]}.zip"')
        self.send_header("Content-Length", str(len(data)))
        self.send_header("X-Content-Type-Options", "nosniff")
        self.send_header("Cache-Control", "no-store")
        self.end_headers()
        self.wfile.write(data)

if __name__ == "__main__":
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--port", type=int, default=8090)
    args = parser.parse_args()
    HTTPServer(("127.0.0.1", args.port), Handler).serve_forever()
