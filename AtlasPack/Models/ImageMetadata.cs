namespace AtlasPack.Models;

public class ImageMetadata
{
    public string Albedo { get; set; }
    public string? HeightMap { get; set; }
    public string? NormalMap { get; set; }
    public string? RoughnessMap { get; set; }
    public string? MetallicMap { get; set; }
    public string? AlphaMap { get; set; }
    public string? AmbientOcclusionMap { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
}