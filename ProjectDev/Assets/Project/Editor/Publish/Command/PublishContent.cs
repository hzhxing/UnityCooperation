using System;
using System.IO;
using Editor.Tools;
using UnityEditor;
using UnityEngine;

namespace Editor.Publish
{
    public class PublishContent
    {
        public static string DEFINE_PUBLISH_PATH = FileOperateUtil.GetRegPath(Path.GetFullPath(Application.dataPath + "/../Publish"));


        public bool referenceABSetting = false;
        public RuntimePlatform platform = RuntimePlatform.Android;

        public string publishPath = DEFINE_PUBLISH_PATH;
        public string resUrl = String.Empty;
        
        public string version = String.Empty;
        public string resVersion = String.Empty;
        public bool bigVersion = false;

        public string updateFile = String.Empty;
        public string updateFileMD5 = String.Empty;
        public long updateFileSize = 0;

        public PublishContent(RuntimePlatform platform)
        {
            this.platform = platform;
            referenceABSetting = false;
        }

        public string GetRootPath()
        {
            return GetPlatformPath(publishPath, platform);
        }

        public string GetVersionPath()
        {
            string path = GetRootPath();
            return path + "/v" + resVersion;
        }

        public string GetUpdateFilePath()
        {
            return GetVersionPath() + "/tmp";
        }

        public BuildTarget GetBuildTarget()
        {
            if (platform == RuntimePlatform.Android)
            {
                return BuildTarget.Android;
            }
            else if (platform == RuntimePlatform.IPhonePlayer)
            {
                return  BuildTarget.iOS;
            }

            throw new Exception("不合法的RuntimePlatform!!!");
        }
        

        public static string GetPlatformPath(string path, RuntimePlatform platform)
        {
            if (platform == RuntimePlatform.Android)
            {
                return FileOperateUtil.GetRegPath(path + "/Android");
            }
            else if (platform == RuntimePlatform.IPhonePlayer)
            {
                return FileOperateUtil.GetRegPath(path + "/IOS");
            }

            Debug.LogError("不支持的platform");
            return String.Empty;
        }

        public static string GetLuaPath(string path)
        {
            return FileOperateUtil.GetRegPath(path + "/Lua");
        }

        public static string GetResPath(string path)
        {
            return FileOperateUtil.GetRegPath(path + "/Res");
        }

        public static string GetABPath(string path)
        {
            return FileOperateUtil.GetRegPath(path + "/AssetBundles");
        }
    }
}