using UnityEditor;
using UnityEngine;

namespace Plugins.Editor
{
    [InitializeOnLoad]
    public static class SymlinkEditor
    {
        static SymlinkEditor()
        {
           // SymlinkUtil.CreateSymlink(Application.dataPath + "/ProjectDev/Project", "Art",
           //     "../../../ProjectArt/Assets/Project/Art");
            
           // Debug.Log("Create Symlink");
        }
    }
}