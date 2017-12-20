using System.Text.RegularExpressions;
using UnityEditor;

namespace Editor
{
    public class EditorConfigItem
    {
        public string regPath;
        
        private Regex mRegex;
        
        public EditorConfigItem(string regPath,RegexOptions options)
        {
            this.regPath = regPath;
            mRegex = new Regex(this.regPath,options);
        }

        public Match MatchPath(string path)
        {
            return mRegex.Match(path);
        }
    }
    
    public class AssetBundleItem:EditorConfigItem
    {
        public string assetBundleName;
        
        public AssetBundleItem(string regPath, string assetBundleName):base(regPath,RegexOptions.IgnoreCase)
        {
            this.assetBundleName = assetBundleName;
        }

        public bool TryGetABName(string path, out string abName)
        {
            abName = null;
            var match = MatchPath(path);
            if (!match.Success)
            {
                return false;
            }

            var groups = match.Groups;
            if (groups.Count == 1)
            {
                abName = assetBundleName;
            }
            else if (groups.Count == 2)
            {
                abName = string.Format(assetBundleName, groups[1].Value);
            }
            else if (groups.Count == 3)
            {
                abName = string.Format(assetBundleName, groups[1].Value,groups[2].Value);
            }
            else if (groups.Count == 4)
            {
                abName = string.Format(assetBundleName, groups[1].Value,groups[2].Value,groups[3].Value);
            }
            return true;
        }
    }

    public class ModelImportItem : EditorConfigItem
    {
        public bool importMaterials;
        public ModelImporterMeshCompression meshCompression;
        public bool optimizeMesh;
        public bool isReadable;
        public bool importBlendShapes;
        public ModelImporterTangents importTangents;
        public ModelImporterNormals importNormals;
        
        public ModelImportItem(string regPath) : base(regPath,RegexOptions.IgnoreCase)
        {
        }
    }
    
    public class TextureImportItem:EditorConfigItem
    {
        public TextureImporterType textureType;
        public bool mipmapEnabled;
        public SpriteImportMode spriteImportMode;
        public string spritePackingTag;
        public bool isReadable;

        public TextureImporterFormat androidFormat;
        public int androidQuality;
        public int androidMaxSize;
        public bool androidAlphaSplit;

        public TextureImporterFormat iosFormat;
        public int iosQuality;
        public int iosMaxSize;

        public TextureImportItem(string regPath) : base(regPath,RegexOptions.IgnoreCase)
        {
        }
    }
}