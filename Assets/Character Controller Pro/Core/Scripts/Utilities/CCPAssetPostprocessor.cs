#if UNITY_EDITOR
using UnityEditor;

namespace Lightbug.CharacterControllerPro.Core
{
    class CCPAssetPostprocessor : AssetPostprocessor
    {        
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            var rootPath = CharacterControllerProEditor.GetAssetRootFolder();

            foreach (string importedAsset in importedAssets)
            {
                if (importedAsset.Equals(rootPath))
                {
                    WelcomeWindow window = EditorWindow.GetWindow<WelcomeWindow>(true, "Welcome");
                }
            }
        }
    }

}

#endif
