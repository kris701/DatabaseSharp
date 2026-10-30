<p align="center">
    <img src="https://github.com/user-attachments/assets/4b024dea-dbd3-475c-9a7b-faf2869d5c8f" width="200" height="200" />
</p>

[![Build and Publish](https://github.com/kris701/DatabaseSharp/actions/workflows/dotnet.yml/badge.svg)](https://github.com/kris701/DatabaseSharp/actions/workflows/dotnet.yml)
![Nuget](https://img.shields.io/nuget/v/DatabaseSharp)
![Nuget](https://img.shields.io/nuget/dt/DatabaseSharp)
![GitHub last commit (branch)](https://img.shields.io/github/last-commit/kris701/DatabaseSharp/main)
![GitHub commit activity (branch)](https://img.shields.io/github/commit-activity/m/kris701/DatabaseSharp)
![Static Badge](https://img.shields.io/badge/Platform-Windows-blue)
![Static Badge](https://img.shields.io/badge/Platform-Linux-blue)
![Static Badge](https://img.shields.io/badge/Framework-dotnet--10.0-green)


# DatabaseSharp

This is a project to introduce simpler type conversion when communicating with a database.

The library here is designed to only communicate through STPs, since it is considered more secure than free SQL.

An example of how this works can be seen below:
```csharp
var client = new SQLServerClient("<--Database Connection String Here-->");
var dataset = await client.ExecuteAsync("some-stp");
var table = dataset[0];
var row = table[0];

int someValue = row.GetValue<int>("COL_NAME");
```

You can also deserialize directly into class objects like this:
```csharp
public class SomeClass
{
    [DatabaseSharp(ColumnName = "PK_NAME")]
    public string Name { get; set; }
    [DatabaseSharp(ColumnName = "ACT_VALUE")]
    public int Value { get; set; }
}

var client = new SQLServerClient("<--Database Connection String Here-->");
var dataset = await client.ExecuteAsync("some-stp");
var table = dataset[0];
var row = table[0];

SomeClass filled = row.Fill<SomeClass>();
```

These are just the methods on the row level. 
However you can also use these on the Table level, where it will instead make a list of the items, as such:
```csharp
var client = new SQLServerClient("<--Database Connection String Here-->");
var dataset = await client.ExecuteAsync("some-stp");
var table = dataset[0];

List<int> someValue = table.GetAllValues<int>("COL_NAME");
```

The project is available as a package on the [NuGet Package Manager](https://www.nuget.org/packages/DatabaseSharp/).

## Connectors
There are different ADO.NET connectors for different SQL servers.
Some of the adaptors does not have the same capabilities of others.

### SQL Server
![Nuget](https://img.shields.io/nuget/v/DatabaseSharp.SQLServer)
![Nuget](https://img.shields.io/nuget/dt/DatabaseSharp.SQLServer)

You can find it on the [NuGet Package Manager](https://www.nuget.org/packages/DatabaseSharp.SQLServer/).

![Static Badge](https://img.shields.io/badge/Single_Arguments-Yes-green)
![Static Badge](https://img.shields.io/badge/List_Arguments-Yes-green)
![Static Badge](https://img.shields.io/badge/Structured_Arguments-Yes-green)

![Static Badge](https://img.shields.io/badge/Single_Result-Yes-green)
![Static Badge](https://img.shields.io/badge/Multi_Table_Result-Yes-green)

### PostgreSQL Server
![Nuget](https://img.shields.io/nuget/v/DatabaseSharp.PostgreSQL)
![Nuget](https://img.shields.io/nuget/dt/DatabaseSharp.PostgreSQL)

You can find it on the [NuGet Package Manager](https://www.nuget.org/packages/DatabaseSharp.PostgreSQL/).

![Static Badge](https://img.shields.io/badge/Single_Arguments-Yes-green)
![Static Badge](https://img.shields.io/badge/List_Arguments-No-red)
![Static Badge](https://img.shields.io/badge/Structured_Arguments-No-red)

![Static Badge](https://img.shields.io/badge/Single_Result-Yes-green)
![Static Badge](https://img.shields.io/badge/Multi_Table_Result-No-red)

> [!INFO]
> Use `FUNCTIONS` and not `PROCEDURES` with this connector!

> [!WARNING]
> PostgreSQL is very limited in its ADO.NET connector.

### MySQL Server
![Nuget](https://img.shields.io/nuget/v/DatabaseSharp.MySQL)
![Nuget](https://img.shields.io/nuget/dt/DatabaseSharp.MySQL)

You can find it on the [NuGet Package Manager](https://www.nuget.org/packages/DatabaseSharp.MySQL/).

![Static Badge](https://img.shields.io/badge/Single_Arguments-Yes-green)
![Static Badge](https://img.shields.io/badge/List_Arguments-Unknown-white)
![Static Badge](https://img.shields.io/badge/Structured_Arguments-Unknown-white)

![Static Badge](https://img.shields.io/badge/Single_Result-Yes-green)
![Static Badge](https://img.shields.io/badge/Multi_Table_Result-Yes-green)
