using System.Diagnostics.CodeAnalysis;

namespace AniLens.Server.Settings;

[SuppressMessage("ReSharper", "InconsistentNaming")]
public abstract class MongoDbSettingsBase
{
    public string ConnectionUri { get; set; } = null!;
    public string DatabaseName { get; set; } = null!;
    public string CollectionName { get; set; } = null!;
}