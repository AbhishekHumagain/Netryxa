namespace Static.Application.Common.Dto.CloudFlare.DeleteDto;

public record CloudFlareDeleteDto
{
    public string Identifier { get; init; }
}

public record Result
{
    public string Id { get; init; }
}

public record Root
{
    public Result Result { get; init; }
}