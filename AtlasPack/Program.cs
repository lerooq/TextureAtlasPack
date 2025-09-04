using System.Text.Json;
using System.Text.Json.Serialization;
using AtlasPack;
using AtlasPack.Models;
using SixLabors.ImageSharp;

var folder = args[0];

var configFilePath = args[0] + @"\atlas.json";

var config = JsonSerializer.Deserialize<AtlasConfig>(
    File.ReadAllText(configFilePath),
    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
);

var (atlas, metadata) = AtlasBuilder.BuildAtlas(folder, config!);

atlas.Save(folder + @"\atlas.png");
File.WriteAllText(folder + @"\atlas_metadata.json",
    JsonSerializer.Serialize(metadata,
        options: new JsonSerializerOptions
            { WriteIndented = true, DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull }));

Console.WriteLine("Atlas generated: atlas.png");
Console.WriteLine("Metadata written: atlas_metadata.json");