namespace AtlasPack.Models;

public class AtlasMetadata
{
    public int Size { get; set; }
    public List<ImageMetadata> Images { get; set; } = new();
}