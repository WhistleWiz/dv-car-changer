using UnityEditor;
using UnityEngine;

namespace CarChanger.Unity
{
    internal static class AssetHelper
    {
        public static void SaveAsset(Object asset)
        {
            EditorUtility.SetDirty(asset);
            AssetDatabase.SaveAssets();
        }
    }
}
