# CSweet.Agent.SDK.VideoGame.Contracts

Optional NuGet package of shared contracts for C-Sweet video-game agents. See the [package README](src/CSweet.Agent.SDK.VideoGame.Contracts/README.md) for its contents.

## Why it exists

The video-game agents used to compile their own copies of these types from an `extensions/video-game` source snapshot in each agent repository. A shared change had to be copied into every agent and its provenance hashes refreshed. This package holds the shared contracts once; agents reference a published version.

The C-Sweet Agent SDK stays domain-neutral and does not depend on this package.

## Build

```
dotnet test CSweet.Agent.SDK.VideoGame.Contracts.slnx
dotnet pack src/CSweet.Agent.SDK.VideoGame.Contracts/CSweet.Agent.SDK.VideoGame.Contracts.csproj -c Release -o artifacts/packages
```

## Releases

Pushing a version change to `main` (or a `v*.*.*` tag) publishes to NuGet.org through trusted publishing and creates a GitHub release from `releases/<version>.md`. See [AGENTS.md](AGENTS.md) for versioning and compatibility rules.

Version 1.1.0 adds optional GDD coordination contracts and semantic coverage identifiers. The package README documents raw JSON exchange for third-party agents and preserves earlier request/result/feedback wire shapes. Document structures remain unrestricted; review and recovery execute in the agents.
