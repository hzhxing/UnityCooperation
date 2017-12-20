using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Editor.Tools
{
    public class FileOperateUtil
    {
        public static string GetRegPath(string path)
        {
            return path.Replace("\\", "/");
        }

        public static string GetRegDirectory(string directory)
        {
            directory = GetRegPath(directory);
            if (directory.EndsWith("/"))
            {
                return directory.Substring(0, directory.Length - 1);
            }
            return directory;
        }

        public static string GetPath(string assetPath)
        {
            string dataPath = Application.dataPath;
            return GetRegPath(dataPath.Substring(0,dataPath.Length - 6) + assetPath);
        }

        public static string GetAssetPath(string filePath)
        {
            return GetRegPath(filePath.Substring(Application.dataPath.Length - 6));
        }

        public static bool IsMetaFile(string file)
        {
            return file.EndsWith(".meta");
        }

        public static bool IsManifestFile(string file)
        {
            return file.EndsWith(".manifest");
        }

        public static string GetMetaFile(string file)
        {
            return file + ".meta";
        }

        public static string GetManifestFile(string file)
        {
            return file + ".manifest";
        }

        public static string[] GetAssets(string assetPath, string searchPattern)
        {
            return GetAssets(assetPath, searchPattern, SearchOption.AllDirectories);
        }

        public static string[] GetAssets(string assetPath,string searchPattern, SearchOption searchOption)
        {
            List<string> assets = new List<string>();

            string[] list =  Directory.GetFiles(GetPath(assetPath), searchPattern, searchOption);
            for (int i = 0; i < list.Length; i++)
            {
                string curFile = list[i];
                if (IsMetaFile(curFile) || IsManifestFile(curFile))
                {
                    continue;
                }
                assets.Add(GetAssetPath(curFile));
            }

            return assets.ToArray();
        }

        public static bool IsFileEqual(string file1, string file2)
        {
            if (!File.Exists(file1) || !File.Exists(file2))
            {
                return false;
            }

            byte[] bytes1 = File.ReadAllBytes(file1);
            byte[] bytes2 = File.ReadAllBytes(file2);
            return IsBinaryEqual(bytes1, bytes2);
        }


        public static bool IsBinaryEqual(byte[] bytes1, byte[] bytes2)
        {
            if (bytes1.Length != bytes2.Length)
            {
                return false;
            }
            else
            {
                for (int i = 0; i < bytes1.Length; i++)
                {
                    if (bytes1[i] != bytes2[i])
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        public static void CopyDirectory(string srcPath, string destPath,string searchPattern)
        {
            string[] files = Directory.GetFiles(srcPath, searchPattern, SearchOption.AllDirectories);
            for (int i = 0; i < files.Length; i++)
            {
                string curFile = GetRegPath(files[i]);
                string relativePath = curFile.Substring(srcPath.Length);
                string copyTo = destPath + relativePath;
                CreateDirectory(copyTo.Substring(0, copyTo.LastIndexOf("/")));

                File.Copy(curFile, copyTo, true);
            }
        }

        public static void CreateDirectory(string dir,bool clear = false)
        {
            if (Directory.Exists(dir))
            {
                if (!clear)
                {
                    return;
                }
                Directory.Delete(dir,true);
            }
            Directory.CreateDirectory(dir);
        }

        public static void ClearDirectory(string dir)
        {
            CreateDirectory(dir,true);
        }

        public static void CreateFileDirectory(string file)
        {
            file = GetRegPath(file);
            CreateDirectory(file.Substring(0,file.LastIndexOf("/")));
        }
    }
}