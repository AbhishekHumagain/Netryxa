# Security

Report vulnerabilities privately through the repository Security tab (enable private vulnerability reporting in GitHub settings). Do not open a public issue containing exploit details or secrets. If private reporting is unavailable, contact the maintainer via the portfolio contact page.

Do not deploy development credentials or `start-dev` Keycloak publicly. Use HTTPS, per-service audiences, managed secrets, and explicit redirect URIs. Never expose the optional local Python code execution service to untrusted users without an isolated execution environment. Review dependency licenses, including AutoMapper and MediatR licensing, for your use case; MIT licensing of this repository does not relicense dependencies.

Only versions with passing CI and a reviewed dependency audit should be tagged as releases.
