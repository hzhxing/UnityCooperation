using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;
using Editor.Tools;
using UnityEditor;
using UnityEngine;

namespace Editor
{    
    public class EditorConfig
    {

        public static AssetBundleItem[] assetBundleItems;
        public static ModelImportItem[] modelImportItems;
        public static TextureImportItem[] textureImportItems;

        [MenuItem("拓展工具/ReloadEditorConfig", false)]
        [InitializeOnLoadMethod]
        public static void Init()
        {
            string text = File.ReadAllText(FileOperateUtil.GetPath("Assets/Project/Art/Editor/EditorConfig.xml"));
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(text);

            XmlElement root = doc.ChildNodes[0] as XmlElement;
            InitAssetBundleItem(root.SelectSingleNode("AssetBundle") as XmlElement);
            InitModelImportItem(root.SelectSingleNode("ModelImport") as XmlElement);
            InitTextureImportItem(root.SelectSingleNode("TextureImport") as XmlElement);
            
            Debug.Log("Reload Completed!!");
        }

        private static void InitAssetBundleItem(XmlElement element)
        {
            List<AssetBundleItem> items = new List<AssetBundleItem>();
            
            for (int i = 0; i < element.ChildNodes.Count; i++)
            {
                XmlElement curElement = element.ChildNodes[i] as XmlElement;
                if (curElement.Name != "Item")
                {
                    continue;
                }

                string path = curElement.GetAttribute("path");
                string abName = curElement.GetAttribute("abName");

                items.Add(new AssetBundleItem(path, abName));
            }
            assetBundleItems = items.ToArray();
        }

        private static void InitModelImportItem(XmlElement element)
        {
            List<ModelImportItem> items = new List<ModelImportItem>();

            for (int i = 0; i < element.ChildNodes.Count; i++)
            {
                XmlElement curElement = element.ChildNodes[i] as XmlElement;
                if (curElement.Name != "Item")
                {
                    continue;
                }
                
                ModelImportItem item = new ModelImportItem(curElement.GetAttribute("path"));
                item.importMaterials = curElement.GetAttribute("importMaterials").Equals("true");
                item.meshCompression = (ModelImporterMeshCompression)Enum.Parse(typeof(ModelImporterMeshCompression),curElement.GetAttribute("meshCompression"),true);
                item.optimizeMesh = curElement.GetAttribute("optimizeMesh").Equals("true");
                item.isReadable = curElement.GetAttribute("isReadable").Equals("true");
                item.importBlendShapes = curElement.GetAttribute("importBlendShapes").Equals("true");

                item.importTangents = (ModelImporterTangents)Enum.Parse(typeof(ModelImporterTangents),curElement.GetAttribute("importTangents"), true);
                item.importNormals = (ModelImporterNormals)Enum.Parse(typeof(ModelImporterNormals),curElement.GetAttribute("importNormals"), true);
                
                items.Add(item);
            }

            modelImportItems = items.ToArray();
        }

        private static void InitTextureImportItem(XmlElement element)
        {
            List<TextureImportItem> items = new List<TextureImportItem>();

            for (int i = 0; i < element.ChildNodes.Count; i++)
            {
                XmlElement curElement = element.ChildNodes[i] as XmlElement;
                if (curElement.Name != "Item")
                {
                    continue;
                }
                
                TextureImportItem item = new TextureImportItem(curElement.GetAttribute("path"));

                item.textureType = (TextureImporterType)Enum.Parse(typeof(TextureImporterType),curElement.GetAttribute("textureType"),true);
                item.mipmapEnabled = curElement.GetAttribute("mipmapEnabled").Equals("true");
                item.spriteImportMode = (SpriteImportMode) Enum.Parse(typeof(SpriteImportMode),curElement.GetAttribute("spriteImportMode"), true);
                item.spritePackingTag = curElement.GetAttribute("spritePackingTag");
                item.isReadable = curElement.GetAttribute("isReadable").Equals("true");
                item.androidFormat = (TextureImporterFormat)Enum.Parse(typeof(TextureImporterFormat),curElement.GetAttribute("androidFormat"),true);;

                item.androidQuality = Convert.ToInt32(curElement.GetAttribute("androidQuality"));
                item.androidMaxSize = Convert.ToInt32(curElement.GetAttribute("androidMaxSize"));
                item.androidAlphaSplit = curElement.GetAttribute("androidAlphaSplit").Equals("true");

                item.iosFormat = (TextureImporterFormat)Enum.Parse(typeof(TextureImporterFormat),curElement.GetAttribute("iosFormat"),true);
                item.iosQuality = Convert.ToInt32(curElement.GetAttribute("iosQuality"));
                item.iosMaxSize = Convert.ToInt32(curElement.GetAttribute("iosMaxSize"));
                
                items.Add(item);
            }

            textureImportItems = items.ToArray();
        }
        
        public static bool TryGetAssetBundleName(string path, out string abName)
        {
            if (assetBundleItems == null)
            {
                Init();
            }

            path = FileOperateUtil.GetRegPath(path);
            abName = null;
            for (int i = 0; i < assetBundleItems.Length; i++)
            {
                AssetBundleItem curItem = assetBundleItems[i];
                if (curItem.TryGetABName(path,out abName))
                {
                    abName = abName.ToLower();
                    return true;
                }
            }

            return false;
        }

        public static ModelImportItem GetModelImportItem(string path)
        {
            if (modelImportItems == null)
            {
                Init();
            }
            path = FileOperateUtil.GetRegPath(path);
            for (int i = 0; i < modelImportItems.Length; i++)
            {
                ModelImportItem item = modelImportItems[i];
                Match match = item.MatchPath(path);
                if (match.Success)
                {
                    return item;
                }
            }

            return null;
        }

        public static TextureImportItem GetTextureImportItem(string path)
        {
            if (textureImportItems == null)
            {
                Init();
            }
            path = FileOperateUtil.GetRegPath(path);
            for (int i = 0; i < textureImportItems.Length; i++)
            {
                TextureImportItem item = textureImportItems[i];
                Match match = item.MatchPath(path);
                if (match.Success)
                {
                    return item;
                }
            }

            return null;
        }
    }
}