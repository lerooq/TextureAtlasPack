namespace AtlasPack.Models;

public class ImageEntry
{
    public string File { get; set; }
    public int? Width { get; set; } // optional, preserves aspect ratio
}