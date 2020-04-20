# Introduction 
proknow-sdk is a .NET Core software development kit (SDK) to interface with [ProKnow](https://proknow.com) from .NET
applications.

# Installation
Install the SDK using [NuGet](https://nuget.org/packages/ProKnow).

# Basic Usage
To use the SDK, you'll need two important items:

- Base URL:  This is the URL that will be used to make API requests to ProKnow.  It's the first part of the web address
you see when you are signed into your ProKnow account.  The form of this URL is typically
`https://your-domain.proknow.com`.

- API Token: You can generate an [API token](https://support.proknow.com/article/165-configuring-your-profile#api-keys)
in the ProKnow user interface. Always keep your API token secret.  Once you have your `credentials.json`, make note of
the file path.

The code snippet below can be used to test your installation and configuration of the `proknow-sdk` package.  Please be
sure to replace `https://example.proknow.com` with your organization's base URL described above, and replace
`./credentials.json` with the path to your credentials file:

```
using ProKnow;
using System.Threading.Tasks;

var pk = new ProKnowApi('https://example.proknow.com', './credentials.json');
var status = await pk.GetStatusAsync();
if (status != "OK")
{
    throw new Exception($"Error connecting to ProKnow API: {status}.");
}
```

Note that the majority of the methods in the SDK are asynchronous.  Asynchronous methods are named with the 'Async'
suffix per convention.  For more information about asynchronous programming, please refer to the Microsoft
[Asynchronous programming with async and await](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/async/)
article.

If an adaptation of the above code snippet does not work for you, please see the troubleshooting guide.
