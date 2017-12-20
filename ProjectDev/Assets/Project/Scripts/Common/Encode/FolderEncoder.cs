using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace Common.Encode
{
    public class FolderEncoder
    {
        private string _srcPath;
        private string _destFile;

        private string[] _fileList;

        public FolderEncoder(string srcPath,string destFile,string searchPattern,params string[] exts)
        {
            this._srcPath = srcPath;
            this._destFile = destFile;
            CheckDestFile();

            string[] list = Directory.GetFiles(srcPath, searchPattern, SearchOption.AllDirectories);
            List<string> tmpList = new List<string>();
            for (int i = 0; i < list.Length; i++)
            {
                string curFile = list[i];
                bool accept = true;
                for (int j = 0; j < exts.Length; j++)
                {
                    if (curFile.EndsWith(exts[j]))
                    {
                        accept = false;
                        break;
                    }
                }
                if (accept)
                {
                    tmpList.Add(curFile);
                }
            }

            this._fileList = tmpList.ToArray();
        }

        private void CheckDestFile()
        {
            string dir = Path.GetDirectoryName(this._destFile);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            if (File.Exists(this._destFile))
            {
                File.Delete(this._destFile);
            }
        }

        public long Encode(IProgress progress)
        {
            long totalSize = 0;

            const int count = 10240;
            byte[] tmpBytes = new byte[count];

            FileStream stream = new FileStream(this._destFile, FileMode.Create);
            stream.Write(BitConverter.GetBytes(this._fileList.Length), 0, 4);
            totalSize += 4;
            for (int i = 0; i < this._fileList.Length; i++)
            {
                string curfile = this._fileList[i].Replace("\\","/");
                string relativePath = curfile.Substring(this._srcPath.Length + 1);

                FileStream fileStream = new FileStream(curfile, FileMode.Open);
                byte[] pathBytes = Encoding.UTF8.GetBytes(relativePath);
                stream.WriteByte((byte)pathBytes.Length);
                stream.Write(pathBytes, 0, pathBytes.Length);
                stream.Write(BitConverter.GetBytes(fileStream.Length), 0, 8);

                totalSize += 1 + pathBytes.Length + 8;

                while (true)
                {
                    int size = fileStream.Read(tmpBytes, 0, count);
                    if (size <= 0)
                    {
                        break;
                    }
                    stream.Write(tmpBytes, 0, size);
                    totalSize += size;

                }
                fileStream.Close();

                if (progress != null)
                {
                    progress.SetPercent((float)i / (float)this._fileList.Length);
                }
            }
            stream.Flush();
            stream.Close();

            return totalSize;
        }
    }
}