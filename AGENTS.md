# C-Sweet Video Game Contracts contributor instructions

## What belongs here

- Shared wire records and string keys that more than one video-game agent exchanges. Agent-specific
  prompts, planning logic and adapters stay in the agents.
- This package is optional. The C-Sweet Agent SDK and the platform must never reference it.

## Compatibility

- JSON shapes and key values must stay compatible with agents that still compile the older
  `CrosswiredStudios.VideoGame.Contracts` source snapshot. Add optional members; do not rename,
  reorder or remove existing ones. New optional members that are null by default must be omitted
  from JSON when null (`JsonIgnore(Condition = WhenWritingNull)`) so retained planning digests do not change.
- Never change an existing key's string value; stored artifacts and pinned workstreams use it.

## Package versioning

- Every edit to the contracts project must increment `Version` in
  `src/CSweet.Agent.SDK.VideoGame.Contracts/CSweet.Agent.SDK.VideoGame.Contracts.csproj` and add
  `releases/<version>.md` whose first line is `# <version>`.
- Use semantic versioning: patch for compatible maintenance, minor for additive contracts, major
  for breaking contracts.
- Update pinned references in downstream agent repositories changed as part of the same work.
- Run `dotnet test CSweet.Agent.SDK.VideoGame.Contracts.slnx` and pack the project. Confirm the
  `.nupkg` filename carries the new version before handoff.
