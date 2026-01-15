# PSToExe

This is an example project of how to create a single EXE file for distribution of a PowerShell script without needing to install PowerShell 7.5.4 on target machines.

This project is intended for demo purposes only and has not been written, tested, or designed for production environments.

This was implemented quick and dirty.
Don't expect a polished, white-glove experience.

## No Support

There will be no free support for this project.
GitHub issue will be ignored and/or immediately closed.
Do not ping the author(s) or maintainer(s) here or elsewhere for help, questions, assistance, etc.
You use this project at your own risk and without assistance.

## Contributions

Pull requests are welcome, but there is no active support for this project.
Your PRs may go unanswered for an indeterminate amount of time.
PRs with excessive pings for review will be closed without review.

## How It Works

- [Script.ps1](Script.ps1) is embedded in the program assembly as a manifest resource at build time.
- The csproj file has the required settings to publish the app as a single, self-contained exe file.
- The program itself uses Hosted PowerShell (Microsoft.PowerShell.SDK) to run the embedded script.

## How to use

1. Edit the `Script.ps1` with your desired PowerShell script
2. Build and publish

## Build and Publish

```bash
dotnet publish -o .\publish\
```

You will then have the `.\publish\PSToExe.exe` file ready for distribution on 64-bit Windows.

## Running the App

Run the resulting EXE

```bash
.\PSToExe.exe
```

## Distributing the App

You only need to distribute the single EXE file.
No additional files or prerequisite installs are required.

## Caveats

- Everything is done non-interactive so you cannot use commands like `Get-Credential` or `Read-Host`
- The entire user script is wrapped in a script block, invoked, and piped to Out-String.

```powershell
{
    # Script.ps1 contents
}.Invoke() | Out-String
```

- Output order is not guaranteed to be the same as seen in a PowerShell console session.
- This has been barely tested beyond some basic PowerShell commands. Expect a bunch of things to not work as expected.
- 64-bit Windows only
