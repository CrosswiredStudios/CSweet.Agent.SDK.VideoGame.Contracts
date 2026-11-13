using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace CSweet.Agent.SDK.VideoGame.Contracts;

public sealed record GameOutcomeSource(string Key, Guid DocumentId, Guid RevisionId, string RevisionDigest, string AuthorRole, string ApprovedText);

/// <summary>Derives a reproducible ledger from the exact package members and their length-delimited approved source revisions.</summary>
public static class GameOutcomeLedger
{
    private sealed record Source(Guid DocumentId, Guid RevisionId, string Sha256, string DocumentType, string Content);
    private sealed record SourceHeader(int Version, Guid WorkstreamId, Guid TeamId, Source Pitch, Source Gdd, int PitchCharacters, int GddCharacters);
    public static IReadOnlyList<GameOutcomeSource> Read(Guid project, Guid team, Guid documentId, Guid revisionId,
        string revisionDigest, string type, string content)
    {
        var rows = new List<GameOutcomeSource>();
        void Add(Guid doc, Guid rev, string digest, string role, string original)
        {
            var paragraphs = Regex.Split(original, @"(?:\r?\n)[ \t]*(?:\r?\n)+");
            for (var i = 0; i < paragraphs.Length; i++)
            {
                var paragraph = paragraphs[i];
                if (string.IsNullOrWhiteSpace(paragraph)) continue;
                rows.Add(new($"{doc:N}/{rev:N}/{i}", doc, rev, digest, role, paragraph));
            }
        }
        if (type != "video-game.production-sources.v1")
        {
            var end = content.IndexOf("<!-- production-sources-reference -->", StringComparison.Ordinal);
            Add(documentId, revisionId, revisionDigest, VideoGameRoleKeys.Producer, end < 0 ? content : content[..end]);
            return rows;
        }
        const string prefix = "# Exact approved production sources\n\n```json\n";
        const string start = "\n```\n\n## Accepted pitch\n";
        const string divider = "\n\n## Accepted GDD\n";
        var headerEnd = content.IndexOf(start, StringComparison.Ordinal);
        if (!content.StartsWith(prefix, StringComparison.Ordinal) || headerEnd < prefix.Length)
            throw new InvalidOperationException("The accepted production sources need a length-delimited header.");
        var header = JsonSerializer.Deserialize<SourceHeader>(content[prefix.Length..headerEnd], new JsonSerializerOptions(JsonSerializerDefaults.Web));
        if (header == null || header.Version != 1 || header.WorkstreamId != project || header.TeamId != team ||
            header.PitchCharacters < 0 || header.GddCharacters < 0 ||
            (long)headerEnd + start.Length + header.PitchCharacters + divider.Length + header.GddCharacters != content.Length)
            throw new InvalidOperationException("Source header differs from the approved package.");
        int offset = headerEnd + start.Length;
        var pitch = content.Substring(offset, header.PitchCharacters);
        offset += header.PitchCharacters;
        if (!content.AsSpan(offset).StartsWith(divider, StringComparison.Ordinal)) throw new InvalidOperationException("Invalid GDD boundary.");
        var gdd = content[(offset + divider.Length)..];
        foreach (var (source, role, original) in new[] { (header.Pitch, VideoGameRoleKeys.GameDirector, pitch), (header.Gdd, VideoGameRoleKeys.CreativeDirector, gdd) })
        {
            if (source == null || source.DocumentId == Guid.Empty || source.RevisionId == Guid.Empty ||
                !Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(original))).Equals(source.Sha256, StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Source revision content differs from the approved digest.");
            Add(source.DocumentId, source.RevisionId, source.Sha256, role, original);
        }
        return rows;
    }
}
