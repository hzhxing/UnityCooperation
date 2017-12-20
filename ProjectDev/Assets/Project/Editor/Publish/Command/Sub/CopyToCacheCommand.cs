using Common.Command;
using Editor.Tools;

namespace Editor.Publish
{
    public class CopyToCacheCommand:Command
    {
        public override void Execute(object content)
        {
            base.Execute(content);

            PublishContent publishContent = content as PublishContent;

            if (!publishContent.bigVersion)
            {
                ProgressBarUtil.Title = "AssetBundle文件拷贝";
                ProgressBarUtil.Content = "AssetBundle文件拷贝到缓存目录";
                ProgressBarUtil.Percent = 0.0f;
                ProgressBarUtil.Show();

                string resPath = PublishContent.GetResPath(publishContent.GetRootPath());
                string resCachePath = resPath + "Cache";
                string updateResPath = PublishContent.GetResPath(publishContent.GetUpdateFilePath());
                PublishUtil.Copy(updateResPath,resCachePath,"*.ab",new ABProgress());

                string versionResCachePath = PublishContent.GetResPath(publishContent.GetVersionPath());
                PublishUtil.Copy(resCachePath,versionResCachePath,"*.ab",new ABProgress());

                ProgressBarUtil.Title = "Lua文件拷贝";
                ProgressBarUtil.Content = "Lua文件拷贝到缓存目录";
                ProgressBarUtil.Percent = 0.0f;
                string luaPath = PublishContent.GetLuaPath(publishContent.GetRootPath());
                string luaCachePath = luaPath + "Cache";
                string updateLuaPath = PublishContent.GetLuaPath(publishContent.GetUpdateFilePath());
                PublishUtil.Copy(updateLuaPath, luaCachePath, "*.lua", new ABProgress());

                string versionLuaCachePath = PublishContent.GetLuaPath(publishContent.GetVersionPath());
                PublishUtil.Copy(luaCachePath, versionLuaCachePath, "*.lua", new ABProgress());

                ProgressBarUtil.Close();
            }

            Success(publishContent);
        }
    }
}