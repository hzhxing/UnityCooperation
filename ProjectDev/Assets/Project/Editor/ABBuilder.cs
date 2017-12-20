using Editor.Tools;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class ABBuilder:EditorWindow
    {
//        [MenuItem("资源编译/AssetBundle/BuildAssetBundle",false,1)]
        public static void BuildAssetBundle()
        {
            GetWindow<ABBuilder>();
        }

        private string mExportPath;
		#if UNITY_IOS
		private BuildTarget mTarget = BuildTarget.iOS;
		#elif UNITY_ANDROID
		private BuildTarget mTarget = BuildTarget.Android;
		#endif

        void OnEnable()
        {
            mExportPath = Application.streamingAssetsPath + "/AssetBundles";
        }

        void OnGUI()
        {
            GUILayout.Width(500);
            GUILayout.Space(20);
            GUILayout.BeginVertical();
            GUILayout.Label("选择输出目录：");
            mExportPath = GUILayout.TextField(this.mExportPath);
            GUILayout.EndVertical();

            GUILayout.Space(20);

            GUILayout.BeginHorizontal();
            GUILayout.Label("选择平台：");
            if (GUILayout.Toggle(this.mTarget == BuildTarget.Android, "Android"))
            {
                mTarget = BuildTarget.Android;
            }

            if (GUILayout.Toggle(mTarget == BuildTarget.iOS, "IOS"))
            {
                mTarget = BuildTarget.iOS;
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(20);
            if (GUILayout.Button("Build"))
            {
                Build();
            }
        }

        private void Build()
        {
            AssetBundleTool.Build(mExportPath,mTarget,true);
        }
    }
}