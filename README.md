# Introduction 
proknow-sdk is a .NET Core SDK to interface with ProKnow.

# Installation
Install the SDK using [NuGet](https://nuget.org/packages/ProKnow).

# Basic Usage
To use the SDK, you'll need two important items:

- Base URL:  This is the URL that will be used to make API requests to ProKnow.  It's the first part of the web address you see when you are signed into your ProKnow account.  The form of this URL is typically `https://your-domain.proknow.com`.
- API Token: You can generate an [API token](https://support.proknow.com/article/165-configuring-your-profile#api-keys) in the ProKnow user interface. Always keep your API token secret.  Once you have your `credentials.json`, make note of the file path.

The code snippet below can be used to test your installation of the `proknow-sdk` package.  Please be sure to replace `https://example.proknow.com` with your organization's base URL described above, and replace `C:/Users/user1/AppData/Local/ProKnow/credentials.json` with the path to your credentials file:

```
using ProKnow;
using System.Threading.Tasks;

var pk = new ProKnowApi('https://example.proknow.com', 'C:/Users/user1/AppData/Local/ProKnow/credentials.json');
var workspaces = await pk.Workspaces.QueryAsync();
Console.WriteLine("The workspaces are as follows:");
foreach (var workspace in workspaces)
{
    Console.WriteLine(workspace.Name);
}
```

Note that the majority of the methods in the SDK are asynchronous.  For more information about asynchronous programming, refer to the Microsoft [Asynchronous programming with async and await](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/async/) article.


If an adaptation of the above code snippet does not work for you, see the troubleshooting section below.

# API
* ProKnowApi - This is the root object for interfacing with ProKnow and includes the following properties for interacting with ProKnow:
    * Requestor - This is the object that issues HTTP requests
    * CustomMetrics - This is the object that interacts with custom metrics
    * ScorecardTemplates - This is the object that interacts with scorecard templates
    * Workspaces - This is the object that interacts with workspaces
    * Uploads - This is the object that interacts with uploads
    * Patients - This is the object that interacts with patients
    * Collections - This is the object that interacts with collections

Please refer to the built-in XML documentation in the SDK for more detailed documentation.

# Feature List (TODO)

# Troubleshooting

## Connection errors
These can be caused by the failure to replace `https://example.proknow.com` as your `baseUrl` in the call to the ProKnowApi constructor. `https://example.proknow.com` is not a real endpoint and will not work. If you’ve already replaced the base URL, log into your ProKnow account and check to make sure it matches the base URL shown in the address bar.

If you are able to log into your ProKnow account and have verified that the base URLs match, there is also a chance that your organization’s firewall is configured to block traffic originating from unknown sources such as your .NET application. If this is the case, please contact your IT department to request special permission for your application to access the ProKnow API from inside your organization’s network.

## File not found errors

If you specified a `credentialsFile` in the call to the ProKnowApi constructor, but are receiving a message saying that the file or directory does not exist, you may have a typo in your credentials file path. Correct the file path, and try again. Note that relative file paths are usually relative to the directory from which the application is being invoked.

## Copying examples without modification

The examples shown throughout the documentation are meant to be representative and are not likely to work without modification. Let’s take a look at the example for the method ProKnowApi.Patients.PatientItem.SetMetadataAsync():

```
using ProKnow;
using System.Threading.Tasks;

var pk = new ProKnowApi('https://example.proknow.com', 'C:/Users/user1/AppData/Local/ProKnow/credentials.json');
var patients = await pk.Patients.Lookup("Clinical", new string[] { "HNC-0522c0009" });
var patient = await patients[0].GetAsync();
var metadata = await patient.GetMetadataAsync();
metadata["Genetic Type"] = "Type II";
await patient.SetMetadataAsync(metadata);
await patient.SaveAsync();
```

This code will not work in your setup for several reasons:

* Your `baseUrl` for your ProKnow account is not `https://example.proknow.com`.
* Your `credentials.json` file may not be located at the path "C:/Users/user1/AppData/Local/ProKnow/credentials.json."
* You probably do not have a patient with the MRN "HNC-0522c0009" in a workspace called "Clinical."
* You probably do not have a custom metric called "Genetic Type."

If you run into problems while running your script, examine the error message and make sure you didn’t copy a code example without making the proper modifications. Values may need to be replaced or additional setup code may need to be added in order for your code to function properly.

# Guides (TODO)

# Running Tests
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
