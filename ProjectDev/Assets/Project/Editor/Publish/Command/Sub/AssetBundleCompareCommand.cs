using System;
using Common.Command;
using Editor.Tools;
using UnityEditor;
using UnityEngine;

namespace Editor.Publish
{
    public class AssetBundleCompareCommand:Command
    {
        private PublishContent publishContent = null;

        public override void Execute(object content)
        {
            base.Execute(content);

            publishContent = _content as PublishContent;
            string resPath = PublishContent.GetResPath(publishContent.GetRootPath());
            string resABPath = PublishContent.GetABPath(resPath);
            string resCachePath = resPath + "Cache";
            string resCacheABPath = PublishContent.GetABPath(resCachePath);

            FileOperateUtil.CreateDirectory(resCachePath);

            if (!publishContent.bigVersion)
            {
                ProgressBarUtil.Title = "AssetBundle文件拷贝和对比";
                ProgressBarUtil.Content = "AssetBundle文件拷贝和对比";
                ProgressBarUtil.Percent = 0.0f;
                ProgressBarUtil.Show();

                string updatePath = PublishContent.GetResPath(this.publishContent.GetUpdateFilePath());
                PublishUtil.Compare(resPath, resCachePath, updatePath, "*.ab", false, new ABProgress());

                ProgressBarUtil.Close();
            }
            else
            {
                ProgressBarUtil.Title = "AssetBundle文件拷贝";
                ProgressBarUtil.Content = "拷贝AssetBundle到缓存目录";
                ProgressBarUtil.Percent = 0.0f;
                ProgressBarUtil.Show();

                string projectABPath = PublishContent.GetABPath(Application.streamingAssetsPath);
                FileOperateUtil.ClearDirectory(projectABPath);
                FileOperateUtil.ClearDirectory(resCacheABPath);

                PublishUtil.Copy(resABPath, resCacheABPath, "*.ab", new ABProgress());

                ProgressBarUtil.Content = "拷贝AssetBundle到项目目录";
                PublishUtil.Copy(resCacheABPath, projectABPath, "*.ab", new ABProgress());

                ProgressBarUtil.Close();
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Success(publishContent);
        }
    }

    public class ABProgress : IProgress
    {

        public void SetPercent(float percent)
        {
            if (Mathf.Abs(percent - ProgressBarUtil.Percent) >= 0.01f)
            {
                ProgressBarUtil.Percent = percent;
            }
        }
    }
}