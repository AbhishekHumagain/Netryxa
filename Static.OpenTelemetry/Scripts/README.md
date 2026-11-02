# Run C# Script Files (.csx) with dotnet-script

This guide explains how to install and use the `dotnet-script` tool to execute C# script files (`.csx`) effortlessly.

## Prerequisites

Make sure you have `dotnet` CLI accessible.

## Installation

To enable the execution of `.csx` files, install the `dotnet-script` tool globally using the command below:

```bash
dotnet tool install -g dotnet-script
```

## Running C# Scripts
Once dotnet-script is installed, you can execute any `.csx` file by running the following command:

```bash
dotnet-script <path-to-your-script.csx>
```

### Example
Suppose you have a C# script file named OTelBackendLauncher.csx located in the Static.OpenTelemetry/Scripts directory. You can run it as follows:

```bash
dotnet-script Static.OpenTelemetry/Scripts/OTelBackendLauncher.csx
```
Ensure the script is correctly configured and that all necessary dependencies are either included in the script or accessible to the runtime.