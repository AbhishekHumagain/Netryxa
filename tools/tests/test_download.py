import sys
import threading
import unittest
import urllib.request
import urllib.error
from http.server import HTTPServer
from pathlib import Path
sys.path.insert(0, str(Path(__file__).resolve().parents[1]))
from download_server import Handler

class DownloadTests(unittest.TestCase):
    def test_http_download_and_invalid_names(self):
        server = HTTPServer(('127.0.0.1', 0), Handler)
        thread = threading.Thread(target=server.serve_forever, daemon=True)
        thread.start()
        try:
            url = f'http://127.0.0.1:{server.server_port}/api/boilerplate/download'
            with urllib.request.urlopen(url + '?name=Acme') as response:
                self.assertEqual('application/zip', response.headers['Content-Type'])
                self.assertIn('Acme.zip', response.headers['Content-Disposition'])
                self.assertTrue(response.read().startswith(b'PK'))
            for query in ['?name=../Bad', '?name=Acme&name=Other', '?name=Acme&source=evil']:
                with self.assertRaises(urllib.error.HTTPError) as error:
                    urllib.request.urlopen(url + query)
                self.assertEqual(400, error.exception.code)
        finally:
            server.shutdown()
            server.server_close()
            thread.join()
