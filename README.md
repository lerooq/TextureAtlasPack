# Usage

`AtlasPack.exe <path-to-folder>`

Textures will be scaled by width, respecting the aspect ratio

## What is produced
produces pngs: atlas albedo, normals, etc.

Also produces atlas_metadata.json showing texture position and size

## Example of atlas.json
```json
{
  "padding": 1,
  "defaultWidth": 64,
  "images": [
    { "albedo": "IMG_20220528_200253-output.png", "width": 128 },
    { "albedo": "IMG_20220528_200259-output.png", "width": 128 },
    { "albedo": "IMG_20220721_180512-output.png", "alphaMap": "IMG_20220721_180512-output_alpha.png", "width": 64 },
    { "albedo": "PXL_20250728_114904614.png", "width": 128 },
    { "albedo": "TCom_GutterCover_512_albedo.tif", "alphaMap": "TCom_GutterCover_512_alpha.tif", "normalMap": "TCom_GutterCover_1K_normal.tif", "width": 256 }
  ]
}
```
This will produce one albedo texture, one alpha texture and one normal texture.
![Texture files produced by the sample atlas.json](alpha_textures_output.png)