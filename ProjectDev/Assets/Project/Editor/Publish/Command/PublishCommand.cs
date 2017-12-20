using Common.Command;

namespace Editor.Publish
{
    public class PublishCommand:MacroCommand
    {
        protected override void InitCommand()
        {
            AddSubCommand(new ReferenceABSettingCommand());
            AddSubCommand(new BuildAssetBundleCommand());
            AddSubCommand(new AssetBundleCompareCommand());
            AddSubCommand(new EncodeUpdateCommand());
            AddSubCommand(new BuildVersionCommand());
            AddSubCommand(new BuildPlayerCommand());
            AddSubCommand(new CopyToCacheCommand());
        }
    }
}