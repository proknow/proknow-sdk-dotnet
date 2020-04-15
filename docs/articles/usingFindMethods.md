# Using Find Methods

## Basics

Within the SDK, there are many classes that support a `FindAsync` method for performing a search over a collection of
items to find an item matching some criteria. This is can be useful when you know something like the name of an item
but not its ID. These methods typically have the following signature:

```
public async Task<T> FindAsync(Func<T, bool> predicate)
```

## Predicate

The `predicate` is a function that is passed an item as input and which should return a bool indicating whether the
item is a match.

For example, to find a patient names "Terence Arnold" in the "Clinical" workspace:
```
using ProKnow;
using System.Linq;
using System.Threading.Tasks;

var pk = new ProKnowApi("https://example.proknow.com", "./credentials.json");
var workspace = await pk.Workspaces.ResolveAsync("Clinical")
var patientSummary = await pk.Patients.FindAsync(workspace.Id, p => p.Name == "Terence Arnold");
```

Note that, in this case, the predicate is a [lambda expression](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/statements-expressions-operators/lambda-expressions).

## Other Important Points

**The method ProKnow.Patient.PatientItem.FindEntities behaves in a similar fashion.**

The method [ProKnow.Patient.PatientItem.FindEntities](xref:ProKnow.Patient.PatientItem#ProKnow_Patient_PatientItem_FindEntities_System_Func_ProKnow_Patient_Entities_EntitySummary_System_Boolean__)
behaves similarly to the `FindAsync` method described above except that the `FindEntities` method traverses through
each entity in the entity hierarchy within a PatientItem to find find matching entities. It returns a list of all
matching entities whereas the `FindAsync` method returns the first matching item it finds.
