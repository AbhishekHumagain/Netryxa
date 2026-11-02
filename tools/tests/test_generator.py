import io
import sys
import tempfile
import unittest
from pathlib import Path
from zipfile import ZipFile
sys.path.insert(0, str(Path(__file__).resolve().parents[1]))
from generate_project import generate, validate_name, PREFIX

class GeneratorTests(unittest.TestCase):
    def test_rejects_injection_and_invalid_identifiers(self):
        for name in ['../Bad', 'Bad;id', 'Bad\r\nHeader', 'class', 'Static', 'CON', 'COM1', 'A', 'Acme.App', '2Bad', 'A'*51]:
            with self.subTest(name=name), self.assertRaises(ValueError):
                validate_name(name)

    def test_generated_archive_references_and_config(self):
        with ZipFile(io.BytesIO(generate('Acme'))) as archive:
            names = archive.namelist()
            self.assertIn('Acme/Acme.sln', names)
            self.assertIn('Acme/Acme.API/Acme.API.csproj', names)
            self.assertIn('Acme/keycloak/realm-acme.json', names)
            self.assertFalse(any('/obj/' in p or '/bin/' in p or p.endswith('/.env') or '.user' in p for p in names))
            sln = archive.read('Acme/Acme.sln').decode()
            if PREFIX != 'Acme': self.assertNotIn(PREFIX + '.', sln)
            self.assertIn('Acme.API', sln)
            self.assertIn('namespace Acme.', archive.read('Acme/Acme.Application/ConfigureServices.cs').decode())
            self.assertIn('static class', archive.read('Acme/Acme.Application/ConfigureServices.cs').decode())
            self.assertIn('realm-acme.json', archive.read('Acme/docker-compose.yml').decode())
            self.assertIn('Acme.API.dll', archive.read('Acme/Dockerfile').decode())

    def test_unknown_root_files_and_symlinks_are_excluded(self):
        with tempfile.TemporaryDirectory() as temp:
            root = Path(temp)
            (root/'.env').write_text('secret')
            (root/'private.json').write_text('secret')
            (root/'README.md').symlink_to(root/'.env')
            with ZipFile(io.BytesIO(generate('Acme', root))) as archive:
                self.assertEqual([], archive.namelist())

if __name__ == '__main__':
    unittest.main()
