# Mapster migration: checks that need a person

`essentialMix.Data.Entity.AutoMapper` was replaced in place by `essentialMix.Data.Entity.Mapster` (version 17.0.16). The previous mapping library needs a commercial license, so the old package is being retired. The automated tests in `Test.Data.Entity.Mapping` cover this repo. The checks below need a person: they involve nuget.org or the applications that consume the package, which an agent cannot reach.

## Items (all resolved 2026-11-12)

1. **Publish the new package.** Run `dotnet build essentialMix.sln -c Release`, then `publish.bat <api-key>`, and confirm `essentialMix.Data.Entity.Mapster` 17.0.16 shows up on nuget.org.
   - Why a person: it needs the nuget.org API key.
   - When: before any consumer migrates.
   - **Resolved 2026-11-12:** published; nuget.org lists `essentialMix.Data.Entity.Mapster` 17.0.16.

2. **Deprecate `essentialMix.Data.Entity.AutoMapper` on nuget.org.** Mark it deprecated as "legacy", and set `essentialMix.Data.Entity.Mapster` as the alternate package.
   - Why a person: it needs the nuget.org account.
   - When: right after item 1.
   - **Resolved 2026-11-12:** deprecated on nuget.org, pointing to `essentialMix.Data.Entity.Mapster` (confirmed by the user).

3. **Migrate and smoke-test each consuming application.** For each one, check the following:
   - The license warning (`[LuckyPennySoftware.AutoMapper.License] ...`) is gone, and startup completes.
   - Pages that list data still work. They go through `List<T>`/`ListAsync<T>`, which now use Mapster's `ProjectToType`, so EF sees different SQL.
   - Why a person: the applications live outside this repo, and checking them needs real databases and data.
   - When: as each application moves to the new package.
   - **Resolved 2026-11-12:** the user confirmed that the license warning is gone and list pages load in every consuming application.

## Migrating a consuming application

- Replace the package reference. Change `using AutoMapper;` to `using MapsterMapper;`, and `essentialMix.Data.Entity.AutoMapper.Patterns.Services` to `essentialMix.Data.Entity.Mapster.Patterns.Services`.
- Register the mapper with `services.AddSingleton(MappingConfiguration.Create(assemblies))`, plus `services.AddTransient<IMapper, ServiceMapper>()` from `Mapster.DependencyInjection` (or `new Mapper(config)` when no map resolves DI services).
- Turn each `Profile` into an `IRegister`. `IgnoreAll()` is now an extension on `TypeAdapterSetter<TSource, TDestination>`.
- Test each application against golden snapshots recorded under the old library before switching. The four map variants and the pitfalls are listed in the original migration task.

Known differences pinned by `Test.Data.Entity.Mapping/MappingBehaviorTests.cs`:

- **Derived or proxy runtime source types.** `Mapper.Map<T>(object)` throws `Mapster.CompileException` for an EF lazy-loading proxy. The old library fell back to the base type's map. The services map with the static entity type (`Map<TEntity, T>`), so they are not affected. Direct callers must do the same, or register the derived pair.
- **Null top-level collection.** `Map<List<T>>(null)` returns `null`, where the old library returned an empty list. The services never map a top-level collection.
- **Custom member expressions are not null-propagated in memory.** `.Map(d => d.Count, s => s.Books.Count)` throws when `Books` is null. The old library returned `0`. Write `s.Books == null ? 0 : s.Books.Count`, which still translates for projections. Plain member chains such as `s.Address.City` stay null-safe.


The following difference comes from an earlier migration. This repo has no conditional maps, so no test here pins it:

- **Conditional maps.** `.Map(d => d.X, s => s.Y, cond)` writes `default` when `cond` is false, where the old `Condition` left the existing value. Use `.Ignore(d => d.X)` plus `.AfterMapping(...)` instead.
