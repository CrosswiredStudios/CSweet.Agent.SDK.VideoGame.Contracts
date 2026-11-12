namespace CSweet.Agent.SDK.VideoGame.Contracts;

/// <summary>Optional GDD coordination protocol. Document layout and prose are unrestricted.</summary>
public static class GameCreativeDesignProtocolV1
{
    public const string RequestType = "video-game.creative-design.request.v1";
    public const string ResultType = "video-game.creative-design.result.v1";
    public const string FeedbackType = "video-game.creative-design.feedback.v1";
    public const string FailureType = "video-game.creative-design.failure.v1";
    public const string DocumentType = "video-game.gdd.v1";
}

// Property names and order preserve the JSON exchanged by earlier Director/Creative Director agents.
public sealed record GameCreativeDesignRequestV1(Guid WorkstreamId, Guid TeamId, Guid DirectorEmployeeId,
    Guid CreativeDirectorEmployeeId, Guid ProducerEmployeeId, Guid PitchArtifactId, Guid PitchRevisionId, string PitchSha256);

public sealed record GameCreativeDesignResultV1(string PitchSha256, Guid DocumentId, Guid RevisionId, string RevisionSha256);

public sealed record GameCreativeDesignFeedbackV1(Guid DocumentId, Guid RevisionId, string Summary,
    IReadOnlyList<GameCreativeDesignFindingV1> Findings);

public sealed record GameCreativeDesignFindingV1(string Code, string Section, string Severity, bool Blocking,
    string Summary, string? RequiredFollowUp);

/// <summary>Machine-readable failure bound to the assigned project, team and pitch; Summary is for people.</summary>
public sealed record GameCreativeDesignFailureV1(Guid WorkstreamId, Guid TeamId, string PitchSha256, string Code, string Summary);

public static class GameCreativeDesignFailureCodes
{
    public const string EmptyGeneration = "gdd.generation.empty";
    public const string IncompleteGeneration = "gdd.generation.incomplete";
    public const string ReviewInvalid = "gdd.review.invalid";
    public const string ReviewUnavailable = "gdd.review.unavailable";
}

/// <summary>Semantic coverage identifiers, not required headings or document fields.</summary>
public static class GameGddCriterionKeys
{
    public const string CreativeIntent = "creative-intent";
    public const string PlayerExperience = "player-experience";
    public const string Gameplay = "gameplay";
    public const string Presentation = "presentation";
    public const string AcceptanceCriteria = "acceptance-criteria";
    public const string Constraints = "constraints";
    public const string OpenDecisions = "open-decisions";
    public static IReadOnlyList<string> All { get; } = Array.AsReadOnly(new[] {
        CreativeIntent, PlayerExperience, Gameplay, Presentation, AcceptanceCriteria, Constraints, OpenDecisions });
}
