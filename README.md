<p align="center">
    <img src="https://github.com/user-attachments/assets/4b024dea-dbd3-475c-9a7b-faf2869d5c8f" width="200" height="200" />
</p>

[![Build and Publish](https://github.com/kris701/DatabaseSharp/actions/workflows/dotnet-desktop.yml/badge.svg)](https://github.com/kris701/DatabaseSharp/actions/workflows/dotnet-desktop.yml)
![Nuget](https://img.shields.io/nuget/v/DatabaseSharp)
![Nuget](https://img.shields.io/nuget/dt/DatabaseSharp)
![GitHub last commit (branch)](https://img.shields.io/github/last-commit/kris701/DatabaseSharp/main)
![GitHub commit activity (branch)](https://img.shields.io/github/commit-activity/m/kris701/DatabaseSharp)
![Static Badge](https://img.shields.io/badge/Platform-Windows-blue)
![Static Badge](https://img.shields.io/badge/Platform-Linux-blue)
![Static Badge](https://img.shields.io/badge/Framework-dotnet--8.0-green)


# DatabaseSharp

This is a project to introduce simpler type conversion when communicating with a database.

The library here is designed to only communicate through STPs, since it is considered more secure than free SQL.

An example of how this works can be seen below:
```csharp
var client = new DBClient("<--Database Connection String Here-->");
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

var client = new DBClient("<--Database Connection String Here-->");
var dataset = await client.ExecuteAsync("some-stp");
var table = dataset[0];
var row = table[0];

SomeClass filled = row.Fill<SomeClass>();
```

These are just the methods on the row level. 
However you can also use these on the Table level, where it will instead make a list of the items, as such:
```csharp
var client = new DBClient("<--Database Connection String Here-->");
var dataset = await client.ExecuteAsync("some-stp");
var table = dataset[0];

List<int> someValue = table.GetAllValues<int>("COL_NAME");
```

The project is available as a package on the [NuGet Package Manager](https://www.nuget.org/packages/DatabaseSharp/).
