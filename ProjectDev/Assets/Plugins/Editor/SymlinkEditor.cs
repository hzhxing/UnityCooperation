using UnityEditor;
using UnityEngine;

namespace Plugins.Editor
{
    public class SymlinkEditor
    {
        [InitializeOnLoadMethod]
        public static void BuildSymlink()
        {
            SymlinkUtil.CreateSymlink(Application.dataPath + "/Project", "Art",
                "../../../ProjectArt/Assets/Project/Art");
        }
    }
}