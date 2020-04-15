# Introduction 
proknow-sdk is a .NET Core SDK to interface with ProKnow.

# Documentation
Complete documentation is available at [Github Pages](http://proknow.github.io/proknow-sdk-dotnet).

# Development

## Running Tests
The tests require access to ProKnow in order to create temporary custom metrics, scorecard templates, and workspaces.

Create a .runsettings file in the solution folder with the following content, edited appropriately:
```
<?xml version="1.0" encoding="utf-8"?>
<RunSettings>
  <!-- Parameters used by tests at run time -->
  <TestRunParameters>
    <Parameter name="baseUrl" value="https://example.proknow.com" />
    <Parameter name="credentialsFile" value="C:/Users/user1/AppData/Local/ProKnow/credentials.json" />
  </TestRunParameters>
</RunSettings>
```
Refer to these [instructions](https://docs.microsoft.com/en-us/visualstudio/test/configure-unit-tests-by-using-a-dot-runsettings-file) regarding how to use the .runsettings file when running tests in Visual Studio or from the command prompt.
