namespace AtlasPack.Models;

public class AtlasMetadata
{
    public int Width { get; set; }
    public int Height { get; set; }
    public List<ImageMetadata> Images { get; set; } = new();
}