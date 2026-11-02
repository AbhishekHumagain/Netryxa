# Portal deployment

Target repository: https://github.com/AbhishekHumagain/Netryxa

The existing portfolio source is not in this checkout. `portal/index.html` is an integration-ready page. Copy it into the portal at `/boilerplate/` and retain the GET form action `/api/boilerplate/download`.

Run `python3 tools/download_server.py --port 8090` from a read-only reviewed release checkout. It listens on loopback, uses only the Python standard library, validates names on the server, and creates ZIPs in memory without shell execution, uploads, or fetching user-supplied URLs. Each worker processes requests sequentially to bound concurrent archive memory.

Proxy `/api/boilerplate/download` on **the same origin** as the portal to this service. Configure HTTPS, a request rate limit, connection limit, upstream timeout, and a supervisor/container restart policy at the proxy. Example Nginx locations (define the zone once inside `http`):

```nginx
limit_req_zone $binary_remote_addr zone=generator:10m rate=6r/m;
# Inside the existing HTTPS server block:
location = /api/boilerplate/download {
    limit_req zone=generator burst=3 nodelay;
    proxy_pass http://127.0.0.1:8090;
    proxy_read_timeout 30s;
}
```

The service is separate from the application API so a database/Keycloak outage does not stop source downloads. Serve a pinned, reviewed release checkout and change it only after CI passes. No GitHub access token is needed at download time. To serve from another domain, implement an explicit origin policy; the provided form assumes same-origin deployment.

Local check: run the service, then open `http://localhost:8090/api/boilerplate/download?name=MyProject`. For portal UI preview, serve `portal/` with a static server; downloads require the same-origin reverse proxy above.

GitHub publication: create/use the named repository, push the reviewed source, enable Actions and private vulnerability reporting, and set the repository as a Template repository. The CI workflow publishes a sanitized source archive artifact; tag publishing requires the release job to pass the full validation workflow. The portal does not need the GitHub template toggle because it uses the same source generator directly.
