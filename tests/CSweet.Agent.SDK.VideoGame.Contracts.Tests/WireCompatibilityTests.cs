using System.Text.Json;
using CSweet.Agent.SDK.VideoGame.Contracts;

namespace CSweet.Agent.SDK.VideoGame.Contracts.Tests;

/// <summary>
/// These types moved from per-agent source snapshots (namespace CrosswiredStudios.VideoGame.Contracts). Agents on the
/// package and agents still on a snapshot must keep exchanging identical JSON, and retained planning digests must not change.
/// </summary>
public sealed class WireCompatibilityTests
{
    private static GameProposedWorkItemV1 Item(string? browserVerification = null) =>
        new("core-loop", VideoGameWorkItemTypeKeys.Task, "Core loop", "Break blocks.", ["A block breaks."],
            VideoGameRoleKeys.Engineer, [], ["gameplay-programming"], ["work.execution.run"], ["dependency-pin-phaser"])
        { ParentProposalKey = "story", BrowserVerification = browserVerification };

    [Fact]
    public void A_proposal_without_browser_verification_serializes_exactly_as_the_snapshot_did()
    {
        // Property order and names of the snapshot record, which retained digests were computed over.
        const string expected = "{\"ProposalKey\":\"core-loop\",\"WorkItemTypeKey\":\"video-game.task.v1\",\"Title\":\"Core loop\"," +
            "\"Description\":\"Break blocks.\",\"AcceptanceCriteria\":[\"A block breaks.\"],\"AccountableRoleKey\":\"game-engineer\"," +
            "\"RequiredSpecializationKeys\":[],\"PreferredSpecializationKeys\":[\"gameplay-programming\"],\"RequiredCapabilityKeys\":[\"work.execution.run\"]," +
            "\"DependencyProposalKeys\":[\"dependency-pin-phaser\"],\"ParentProposalKey\":\"story\"}";

        Assert.Equal(expected, JsonSerializer.Serialize(Item()));
    }

    [Fact]
    public void Browser_verification_round_trips_and_older_payloads_still_read()
    {
        var web = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        var declared = JsonSerializer.Deserialize<GameProposedWorkItemV1>(JsonSerializer.Serialize(Item(GameBrowserVerification.Canvas), web), web)!;
        Assert.Equal(GameBrowserVerification.Canvas, declared.BrowserVerification);

        var legacy = JsonSerializer.Deserialize<GameProposedWorkItemV1>(JsonSerializer.Serialize(Item(), web), web)!;
        Assert.Null(legacy.BrowserVerification);
        Assert.Equal(JsonSerializer.Serialize(Item()), JsonSerializer.Serialize(legacy));
    }

    [Fact]
    public void Browser_verification_offers_exactly_three_choices()
    {
        Assert.Equal(new[] { "canvas", "page", "none" }, GameBrowserVerification.All);
    }

    [Theory]
    [InlineData(VideoGameRoleKeys.GameDirector, "game-director")]
    [InlineData(VideoGameRoleKeys.CreativeDirector, "creative-director")]
    [InlineData(VideoGameRoleKeys.Producer, "game-producer")]
    [InlineData(VideoGameRoleKeys.TechnicalDirector, "game-technical-director")]
    [InlineData(VideoGameRoleKeys.Engineer, "game-engineer")]
    [InlineData(VideoGameRoleKeys.QualityAssurance, "game-quality-assurance")]
    [InlineData(VideoGameWorkItemTypeKeys.Task, "video-game.task.v1")]
    public void Stored_keys_keep_their_wire_values(string actual, string expected) => Assert.Equal(expected, actual);
}
