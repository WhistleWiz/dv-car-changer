using CarChanger.Common;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO.Compression;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace CarChanger.Unity.Inspector
{
    [CustomEditor(typeof(CarChangePack))]
    internal class CarChangePackEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.Space();

            if (GUILayout.Button("Export mod"))
            {
                var pack = (CarChangePack)target;

                foreach (var item in pack.PackConfigs)
                {
                    if (item == null)
                    {
                        Debug.LogError("Please remove all empty configs before exporting.");
                        return;
                    }

                    if (!item.DoValidation(out var error))
                    {
                        Debug.LogError($"Error in {item.ModificationId}: {error}");
                        return;
                    }
                }

                var path = Export(pack);
                Debug.Log($"[{System.DateTime.Now:HH:mm:ss}] Pack exported!");
                GUIUtility.ExitGUI();
                EditorUtility.RevealInFinder(path);
            }
        }

        private static string Export(CarChangePack pack)
        {
            using var memoryStream = new MemoryStream();
            var fileName = pack.ModId;
            var path = AssetBundleHelper.GetFullPath(pack);

            using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
            {
                // Create Info.json from pack.
                JsonSerializer serializer = new JsonSerializer();
                serializer.Formatting = Formatting.Indented;

                var file = archive.CreateEntry($"{fileName}/{Constants.ModInfo}");

                using (var entryStream = file.Open())
                using (var streamWriter = new StreamWriter(entryStream))
                using (var jsonWr = new JsonTextWriter(streamWriter))
                {
                    serializer.Serialize(jsonWr, pack.GetModInfo());
                }

                // Create the asset bundle.
                var bundlePath = AssetBundleHelper.CreateBundle(path, AssetBundleHelper.GetAssetPath(pack),
                    new List<(Object, string?)> { (pack, null) });

                // Only add the bundle itself to the zip.
                // Meta files and manifests aren't needed.
                file = archive.CreateEntryFromFile(bundlePath, $"{fileName}/{Path.GetFileName(bundlePath)}");

                // Delete the bundle files, leaving only the one already inside the zip.
                AssetBundleHelper.DeleteBundle(path, bundlePath);
            }

            var outputPath = Path.Combine(path,
                $"{fileName}.zip");

            using (var fileStream = new FileStream(outputPath, FileMode.Create))
            {
                memoryStream.Seek(0, SeekOrigin.Begin);
                memoryStream.CopyTo(fileStream);
            }

            AssetDatabase.Refresh();

            return outputPath;
        }
    }
}
