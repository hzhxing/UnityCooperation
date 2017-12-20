using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Editor.Tools
{
    public class AssetBundleTool
    {
        public static AssetBundleManifest Build(string outputPath, BuildTarget target,bool exportStats = false)
        {
            outputPath = FileOperateUtil.GetRegDirectory(outputPath);
            FileOperateUtil.CreateDirectory(outputPath);

            AssetBundleManifest manifest = BuildPipeline.BuildAssetBundles(outputPath,
               BuildAssetBundleOptions.DeterministicAssetBundle | BuildAssetBundleOptions.ChunkBasedCompression, target);

            string[] files = Directory.GetFiles(outputPath, "*.*", SearchOption.AllDirectories);
            for (int i = 0; i < files.Length; i++)
            {
                string ext = Path.GetExtension(files[i]);
                if (String.IsNullOrEmpty(ext))
                {
                    string targetFile = files[i] + ".ab";
                    if (File.Exists(targetFile))
                    {
                        File.Delete(targetFile);
                    }
                    File.Move(files[i],targetFile);
                }
            }
            
            if (exportStats)
            {
                ExportStats(outputPath,manifest);
            }

            AssetDatabase.Refresh();
            return manifest;
        }

        private static void ExportStats(string outputPath, AssetBundleManifest manifest)
        {
            ProgressBarUtil.Title = "assetBundle资源统计";
            ProgressBarUtil.Content = "assetBundle资源统计...";
            ProgressBarUtil.Percent = 0;
            ProgressBarUtil.Show();

            string str = "文件\t文件大小（KB）\t依赖文件数\t依赖文件大小（KB）\t总大小（KB）\n";
            string format = "{0}\t{1}\t{2}\t{3}\t{4}\n";
            string pathFomat = "{0}/{1}.ab";

            long totalSize = 0;
            Dictionary<string, long> map = new Dictionary<string, long>();
            string[] assetBundles = manifest.GetAllAssetBundles();
            for (int i = 0; i < assetBundles.Length; i++)
            {
                string curAssetBundle = assetBundles[i];
                long assetBundleSize = 0;
                if (map.ContainsKey(curAssetBundle))
                {
                    assetBundleSize = map[curAssetBundle];
                }
                else
                {
                    assetBundleSize = File.ReadAllBytes(String.Format(pathFomat,outputPath,curAssetBundle)).Length;
                    map.Add(curAssetBundle,assetBundleSize);
                }

                string[] depends = manifest.GetAllDependencies(curAssetBundle);
                int dependFileCount = depends.Length;
                long dependSize = 0;
                for (int j = 0; j < depends.Length; j++)
                {
                    string curDepend = depends[j];
                    if (map.ContainsKey(curDepend))
                    {
                        dependSize += map[curDepend];
                    }
                    else
                    {
                        long size = File.ReadAllBytes(String.Format(pathFomat,outputPath,curDepend)).Length;
                        map.Add(curDepend,size);
                        dependSize += size;
                    }
                }

                totalSize += assetBundleSize;
                str += String.Format(format,curAssetBundle,assetBundleSize / 1024,dependFileCount,dependSize / 1024, (assetBundleSize + dependSize)/1024);

                ProgressBarUtil.Percent = (float) i/(float) assetBundles.Length;
            }

            str += "totalSize\t" + (totalSize / 1024) + "KB";
            File.WriteAllText(outputPath + "/stats.txt",str);

            ProgressBarUtil.Close();
        }

        public static void BuildDepend()
        {
            ClearDepend();
            ABResDepend.BuildDepend();
        }

        public static void ClearDepend()
        {
            ProgressBarUtil.Title = "依赖设置";
            ProgressBarUtil.Content = "正在清除AssetBundle...";
            ProgressBarUtil.Percent = 0;
            ProgressBarUtil.Show();

            string[] abNames = AssetDatabase.GetAllAssetBundleNames();
            for (int i = 0; i < abNames.Length; i++)
            {
                ProgressBarUtil.Percent = (float)i / (float)abNames.Length;
                string curABName = abNames[i];

                bool needClear = false;
                for (int j = 0; j < ABResUtil.PRE_LIST.Length; j++)
                {
                    if (curABName.StartsWith(ABResUtil.PRE_LIST[j]))
                    {
                        needClear = true;
                        break;
                    }
                }

                if (needClear)
                {
                    AssetDatabase.RemoveAssetBundleName(curABName, true);
                }
            }

            ProgressBarUtil.Close();

            AssetDatabase.Refresh();
        }
    }
}