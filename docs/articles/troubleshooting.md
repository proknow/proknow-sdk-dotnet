# Troubleshooting

## Connection errors
These can be caused by the failure to replace `https://example.proknow.com` as your `baseUrl` in the call to the
ProKnowApi constructor. `https://example.proknow.com` is not a real endpoint and will not work. If you've already
replaced the base URL, log into your ProKnow account and check to make sure it matches the base URL shown in the
address bar.

If you are able to log into your ProKnow account and have verified that the base URLs match, there is also a chance
that your organization's firewall is configured to block traffic originating from unknown sources such as your .NET
application. If this is the case, please contact your IT department to request special permission for your application
to access the ProKnow API from inside your organization's network.

## File not found errors

If you specified a `credentialsFile` in the call to the ProKnowApi constructor, but are receiving a message saying
that the file or directory does not exist, you may have a typo in your credentials file path. Correct the file path,
and try again. Note that relative file paths are usually relative to the directory from which the application is being
invoked.

## Copying examples without modification

The examples shown throughout the documentation are meant to be representative and are not likely to work without
modification. Let's take a look at the example for the method ProKnowApi.Patients.PatientItem.SetMetadataAsync():

```
using ProKnow;
using System.Threading.Tasks;

var pk = new ProKnowApi("https://example.proknow.com", "./credentials.json");
var patients = await pk.Patients.Lookup("Clinical", new string[] { "HNC-0522c0009" });
var patient = await patients[0].GetAsync();
var metadata = await patient.GetMetadataAsync();
metadata["Genetic Type"] = "Type II";
await patient.SetMetadataAsync(metadata);
await patient.SaveAsync();
```

This code will not work in your setup for several reasons:

* Your `baseUrl` for your ProKnow account is not "https://example.proknow.com."
* Your `credentials.json` file may not be located at the path "./credentials.json.""
* You probably do not have a patient with the MRN "HNC-0522c0009" in a workspace called "Clinical."
* You probably do not have a custom metric called "Genetic Type."

If you run into problems while running your script, examine the error message and make sure you didn't copy a code
example without making the proper modifications. Values may need to be replaced or additional setup code may need to
be added in order for your code to function properly.
