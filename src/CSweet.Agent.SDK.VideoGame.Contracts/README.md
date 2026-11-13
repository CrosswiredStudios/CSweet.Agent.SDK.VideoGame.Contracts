# CSweet.Agent.SDK.VideoGame.Contracts

Optional shared contracts for C-Sweet video-game agents. The C-Sweet Agent SDK and the platform do not reference this package; add it only to agents that exchange video-game planning and production payloads.

Contents (namespace `CSweet.Agent.SDK.VideoGame.Contracts`):

- Planning payloads: `GameProductionPlanningCycleV1`, `GameProposedWorkItemV1`, `GameDesignerBacklogProposalV1`, `GameTechnicalDeliveryProposalV1`, `ReconciledGameProductionPlanV1`, estimate, capacity and QA sprint-readiness records.
- Keys: `VideoGameRoleKeys`, `VideoGameSpecializationKeys`, `VideoGameWorkItemTypeKeys`, `VideoGameArtifactTypeKeys`, lifecycle phases, milestones and toolchain recipes.
- Production records: vision brief, asset strategy, creative review rubric, toolchain recommendation, playtest plan and report, status-report extension.
- `GameProposedWorkItemV1.BrowserVerification` with `GameBrowserVerification` (`canvas`, `page`, `none`): the browser evidence a planned task's candidate must carry.
- `GameProposedWorkItemV1.InteractionCriteria` (1.2.0): zero-based indexes of the acceptance criteria that need live browser interaction evidence. The Producer records it on the ticket as a `browser-interaction` verification requirement (`CSweet.WorkManagement.Contracts` 5.5.0); QA requires behavioral checks for those criteria only.
- Research spikes use `VideoGameWorkItemTypeKeys.ResearchSpike`. A spike's deliverable is a record of what was observed; it is judged on whether it covers what it must capture and does not contradict observed behavior, not on whether the observed behavior was correct.

Wire compatibility: JSON shapes and string keys are the same as the per-agent `CrosswiredStudios.VideoGame.Contracts` source snapshots these types came from. Only the CLR namespace differs, so agents on the package and agents still on a snapshot exchange the same payloads.


## GDD coordination (1.1.0)

`GameCreativeDesignProtocolV1` defines the existing `video-game.creative-design.request.v1`,
`result.v1` and `feedback.v1` keys plus the additive `failure.v1` key. The corresponding
records are `GameCreativeDesignRequestV1`, `GameCreativeDesignResultV1`,
`GameCreativeDesignFeedbackV1`, `GameCreativeDesignFindingV1` and
`GameCreativeDesignFailureV1`. Existing request/result/feedback JSON property names
and order are preserved. These records carry identity and review outcomes, not document layout.

Third-party implementations can exchange ordinary JSON without referencing this
assembly. A result payload uses the camel-case shape below; additional provider
properties and platform document-reference metadata may coexist with these fields.

```json
{
  "pitchSha256": "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa",
  "documentId": "11111111-1111-1111-1111-111111111111",
  "revisionId": "22222222-2222-2222-2222-222222222222",
  "revisionSha256": "bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb"
}
```

The values must identify the authorized project pitch and exact submitted GDD revision.
The surrounding coordination artifact uses `ResultType`, schema version `1.0` and
the pitch digest as its key. Platform grants and current assignment authority remain required.

`GameGddCriterionKeys` names semantic coverage subjects, not mandatory headings,
section order or content fields. Equivalent prose, tables, outlines or other readable
document structures can cover the subjects. Agents perform semantic review; this
contracts package contains no prompts, model execution or document parser.

Failure payloads bind `workstreamId`, `teamId`, `pitchSha256`, `code` and a human
`summary`. `GameCreativeDesignFailureCodes` distinguishes empty/incomplete generation
from invalid/unavailable semantic review. Consumers use the stable code and scope;
they must not infer authorization or recovery eligibility from localized summary text.
Unknown failures require explicit diagnosis rather than being treated as a known retry.

The core Agent SDK and platform continue to have no dependency on this package.
