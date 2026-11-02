# Contributing

Open an issue for significant architecture changes. Keep changes focused and include regression coverage for behavioral changes. Never submit credentials, local .env files, or user data.

Run `dotnet build Static.sln -c Release`, `dotnet run --project tests/Static.RegressionTests -c Release`, and `python3 -m unittest discover -s tools/tests -v`. Verify renamed projects as described in README. Pull requests run these checks in GitHub Actions.

By contributing, you agree that your contributions use the repository MIT license.
