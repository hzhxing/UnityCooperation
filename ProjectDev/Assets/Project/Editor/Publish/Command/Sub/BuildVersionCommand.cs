using System.IO;
using Common.Command;
using Common.Version;
using Editor.Tools;
using UnityEngine;

namespace Editor.Publish
{
    public class BuildVersionCommand:Command
    {
        public override void Execute(object content)
        {
            base.Execute(content);

            PublishContent publishContent = _content as PublishContent;

            string versionFile = publishContent.GetVersionPath() + "/version.xml";
            string cacheVersionFile = publishContent.GetRootPath() + "/version.xml";
            FileOperateUtil.CreateFileDirectory(versionFile);

            if (publishContent.bigVersion)
            {
                VersionContent versionContent = new VersionContent();
                versionContent.version = publishContent.version;
                versionContent.resUrl = publishContent.resUrl;
                versionContent.Save(versionFile);

                string projectVersionFile = FileOperateUtil.GetRegPath(Application.dataPath + "/Resources/version.bytes");
                File.Copy(versionFile,projectVersionFile,true);
            }
            else
            {
                VersionContent versionContent = new VersionContent();
                if (File.Exists(cacheVersionFile))
                {
                    versionContent.Parse(File.ReadAllText(cacheVersionFile));
                }
                versionContent.version = publishContent.version;
                versionContent.resUrl = publishContent.resUrl;

                ResVersion resVersion;
                resVersion.version = publishContent.resVersion;
                resVersion.url = publishContent.updateFile;
                resVersion.md5 = publishContent.updateFileMD5;
                resVersion.size = publishContent.updateFileSize;

                versionContent.resVersions.Add(resVersion);
                versionContent.Save(versionFile);
            }

            File.Copy(versionFile,cacheVersionFile,true);

            Success(publishContent);
        }
    }
}