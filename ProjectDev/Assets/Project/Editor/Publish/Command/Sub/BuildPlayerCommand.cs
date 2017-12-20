using System.Collections.Generic;
using Common.Command;
using Editor.Tools;
using UnityEditor;
using UnityEngine;

namespace Editor.Publish
{
    public class BuildPlayerCommand:Command
    {
        public override void Execute(object content)
        {
            base.Execute(content);

            PublishContent publishContent = _content as PublishContent;
            if (publishContent.bigVersion)
            {
                BuildPlayer(publishContent.GetVersionPath(), publishContent.platform);
            }
           
            Success(publishContent);
        }

        private void BuildPlayer(string path, RuntimePlatform platform)
        {
            BuildTarget target = BuildTarget.Android;
            string buildPath = path;
            FileOperateUtil.CreateDirectory(path);
            if (platform == RuntimePlatform.Android)
            {
                target = BuildTarget.Android;
                buildPath = buildPath + "/Release.apk"; 
            }
            else if (platform == RuntimePlatform.IPhonePlayer)
            {
                target = BuildTarget.iOS;
                buildPath = buildPath + "/Release" ;
                FileOperateUtil.ClearDirectory(buildPath);
            }

            List<string> names = new List<string>();
            foreach (EditorBuildSettingsScene e in EditorBuildSettings.scenes)
            {
                if (e == null)
                    continue;
                if (e.enabled)
                    names.Add(e.path);
            }

            BuildPipeline.BuildPlayer(names.ToArray(), buildPath, target, BuildOptions.None);
        }
    }
}