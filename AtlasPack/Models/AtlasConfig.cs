namespace AtlasPack.Models;

public class AtlasConfig
{
    public int Padding { get; set; }
    public int DefaultWidth { get; set; } = 64;
    public List<ImageEntry> Images { get; set; } = null!;
}