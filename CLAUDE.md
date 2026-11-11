# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## What this repo is

`essentialMix.Net` is a set of NuGet libraries layered on top of the core `essentialMix` packages (`essentialMix`, `essentialMix.Data`, `essentialMix.Windows`, `essentialMix.Threading`, etc.). The core packages live in a separate sibling repo (`../essentialMix`) and are consumed here **only as `PackageReference`s from nuget.org**, never as project references. A change that needs a new core API must land and be published in the core repo first, then the version here bumped.

Projects in `essentialMix.sln`:

- Libraries (each packs to its own NuGet package, `PackageId = $(ProjectName)`):
  - `essentialMix.Web`: ASP.NET Core helpers (middleware, MVC/controllers, tag helpers, auth, security headers through NWebsec, Polly HTTP).
  - `essentialMix.Authentication.JwtBearer`, `essentialMix.Swagger`, `essentialMix.Mail` (SMTP/SendGrid/log `IEmailSender`), `essentialMix.Web.Formatters.Csv`, `essentialMix.Web.Formatters.NewtonsoftJson`.
  - `essentialMix.Data.Entity`: EF Core implementation of the repository/unit-of-work/service patterns whose base interfaces are defined in the core `essentialMix.Data` package (`essentialMix.Data.Patterns.Repository`, `essentialMix.Data.Model.IEntity`). Interfaces are generic over `TContext, TEntity` plus up to 4 key types (`TKey1..TKey4`); keep all arities in sync when changing one.
  - `essentialMix.Data.Entity.Mapster`: Mapster-based services (in-memory `Map<TEntity, T>` plus `ProjectToType<T>` query projections) and `MappingConfiguration.Create(...)`, which builds a `TypeAdapterConfig` with the global settings consumers should share. The only library with a `ProjectReference` (to `essentialMix.Data.Entity`). It replaced the previous mapping library, which needs a commercial license; never add that library back (`NoAutoMapperTests` guards against it). The library is slated for deprecation.
  - `essentialMix.WinForms`, `essentialMix.WPF`: `net10.0-windows` UI controls and helpers. The WinForms `packages.config`/`app.config` are legacy leftovers.
- Test projects (not packed):
  - `Test.Data.Entity.Mapping`: NUnit tests for `essentialMix.Data.Entity.Mapster` against SQLite in memory. `Snapshots/*.json` are golden masters recorded under the previous mapping library: they are the spec, so a mismatch is a bug to fix, never a reason to re-record (`RECORD_SNAPSHOTS=1` writes them).
  - `Test.App`: console scratch app; edit `Program.cs` to exercise code.
  - `Test.WPF`: WPF app that references `essentialMix.WPF`.
  - `Test.Common` (`netstandard2.1`): shared Bogus-based fake data (`Generator`, `Model/`).

The only automated tests are in `Test.Data.Entity.Mapping`; everything else is verified by building plus running `Test.App`/`Test.WPF`.

## Commands

Windows only (WinForms/WPF projects target `net10.0-windows`). Requires the .NET 10 SDK.

```powershell
dotnet build essentialMix.sln                      # Debug, AnyCPU
dotnet build essentialMix.sln -c Release           # also creates .nupkg/.snupkg in .\nuget (GeneratePackageOnBuild)
dotnet build essentialMix.Web/essentialMix.Web.csproj
dotnet test Test.Data.Entity.Mapping              # NUnit mapping, golden-master and guard tests
dotnet run --project Test.App                      # Test.App defaults to Platform x64
dotnet run --project Test.WPF
publish.bat <nuget-api-key>                        # pushes every .\nuget\*.nupkg to nuget.org (--skip-duplicate)
```

Build Release packages through the solution: `PackageOutputPath` is `$(SolutionDir)nuget`, which is wrong when `SolutionDir` is unset. Library output goes to `bin\<Configuration>\<Platform>\<TargetFramework>\` at the repo root.

## Project conventions

- No `Directory.Build.props`: every `.csproj` repeats its own properties (configurations, platforms `AnyCPU;x64;x86`, `LangVersion 14.0` except `Test.Common` at `13.0`, package metadata). Edit each file when changing shared settings.
- Versioning: `AssemblyVersion`, `Version`, `PackageVersion` (e.g. `17.0.15.0`), and `FileVersion` (date-based `yy.MM.dd`) are hand-set per library project and kept identical across all of them. The `essentialMix.*` package references are bumped together to the same version; Microsoft packages are pinned to the matching .NET 10 patch (e.g. `10.0.14`).
- Every project links the root `GlobalSuppressions.cs`, which suppresses `var` suggestions, `IDE0058`, and allocation analyzers on purpose; `.editorconfig` disables `IDE0090` (`new(...)`) and `IDE0063` (simple `using`). Code style follows from that: explicit types instead of `var`, `default(T)` written out, tab indentation, file-scoped namespaces, JetBrains `[NotNull]` annotations on public APIs.
- Namespaces match the package name (`essentialMix.Data.Entity.Patterns.Repository`); the EF `DbContext` is aliased as `SystemDbContext` to avoid clashing with this repo's own `essentialMix.Data.Entity.DbContext`.
- Branch `stage` is the main branch.

## Do not touch

`sign.ps1`/`sign.bat` generate a self-signed code-signing certificate interactively, and `sign.txt` holds a secret. Never print, commit changes to, or use these files.
