using System;
using System.IO;
using Common.Version;
using UnityEditor;
using UnityEngine;

namespace Editor.Publish
{
    public class PublishWindow:EditorWindow
    {
        [MenuItem("发布/资源发布", false, 1)]
        public static void StartPublish()
        {
            PublishWindow window = GetWindow<PublishWindow>();
            window.ResetVersion();
        }

        private string version = String.Empty;
        private string resVersion = String.Empty;
        private bool bigVersion = false;

#if UNITY_IOS
        private RuntimePlatform mPlatform = RuntimePlatform.IPhonePlayer;
#else
        private RuntimePlatform mPlatform = RuntimePlatform.Android;
#endif

        private bool mCurBigVersion;
        private RuntimePlatform mCurPlatform;

        void OnGUI()
        {
            GUILayout.Width(500);
            GUILayout.Height(800);
            GUILayout.Space(20);
            GUILayout.BeginHorizontal();
            GUILayout.Label("选择平台：");
            if (GUILayout.Toggle(mPlatform == RuntimePlatform.Android, "Android"))
            {
                mPlatform = RuntimePlatform.Android;
            }
            if (GUILayout.Toggle(mPlatform == RuntimePlatform.IPhonePlayer, "IOS"))
            {
                mPlatform = RuntimePlatform.IPhonePlayer;
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(20);
            GUILayout.BeginHorizontal();
            GUILayout.Label("版本号：");
            version = GUILayout.TextField(this.version);
            GUILayout.EndHorizontal();

            GUILayout.Space(20);
            GUILayout.BeginHorizontal();
            GUILayout.Label("资源版本号：");
            resVersion = GUILayout.TextField(this.resVersion);
            GUILayout.EndHorizontal();

            GUILayout.Space(20);
            GUILayout.BeginHorizontal();
            if (GUILayout.Toggle(!this.bigVersion, "小版本"))
            {
                bigVersion = false;
            }
            if (GUILayout.Toggle(this.bigVersion, "大版本"))
            {
                bigVersion = true;
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(20);
            if (mCurPlatform != mPlatform || mCurBigVersion != bigVersion )
            {
                mCurPlatform = mPlatform;
                mCurBigVersion = bigVersion;

                ResetVersion();
            }

            if (GUILayout.Button("发布"))
            {
                Build();
            }
        }

        private void Build()
        {
            PublishContent content = new PublishContent(this.mPlatform);
            content.version = version;
            content.resVersion = resVersion;
            content.bigVersion = bigVersion;
            content.resUrl = "http://热更新资源地址";
            new PublishCommand().Execute(content);
        }

        private void ResetVersion()
        {
            string path = PublishContent.GetPlatformPath(PublishContent.DEFINE_PUBLISH_PATH,this.mPlatform);
            string file = path + "/version.xml";

            if (File.Exists(file))
            {
                string text = File.ReadAllText(file);
                VersionContent versionContent = new VersionContent();
                versionContent.Parse(text);

                string[] vStrList = versionContent.version.Split('.');
                if (bigVersion)
                {
                    version = vStrList[0] + "." + (Convert.ToInt32(vStrList[1]) + 1);
                    resVersion = version + ".0";
                }
                else
                {
                    if (versionContent.resVersions.Count > 0)
                    {
                        resVersion = versionContent.resVersions[versionContent.resVersions.Count - 1].version;
                    }
                    else
                    {
                        resVersion = versionContent.version + ".0";
                    }
                    string[] resVStrList = resVersion.Split('.');
                    resVersion = resVStrList[0] + "." + resVStrList[1] + "." + (Convert.ToInt32(resVStrList[2]) + 1);
                    version = versionContent.version;
                }
            }
            else
            {
                version = "1.0";
                resVersion = "1.0.0";
            }
        }
    }
}