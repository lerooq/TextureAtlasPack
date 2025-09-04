using AtlasPack.Models;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace AtlasPack;

public static class AtlasBuilder
{
    public static (Image<Rgba32> atlas, AtlasMetadata metadata) BuildAtlas(string folderPath, AtlasConfig config)
    {
        var rects = new List<PackedRect>();

        foreach (var entry in config.Images)
        {
            var path = Path.Combine(folderPath, entry.File);
            using var loadedImage = Image.Load<Rgba32>(path);
            var targetWidth = entry.Width ?? config.DefaultWidth;
            var aspectRatio = (float)loadedImage.Height / loadedImage.Width;
            var targetHeight = (int)(targetWidth * aspectRatio);
            loadedImage.Mutate(ctx => ctx.Resize(targetWidth, targetHeight));
            var effWidth = targetWidth + 2 * config.Padding;
            var effHeight = targetHeight + 2 * config.Padding;
            rects.Add(new PackedRect
            {
                EffWidth = effWidth,
                EffHeight = effHeight,
                File = entry.File,
                Image = loadedImage.Clone()
            });
        }

        if (rects.Count == 0)
        {
            return (new Image<Rgba32>(1, 1), new AtlasMetadata { Images = new List<ImageMetadata>() });
        }

        var maxWidth = rects.Max(r => r.EffWidth);
        var maxHeight = rects.Max(r => r.EffHeight);
        var totalArea = rects.Sum(r => (double)r.EffWidth * r.EffHeight);
        var minPossibleSide = (int)Math.Max(Math.Max(maxWidth, maxHeight), Math.Ceiling(Math.Sqrt(totalArea)));

        var sortedRects = rects.OrderByDescending(r => r.EffHeight).ToList();

        var optimalSide = minPossibleSide;
        while (!CanPack(sortedRects, optimalSide))
        {
            optimalSide *= 2;
        }

        var upperBound = optimalSide;
        var lowerBound = minPossibleSide;
        while (lowerBound < upperBound)
        {
            var mid = lowerBound + (upperBound - lowerBound) / 2;
            if (CanPack(sortedRects, mid))
            {
                upperBound = mid;
            }
            else
            {
                lowerBound = mid + 1;
            }
        }

        optimalSide = lowerBound;

        var atlasSide = 1;
        while (atlasSide < optimalSide)
        {
            atlasSide *= 2;
        }

        Pack(sortedRects, atlasSide);

        var atlas = new Image<Rgba32>(atlasSide, atlasSide);
        foreach (var packed in sortedRects)
        {
            atlas.Mutate(ctx =>
                ctx.DrawImage(packed.Image, new Point(packed.X + config.Padding, packed.Y + config.Padding), 1f));
        }

        var metadata = new AtlasMetadata { Images = new List<ImageMetadata>() };
        foreach (var packed in sortedRects)
        {
            metadata.Images.Add(new ImageMetadata
            {
                File = packed.File,
                X = packed.X + config.Padding,
                Y = packed.Y + config.Padding,
                Width = packed.Image.Width,
                Height = packed.Image.Height
            });
        }

        foreach (var packed in rects)
        {
            packed.Image.Dispose();
        }

        return (atlas, metadata);
    }

    private static bool CanPack(List<PackedRect> sortedRects, int side)
    {
        var shelves = new List<Shelf>();
        foreach (var rect in sortedRects)
        {
            var placed = false;
            foreach (var shelf in shelves)
            {
                if (shelf.CurrentX + rect.EffWidth <= side)
                {
                    shelf.CurrentX += rect.EffWidth;
                    placed = true;
                    break;
                }
            }

            if (placed)
                continue;

            var newY = shelves.Count > 0 ? shelves[^1].Y + shelves[^1].Height : 0;
            if (newY + rect.EffHeight > side)
            {
                return false;
            }

            shelves.Add(new Shelf { Y = newY, Height = rect.EffHeight, CurrentX = rect.EffWidth });
        }

        return true;
    }

    private static void Pack(List<PackedRect> sortedRects, int side)
    {
        var shelves = new List<Shelf>();
        foreach (var rect in sortedRects)
        {
            var placed = false;
            foreach (var shelf in shelves)
            {
                if (shelf.CurrentX + rect.EffWidth <= side)
                {
                    rect.X = shelf.CurrentX;
                    rect.Y = shelf.Y;
                    shelf.CurrentX += rect.EffWidth;
                    placed = true;
                    break;
                }
            }

            if (!placed)
            {
                var newY = shelves.Count > 0 ? shelves[^1].Y + shelves[^1].Height : 0;
                rect.X = 0;
                rect.Y = newY;
                shelves.Add(new Shelf { Y = newY, Height = rect.EffHeight, CurrentX = rect.EffWidth });
            }
        }
    }

    private class Shelf
    {
        public int Y { get; set; }
        public int Height { get; set; }
        public int CurrentX { get; set; }
    }

    private class PackedRect
    {
        public int EffWidth { get; set; }
        public int EffHeight { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public string File { get; set; } = null!;
        public Image<Rgba32> Image { get; set; } = null!;
    }
}