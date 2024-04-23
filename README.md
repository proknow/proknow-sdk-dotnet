# Introduction 
proknow-sdk is a .NET SDK to interface with ProKnow.

# Documentation
Complete documentation is available at [Github Pages](http://proknow.github.io/proknow-sdk-dotnet).

# Development

## Generating Documentation
Documentation is generated using the tool [DocFx](https://dotnet.github.io/docfx) and served on [Github Pages](http://proknow.github.io/proknow-sdk-dotnet).

### Prerequisites
- Windows PowerShell v2+ - This is automatically included in Windows 10 starting with update 1607.  If you are running
an earlier version of Windows, PowerShell can be installed following these
[instructions](https://docs.microsoft.com/en-us/powershell/scripting/install/installing-powershell-core-on-windows).
- .NET 8+ - The installer for this can be downloaded [here](https://dotnet.microsoft.com/download).
- Chocolatey - This is a package manager for Windows.  The instructions for installing this can be found [here](https://chocolatey.org/install).
- DocFx - This can be installed using Chocolatey from an administrative command prompt with the command `choco install docfx`.

### Previewing Documentation
In a command prompt, from the project root directory, run `docfx "./docs/docfx.json" --serve`.  This will serve a
documentation website on *http://localhost:8080* to verify proper content, links, formatting, etc.  If there are errors
generating the documentation, they will be highlighted in yellow.

### Verifying Documentation
Once branch changes have been merged into the master branch, the build pipeline will regenerate the documentation which
can then be viewed on [Github Pages](http://proknow.github.io/proknow-sdk-dotnet).

## Running Tests

### Access to ProKnow
The tests require access to ProKnow in order to create temporary custom metrics, scorecard templates, and workspaces. To configure your environment to run tests locally

1. Copy `./proknow-sdk-test/TestEnvironment/templates/config.json` into `./proknow-sdk-test/TestEnvironment/etc` and replace the following values with what is in your Atlas development `config.json`:
    - `S3_ACCESS_KEY_ID`
    - `S3_ACCESS_KEY_SECRET`
    - `AUTH0_CLIENT_ID`
    - `AUTH0_CLIENT_SECRET`
2. Open a PowerShell command prompt in the solution folder run the following:

    ```ps1
    $ cd ./proknow-sdk-test/TestEnvironment
    $ docker-compose up -d
    $ ./test-env-setup.ps1
    ```

    If you encounter errors running the last command, you may need to adjust the PowerShell execution policy to allow script execution. To do this, run PowerShell as an administrator and execute `$ Set-ExecutionPolicy RemoteSigned`.

    A `credentials.json` file will be created in the local directory. Update the `.runsettings` file with the full path to the `credentials.json` file.

3. If a `.runsettings` file does not yet exist, create one in the solution folder with the following content:
    ```
    <?xml version="1.0" encoding="utf-8"?>
    <RunSettings>
      <!-- Parameters used by tests at run time -->
      <TestRunParameters>
        <Parameter name="baseUrl" value="https://localhost:3005" />
        <Parameter name="credentialsFile" value="C:/src/pk/proknow-sdk-dotnet/proknow-sdk-test/TestEnvironment/credentials.json" />
      </TestRunParameters>
    </RunSettings>
    ```
    Refer to these [instructions](https://docs.microsoft.com/en-us/visualstudio/test/configure-unit-tests-by-using-a-dot-runsettings-file) regarding how to use the .runsettings file when running tests in Visual Studio or from the command prompt.

### Test Data
The project test data is managed using [Git Large File Storage](https://github.com/git-lfs/git-lfs).  This tool must be
installed on a developers workstation in order for the data files to contain the actual contents rather than pointers.
Follow the instructions in the provided link for your platform.

Once git-lfs has been installed on your workstation, initialize the use of it by running:
```
git lfs install
```

If the repository had already been cloned before git-lfs was installed, replace the pointers in the data files with the
actual content by running this command from the project directory:
```
git lfs checkout
```

To run the tests, right click on the "proknow-sdk-test" folder and click on `Run Tests` option.

# Deployment

## Creating a New Release

The pipeline will automatically create and publish a new NuGet package when code is merged into the master branch. 
The following files need to be updated before each release:

1. `releaseHistory.md`
2. `proknow-sdk.csproj` - Update the `<Version>` attribute with the new version following the [semantic versioning](https://semver.org/) rules.
