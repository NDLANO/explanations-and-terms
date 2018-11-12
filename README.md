# NDLA Forklaringstjenste API
![Build Status](https://codebuild.eu-west-1.amazonaws.com/badges?uuid=eyJlbmNyeXB0ZWREYXRhIjoienRYSi92THhCVXF5dzNjWjlINm1icU1KdnljaTFPSVpUaHprKzdPUVR1TE9BZHphN1ZaYlI2OURPc1RMS0txSUJVWXJHT1liK3ZubmQzTnAxbCtiNjgwPSIsIml2UGFyYW1ldGVyU3BlYyI6IlVLYXFGQTVnODczSmVISDAiLCJtYXRlcmlhbFNldFNlcmlhbCI6MX0%3D&branch=master)

## Requirements

- .NET Core ~2.1
- Docker (optional)


### Dependencies
All of the following commands must be executed inside a project.

All dependencies are defined in `*.csproj*` files, inside each project. They are managed by the dotnet-cli. To
initially install all dependencies and when the list dependency has changed, navigate to a project and run `dotnet restore`.

```
$ dotnet restore
```

### Start development server

Start the development server on port 5000.

```
$ dotnet run
```

### Unit tests

Run unittests.

```
$ dotnet test
```


## Dependencies

### ConceptsMicroservice

**Microsoft.AspNetCore.App:**
Provides a default set of APIs for building an ASP.NET Core application.
https://github.com/aspnet/AspNetCore

**Microsoft.AspNetCore.Cors:**
Includes the core implementation for CORS policy, utilized by the CORS middleware and MVC.
https://github.com/aspnet/CORS

**Microsoft.VisualStudio.Azure.Containers.Tools.Targets:**
Supports building, debugging, and running containerized ASP.NET Core apps targeting .NET Core in both Windows and Linux containers.
https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/docker/visual-studio-tools-for-docker?view=aspnetcore-2.1

**Npgsql.EntityFrameworkCore.PostgreSQL:**
An Entity Framework Core provider built on top of Npgsql. It allows you to use the EF Core O/RM with PostgreSQL.
https://github.com/npgsql/Npgsql.EntityFrameworkCore.PostgreSQL

**NSwag.AspNetCore:**
NSwag is a Swagger/OpenAPI 2.0 and 3.0 toolchain for .NET, .NET Core, Web API, ASP.NET Core.
https://github.com/RSuter/NSwag

**Newtonsoft.Json:**
A package for serializing and deserializing .NET objects to JSON.
https://www.newtonsoft.com/json

### ConceptsMicroservice.UnitTests

**FakeItEasy**
A .Net dynamic fake framework for creating all types of fake objects, mocks, stubs etc.
https://fakeiteasy.github.io/

**Microsoft.NET.Test.Sdk**
The MSbuild targets and properties for building .NET test projects.
https://github.com/microsoft/vstest/

**Npgsql.EntityFrameworkCore.PostgreSQL:**
An Entity Framework Core provider built on top of Npgsql. It allows you to use the EF Core O/RM with PostgreSQL.
https://github.com/npgsql/Npgsql.EntityFrameworkCore.PostgreSQL

**xunit**
xUnit.net is a free, open source, community-focused unit testing tool for the .NET Framework.
https://xunit.github.io/

**xunit.runner.visualstudio**
Visual Studio 2012+ Test Explorer runner for the xUnit.net framework.
https://www.nuget.org/packages/xunit.runner.visualstudio/
