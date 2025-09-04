using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace AtlasPack.Models;

public class AtlasOutputTexture
{
    public string Name { get; set; }
    public Image<Rgba32> Image { get; set; }
}