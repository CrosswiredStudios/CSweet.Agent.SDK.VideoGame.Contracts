using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using CSweet.Agent.SDK.VideoGame.Contracts;

namespace CSweet.Agent.SDK.VideoGame.Contracts.Tests;

public sealed class GameOutcomeLedgerTests
{
    [Fact]
    public void Bundled_sources_bind_exact_text_authorship_and_revision_and_reject_substitution()
    {
        var project = Guid.NewGuid();
        var team = Guid.NewGuid();
        var pitch = "Keep the budget fixed.";
        var gdd = "Rebound the ball.\nObserve the angle.\n\nBreak the brick.";
        static object Source(string text) => new {
            DocumentId = Guid.NewGuid(), RevisionId = Guid.NewGuid(), DocumentType = "text",
            Sha256 = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(text))) };
        var header = JsonSerializer.Serialize(new { Version = 1, WorkstreamId = project, TeamId = team,
            Pitch = Source(pitch), Gdd = Source(gdd), PitchCharacters = pitch.Length, GddCharacters = gdd.Length });
        var content = "# Exact approved production sources\n\n```json\n" + header + "\n```\n\n## Accepted pitch\n" + pitch + "\n\n## Accepted GDD\n" + gdd;
        IReadOnlyList<GameOutcomeSource> Read(string bundled) => GameOutcomeLedger.Read(project, team, Guid.Empty, Guid.Empty,
            "approved-package", "video-game.production-sources.v1", bundled);
        var rows = Read(content);
        Assert.Equal(3, rows.Count);
        Assert.Equal(VideoGameRoleKeys.GameDirector, rows[0].AuthorRole);
        Assert.Equal(VideoGameRoleKeys.CreativeDirector, rows[1].AuthorRole);
        Assert.Equal("Rebound the ball.\nObserve the angle.", rows[1].ApprovedText);
        Assert.Equal(rows[1].Key, Read(content)[1].Key);
        Assert.Throws<InvalidOperationException>(() => Read(content.Replace("ball.", "wall.")));
    }
}
