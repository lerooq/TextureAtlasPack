namespace AtlasPack.Models;

public interface ITextureMaps
{
    public string Albedo { get; set; }
    public string? HeightMap { get; set; }
    public string? NormalMap { get; set; }
    public string? RoughnessMap { get; set; }
    public string? MetallicMap { get; set; }
    public string? AlphaMap { get; set; }
    public string? AmbientOcclusionMap { get; set; }
}