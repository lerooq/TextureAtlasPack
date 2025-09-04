using System.Text.Json;
using System.Text.Json.Serialization;
using AtlasPack;
using AtlasPack.Constants;
using AtlasPack.Models;
using SixLabors.ImageSharp;

// Configure
var folder = args[0];
var configFilePath = Path.Combine(args[0], FileNames.InputConfiguration);

// Load
var config = JsonSerializer.Deserialize<AtlasConfig>(
    File.ReadAllText(configFilePath),
    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
);

// Build
var (atlas, metadata) = AtlasBuilder.BuildAtlas(folder, config!);
var textureMaps = MaterialBuilder.BuildAdditionalMaps(config!, metadata, folder);
textureMaps.Add(new AtlasOutputTexture { Image = atlas, Name = FileNames.OutputAlbedo });

// Save
foreach (var textureMap in textureMaps)
{
    textureMap.Image.Save(Path.Combine(folder, textureMap.Name));
}

File.WriteAllText(Path.Combine(folder, FileNames.OutputMetadata),
    JsonSerializer.Serialize(metadata,
        options: new JsonSerializerOptions
            { WriteIndented = true, DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull }));

// Write output
Console.WriteLine($"Atlas textures generated: {string.Join(", ", textureMaps.Select(m => m.Name))}");
Console.WriteLine($"Atlas metadata written: {FileNames.OutputMetadata}");