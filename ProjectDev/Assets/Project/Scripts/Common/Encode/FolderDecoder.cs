using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace Common.Encode
{
    public class FolderDecoder
    {
        private Stream _stream;
        private string _destPath;

        public FolderDecoder(string file,string destPath)
        {
            if (!File.Exists(file))
            {
                Debug.LogError("file:" + file + " is not exist!");
                return;
            }
            this._stream = new FileStream(file,FileMode.Open);
            this._destPath = destPath;
        }

        public FolderDecoder(Stream stream, string destPath)
        {
            _stream = stream;
            _destPath = destPath;
        }

        public void Decode(IProgress progress)
        {
            const int count = 10240;
            byte[] tmpBytes = new byte[count];

            byte[] fileNumBytes = new byte[4];
            this._stream.Read(fileNumBytes, 0, 4);
            int fileNum = BitConverter.ToInt32(fileNumBytes, 0);

            for (int i = 0; i < fileNum; i++)
            {
                int outPathBytesLen = this._stream.ReadByte();
                byte[] outPathBytes = new byte[outPathBytesLen];
                this._stream.Read(outPathBytes, 0, outPathBytesLen);
                string outPath = Encoding.UTF8.GetString(outPathBytes);

                byte[] leftBytesLenBytes = new byte[8];
                this._stream.Read(leftBytesLenBytes, 0, 8);
                long leftBytesLen = BitConverter.ToInt64(leftBytesLenBytes, 0);

                string filePath = this._destPath + "/" + outPath;
                string dir = Path.GetDirectoryName(filePath);
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }

                FileStream outputStream = new FileStream(filePath, FileMode.Create);
                while (true)
                {
                    int readLen = leftBytesLen <= count ? (int)leftBytesLen : count;
                    this._stream.Read(tmpBytes, 0, (int)readLen);
                    outputStream.Write(tmpBytes, 0, (int)readLen);

                    leftBytesLen = leftBytesLen - readLen;
                    if (leftBytesLen <= 0)
                    {
                        break;
                    }
                }
                outputStream.Flush();
                outputStream.Close();

                if (progress != null)
                {
                    progress.SetPercent((float)i / (float)fileNum);
                }
            }
            if (this._stream.Position != this._stream.Length)
            {
                Debug.Log("?????????????????????????:" + (this._stream.Length - this._stream.Position));
            }
            this._stream.Close();
        }

        public static Dictionary<string, byte[]> DecodeBytes(byte[] bytes)
        {
            Dictionary<string,byte[]> maping = new Dictionary<string, byte[]>();
            MemoryStream stream = new MemoryStream(bytes);

            byte[] fileNumBytes = new byte[4];
            stream.Read(fileNumBytes, 0, 4);
            int fileNum = BitConverter.ToInt32(fileNumBytes, 0);

            for (int i = 0; i < fileNum; i++)
            {
                int filePathBytesLen = stream.ReadByte();
                byte[] filePathBytes = new byte[filePathBytesLen];
                stream.Read(filePathBytes, 0, filePathBytesLen);
                string filePath = Encoding.UTF8.GetString(filePathBytes);

                byte[] fileByteslenBytes = new byte[8];
                stream.Read(fileByteslenBytes, 0, 8);
                long fileBytesLen = BitConverter.ToInt64(fileByteslenBytes, 0);
                byte[] fileBytes = new byte[fileBytesLen];
                stream.Read(fileBytes, 0, (int)fileBytesLen);

                maping.Add(filePath, fileBytes);
                
            }
            stream.Close();

            return maping;
        } 
    }
}