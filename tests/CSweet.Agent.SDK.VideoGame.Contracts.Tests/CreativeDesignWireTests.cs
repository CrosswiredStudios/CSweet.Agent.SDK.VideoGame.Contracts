using System.Text.Json;
using CSweet.Agent.SDK.VideoGame.Contracts;

namespace CSweet.Agent.SDK.VideoGame.Contracts.Tests;

public sealed class CreativeDesignWireTests
{
    private static readonly JsonSerializerOptions Web = new(JsonSerializerDefaults.Web);

    [Fact]
    public void Request_and_result_keep_the_existing_wire_shape_and_property_order()
    {
        var request = new GameCreativeDesignRequestV1(Guid.NewGuid(),Guid.NewGuid(),Guid.NewGuid(),Guid.NewGuid(),Guid.NewGuid(),Guid.NewGuid(),Guid.NewGuid(),new string('a',64));
        var legacy = new { request.WorkstreamId,request.TeamId,request.DirectorEmployeeId,request.CreativeDirectorEmployeeId,
            request.ProducerEmployeeId,request.PitchArtifactId,request.PitchRevisionId,request.PitchSha256 };
        Assert.Equal(JsonSerializer.Serialize(legacy,Web),JsonSerializer.Serialize(request,Web));
        var result = new GameCreativeDesignResultV1(request.PitchSha256,Guid.NewGuid(),Guid.NewGuid(),new string('b',64));
        Assert.Equal(JsonSerializer.Serialize(new {result.PitchSha256,result.DocumentId,result.RevisionId,result.RevisionSha256},Web),JsonSerializer.Serialize(result,Web));
    }

    [Fact]
    public void Existing_feedback_reads_and_round_trips_without_changes()
    {
        var finding = new { code="missing-method",section="Validation plan",severity="Major",blocking=true,summary="Describe planned observation",requiredFollowUp="Name the method and owner" };
        var legacy = JsonSerializer.Serialize(new { documentId=Guid.NewGuid(),revisionId=Guid.NewGuid(),summary="Needs a validation method",findings=new[] {finding} },Web);
        var feedback = JsonSerializer.Deserialize<GameCreativeDesignFeedbackV1>(legacy,Web)!;
        Assert.Equal(legacy,JsonSerializer.Serialize(feedback,Web));
    }

    [Fact]
    public void A_third_party_can_exchange_plain_JSON_without_referencing_the_package()
    {
        const string json = """
            {"pitchSha256":"aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa",
             "documentId":"11111111-1111-1111-1111-111111111111","revisionId":"22222222-2222-2222-2222-222222222222",
             "revisionSha256":"bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb",
             "documentReferences":[],"vendorExtension":{"format":"plain-text"}}
            """;
        var result = JsonSerializer.Deserialize<GameCreativeDesignResultV1>(json,Web)!;
        using var emitted = JsonDocument.Parse(JsonSerializer.Serialize(result,Web));
        Assert.Equal("11111111-1111-1111-1111-111111111111",emitted.RootElement.GetProperty("documentId").GetString());
        Assert.Equal(new string('a',64),result.PitchSha256);
    }

    [Fact]
    public void Typed_failure_keeps_code_and_scope_separate_from_human_prose()
    {
        var failure = new GameCreativeDesignFailureV1(Guid.NewGuid(),Guid.NewGuid(),new string('a',64),GameCreativeDesignFailureCodes.EmptyGeneration,"Aucun contenu fourni.");
        var decoded = JsonSerializer.Deserialize<GameCreativeDesignFailureV1>(JsonSerializer.Serialize(failure,Web),Web);
        Assert.Equal(failure,decoded);
        Assert.Equal("gdd.generation.empty",decoded!.Code);
    }

    [Fact]
    public void Stored_protocol_keys_are_unchanged_and_the_package_remains_independent()
    {
        Assert.Equal("video-game.creative-design.request.v1",GameCreativeDesignProtocolV1.RequestType);
        Assert.Equal("video-game.creative-design.result.v1",GameCreativeDesignProtocolV1.ResultType);
        Assert.Equal("video-game.creative-design.feedback.v1",GameCreativeDesignProtocolV1.FeedbackType);
        Assert.DoesNotContain(typeof(GameCreativeDesignRequestV1).Assembly.GetReferencedAssemblies(),x=>x.Name!.StartsWith("CSweet.Agent.SDK"));
        Assert.Equal(7,GameGddCriterionKeys.All.Distinct().Count());
    }
}
