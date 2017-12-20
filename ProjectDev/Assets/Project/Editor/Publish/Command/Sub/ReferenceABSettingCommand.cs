using Common.Command;
using Editor.Tools;

namespace Editor.Publish
{
    public class ReferenceABSettingCommand:Command
    {
        public override void Execute(object content)
        {
            base.Execute(content);

            PublishContent publishContent = _content as PublishContent;
            if (publishContent.referenceABSetting)
            {
                AssetBundleTool.BuildDepend();
            }

            Success(publishContent);
        }
    }
}