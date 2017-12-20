using Common.Command;
using Editor.Tools;
using UnityEditor;

namespace Editor.Publish
{
    public class BuildAssetBundleCommand:Command
    {
        private PublishContent publishContent = null;

        public override void Execute(object content)
        {
            base.Execute(content);

            publishContent = _content as PublishContent;
            string resPath = PublishContent.GetResPath(publishContent.GetRootPath());
            string resABPath = PublishContent.GetABPath(resPath);

            //            bool clearResPath = this.publishContent.env == PublishEnvironment.ONLINE;
            bool clearResPath = publishContent.bigVersion;  //不清理res资源
            FileOperateUtil.CreateDirectory(resPath, clearResPath);

            BuildTarget target = publishContent.GetBuildTarget();
            AssetBundleTool.Build(resABPath, target);

            Success(publishContent);
        }

    }
}