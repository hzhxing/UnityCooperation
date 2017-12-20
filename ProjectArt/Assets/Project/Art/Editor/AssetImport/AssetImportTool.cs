using System;
using UnityEditor;
using UnityEngine;

namespace Editor.AssetImport
{
    public class AssetImportTool:AssetPostprocessor
    {
        void OnPreprocessModel()
        {
            ModelImportItem item = EditorConfig.GetModelImportItem(assetPath);
            if (item != null)
            {
                ModelImporter modelImporter = assetImporter as ModelImporter;
                modelImporter.importMaterials = item.importMaterials;
                modelImporter.meshCompression = item.meshCompression;
                modelImporter.optimizeMesh = item.optimizeMesh;
                modelImporter.isReadable = item.isReadable;
                modelImporter.importBlendShapes = item.importBlendShapes;
                modelImporter.importTangents = item.importTangents;
                modelImporter.importNormals = item.importNormals;
            }
        }

        void OnPreprocessTexture()
        {
            TextureImportItem item = EditorConfig.GetTextureImportItem(assetPath);
            if (item != null)
            {
                TextureImporter textureImporter = assetImporter as TextureImporter;
                textureImporter.textureType = item.textureType;
                textureImporter.mipmapEnabled = item.mipmapEnabled;
                if (item.textureType == TextureImporterType.Sprite)
                {
                    if (!string.IsNullOrEmpty(item.spritePackingTag))
                    {
                        textureImporter.spritePackingTag = item.spritePackingTag;
                    }
                    
                    textureImporter.spriteImportMode = SpriteImportMode.Single;
                }
                
                textureImporter.isReadable = item.isReadable;

                TextureImporterPlatformSettings androidSettings = new TextureImporterPlatformSettings();
                androidSettings.name = "Android";
                androidSettings.overridden = true;
                androidSettings.maxTextureSize = item.androidMaxSize;
                androidSettings.format = item.androidFormat;
                androidSettings.allowsAlphaSplitting = item.androidAlphaSplit;
                textureImporter.SetPlatformTextureSettings(androidSettings);
                
                TextureImporterPlatformSettings iosSettings = new TextureImporterPlatformSettings();
                iosSettings.name = "iPhone";
                iosSettings.overridden = true;
                iosSettings.maxTextureSize = item.iosMaxSize;
                iosSettings.format = item.iosFormat;
                textureImporter.SetPlatformTextureSettings(iosSettings);
            }

        }
        
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            for (int i = 0; i < importedAssets.Length; i++)
            {
                string path = importedAssets[i];
                string abName;
                Debug.Log("importAsset:" + path);
                if (EditorConfig.TryGetAssetBundleName(path,out abName))
                {
                    AssetImporter importer =  AssetImporter.GetAtPath(path);
                    if (!importer.assetBundleName.Equals(abName))
                    {
                        importer.assetBundleName = abName;
                        Debug.LogFormat("path={0} abName={1}",path,abName);
                    }
                }
            }
            
            for (int i = 0; i < movedAssets.Length; i++)
            {
                string moveFromPath = movedFromAssetPaths[i];
                string moveToPath = movedAssets[i];
                string abName;
                Debug.LogFormat("movedAsset {0} => {1}",moveFromPath,moveToPath);
                if (EditorConfig.TryGetAssetBundleName(moveToPath,out abName))
                {
                    AssetImporter importer =  AssetImporter.GetAtPath(moveToPath);
                    if (!importer.assetBundleName.EndsWith(abName))
                    {
                        importer.assetBundleName = abName;
                        Debug.LogFormat("path={0} abName={1}",moveToPath,abName);
                    }
                }
                else
                {
                    if (EditorConfig.TryGetAssetBundleName(moveFromPath,out abName))
                    {
                        AssetImporter importer =  AssetImporter.GetAtPath(moveToPath);
                        importer.assetBundleName = null;
                        Debug.LogFormat("path={0} set assetBundleName to null",moveToPath);
                    }
                }
            }
        }
    }
}