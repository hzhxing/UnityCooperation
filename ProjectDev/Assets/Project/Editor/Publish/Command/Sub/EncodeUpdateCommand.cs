using Common.Command;
using Common.Encode;
using Editor.Tools;

namespace Editor.Publish
{
    public class EncodeUpdateCommand:Command
    {
        public override void Execute(object content)
        {
            base.Execute(content);

            PublishContent publishContent = this._content as PublishContent;
            if (!publishContent.bigVersion)
            {
                string updateFilePath = publishContent.GetUpdateFilePath();
                string updateFile = "/res_" + publishContent.resVersion + ".bin";
                string updateFullFile = publishContent.GetVersionPath() + updateFile;
                FileOperateUtil.CreateFileDirectory(updateFullFile);

                FolderEncoder folderEncoder = new FolderEncoder(updateFilePath, updateFullFile, "*.*");
                long size = folderEncoder.Encode(null);

                publishContent.updateFile = updateFile;
                publishContent.updateFileSize = size;
            }

            this.Success(publishContent);
        }
    }
}