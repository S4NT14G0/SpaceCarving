using UnityEngine;
using UnityEditor;
using System.Collections;

public class ProceduralTextureImporter : AssetPostprocessor
{

    private void OnPreprocessTexture()
    {
        // Get accesss to the asset importer
        var importer = assetImporter as TextureImporter;

        importer.textureType = TextureImporterType.Default;

        importer.alphaSource = TextureImporterAlphaSource.None;

        importer.mipmapEnabled = false;
        importer.textureCompression = TextureImporterCompression.Uncompressed;
        importer.textureFormat = TextureImporterFormat.RGBA32;



        importer.isReadable = true;
    }
}