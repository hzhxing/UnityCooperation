using System;
using System.IO;
using Editor.Utils;
using UnityEditor;

namespace Editor.Tools
{
    public class ABResUtil
    {
        public static string PRE_MAT = "common/mat/";
        public static string PRE_PREFAB = "common/prefab/";
        public static string PRE_TEXTURE = "common/texture/";
        public static string PRE_ATLAS = "common/atlas/";
        public static string PRE_ANIM = "common/anim/";
        public static string PRE_FBX = "common/fbx/";
        public static string PRE_EXT = "common/ext/";

        public static string[] PRE_LIST = new[]
        {PRE_MAT, PRE_PREFAB, PRE_TEXTURE, PRE_ATLAS, PRE_ANIM, PRE_FBX, PRE_EXT};

        public static ABResEnum GetType(string path)
        {
            return GetType(path,null);
        }

        public static ABResEnum GetType(string path,AssetImporter importer)
        {
            string ext = Path.GetExtension(path).ToLower();
            if (ext == ".mat")
            {
                return ABResEnum.material;
            }
            else if (ext == ".prefab")
            {
                return ABResEnum.prefab;
            }
            else if (ext == ".anim")
            {
                return ABResEnum.animation;
            }
            else if (ext == ".fbx")
            {
                return ABResEnum.model;
            }
            else if (ext == ".png" || ext == ".jpg" || ext == ".tga")
            {
                if (importer == null)
                {
                    importer = AssetImporter.GetAtPath(path);
                }
                TextureImporter textureImporter = importer as TextureImporter;
                if (String.IsNullOrEmpty(textureImporter.spritePackingTag))
                {
                    return ABResEnum.texture;
                }
                else
                {
                    return ABResEnum.atlas;
                }

            }
            else
            {
                return ABResEnum.extension;
            }
        }

        public static string GetABName(string path)
        {
            return GetABName(path, GetType(path), null);
        }

        public static string GetABName(string path,ABResEnum type,AssetImporter importer)
        {
            string pre = GetABPre(type);
            if (type == ABResEnum.atlas)
            {
                if (importer == null)
                {
                    importer = AssetImporter.GetAtPath(path);
                }
                TextureImporter textureImporter = importer as TextureImporter;
                return pre + textureImporter.spritePackingTag;
            }
            else if (type == ABResEnum.texture)
            {
                string fileName = Path.GetFileNameWithoutExtension(path);
                string[] splits = fileName.Split('_');
                if (splits.Length < 2)
                {
                    string md5 = MD5Util.Sum(path).Substring(0, 8).ToLower();
                    return pre + md5;
                }
                else
                {
                    return pre + MD5Util.Sum(splits[0] + "_" + splits[1]).Substring(0, 8).ToLower();
                }
            }
            else
            {
                string md5 = MD5Util.Sum(path).Substring(0, 8).ToLower();
                return pre + md5;
            }
        }

        public static string GetABPre(ABResEnum type)
        {
            switch (type)
            {
                case ABResEnum.prefab:
                    return PRE_PREFAB;
                case ABResEnum.material:
                    return PRE_MAT;
                case ABResEnum.model:
                    return PRE_FBX;
                case ABResEnum.animation:
                    return PRE_ANIM;
                case ABResEnum.texture:
                    return PRE_TEXTURE;
                case ABResEnum.atlas:
                    return PRE_ATLAS;
                case ABResEnum.extension:
                    return PRE_EXT;
            }
            throw new Exception("无法处理的ABResEnum=" + type);
        }
    }
}