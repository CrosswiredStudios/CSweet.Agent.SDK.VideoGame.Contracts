# CSweet.Agent.SDK.VideoGame.Contracts

Optional shared contracts for C-Sweet video-game agents. The C-Sweet Agent SDK and the platform do not reference this package; add it only to agents that exchange video-game planning and production payloads.

Contents (namespace `CSweet.Agent.SDK.VideoGame.Contracts`):

- Planning payloads: `GameProductionPlanningCycleV1`, `GameProposedWorkItemV1`, `GameDesignerBacklogProposalV1`, `GameTechnicalDeliveryProposalV1`, `ReconciledGameProductionPlanV1`, estimate, capacity and QA sprint-readiness records.
- Keys: `VideoGameRoleKeys`, `VideoGameSpecializationKeys`, `VideoGameWorkItemTypeKeys`, `VideoGameArtifactTypeKeys`, lifecycle phases, milestones and toolchain recipes.
- Production records: vision brief, asset strategy, creative review rubric, toolchain recommendation, playtest plan and report, status-report extension.
- `GameProposedWorkItemV1.BrowserVerification` with `GameBrowserVerification` (`canvas`, `page`, `none`): the browser evidence a planned task's candidate must carry.

Wire compatibility: JSON shapes and string keys are the same as the per-agent `CrosswiredStudios.VideoGame.Contracts` source snapshots these types came from. Only the CLR namespace differs, so agents on the package and agents still on a snapshot exchange the same payloads.
