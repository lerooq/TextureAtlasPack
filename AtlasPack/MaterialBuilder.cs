using AtlasPack.Constants;
using AtlasPack.Models;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace AtlasPack;

public static class MaterialBuilder
{
    public static List<AtlasOutputTexture> BuildAdditionalMaps(AtlasConfig config, AtlasMetadata metadata,
        string folderPath)
    {
        var output = new List<AtlasOutputTexture>();

        var mapConfigs = new List<MapConfig>
        {
            new() { Name = FileNames.OutputHeight, MapSelector = e => e.HeightMap! },
            new() { Name = FileNames.OutputNormal, MapSelector = e => e.NormalMap! },
            new() { Name = FileNames.OutputRoughness, MapSelector = e => e.RoughnessMap! },
            new() { Name = FileNames.OutputMetallic, MapSelector = e => e.MetallicMap! },
            new() { Name = FileNames.OutputAlpha, MapSelector = e => e.AlphaMap! },
            new() { Name = FileNames.OutputAmbientOcclusion, MapSelector = e => e.AmbientOcclusionMap! }
        };

        foreach (var mapConfig in mapConfigs)
        {
            var images = config.Images.Where(i => !string.IsNullOrEmpty(mapConfig.MapSelector(i))).ToList();
            var result = BuildMap(metadata, folderPath, images, mapConfig.MapSelector);
            if (result.success)
            {
                output.Add(new AtlasOutputTexture { Image = result.image!, Name = mapConfig.Name });
            }
        }

        return output;
    }

    private static (bool success, Image<Rgba32>? image) BuildMap(AtlasMetadata metadata, string folderPath,
        List<ImageEntry> imageEntries, Func<ImageEntry, string> textureMapSelector)
    {
        if (imageEntries.Count == 0)
            return (false, null);

        var atlas = new Image<Rgba32>(metadata.Size, metadata.Size);

        foreach (var imageEntry in imageEntries)
        {
            var metaData = metadata.Textures.First(i => i.Albedo == imageEntry.Albedo);

            var imageEntryProperty = textureMapSelector(imageEntry);
            var path = Path.Combine(folderPath, imageEntryProperty);
            var loadedImage = Image.Load<Rgba32>(path);

            loadedImage.Mutate(ctx => ctx.Resize(metaData.Width, metaData.Height));

            atlas.Mutate(ctx =>
                ctx.DrawImage(loadedImage, new Point(metaData.X, metaData.Y), 1f));
        }

        return (true, atlas);
    }
    
    private struct MapConfig
    {
        public string Name { get; init; }
        public Func<ImageEntry, string> MapSelector { get; init; }
    }
}