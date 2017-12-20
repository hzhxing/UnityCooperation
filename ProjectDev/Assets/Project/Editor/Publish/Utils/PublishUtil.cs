using System.IO;
using Editor.Tools;

namespace Editor.Publish
{
    public interface IProgress
    {
        void SetPercent(float percent);
    }

    public class PublishUtil
    {
        public static void Compare(string srcPath, string destPath, string toPath, string searchPattern,bool replaceDest,IProgress progress = null)
        {
            string[] files = Directory.GetFiles(srcPath, searchPattern, SearchOption.AllDirectories);
            for (int i = 0; i < files.Length; i++)
            {
                string curFile = files[i];
                string relativeFile = curFile.Substring(srcPath.Length);
                string targetFile = destPath + relativeFile;
                if (!FileOperateUtil.IsFileEqual(curFile,targetFile))
                {
                    if (replaceDest)
                    {
                        FileOperateUtil.CreateFileDirectory(targetFile);
                        File.Copy(curFile, targetFile, true);
                    }

                    string toFile = toPath + relativeFile;
                    FileOperateUtil.CreateFileDirectory(toFile);
                    File.Copy(curFile,toFile,true);
                }

                if (progress != null)
                {
                    progress.SetPercent((float)i/(float)files.Length);
                }
            }
        }

        public static void Copy(string srcPath, string destPath, string searchPattern, IProgress progress = null)
        {
            if (!Directory.Exists(srcPath))
            {
                if (progress != null)
                {
                    progress.SetPercent(1);
                }
                return;
            }

            string[] files = Directory.GetFiles(srcPath, searchPattern, SearchOption.AllDirectories);
            for (int i = 0; i < files.Length; i++)
            {
                string curFile = files[i];
                string relativeFile = curFile.Substring(srcPath.Length);
                string destFile = destPath + relativeFile;
                FileOperateUtil.CreateFileDirectory(destFile);

                File.Copy(curFile,destFile,true);

                if (progress != null)
                {
                    progress.SetPercent((float)i / (float)files.Length);
                }
            }
        }
    }
}