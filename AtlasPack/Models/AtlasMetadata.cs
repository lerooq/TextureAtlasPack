namespace AtlasPack.Models;

public class AtlasMetadata
{
    public int Size { get; set; }
    public List<TextureMetadata> Textures { get; set; } = [];
}