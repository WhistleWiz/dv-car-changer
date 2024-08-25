using CarChanger.Common;
using DV.JObjectExtstensions;
using HarmonyLib;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;

namespace CarChanger.Game.Patches
{
    [HarmonyPatch(typeof(SaveGameManager))]
    internal class SaveGameManagerPatches
    {
        [HarmonyPatch("DoSaveIO"), HarmonyPrefix]
        public static void InjectSaveData(SaveGameData data)
        {
            var loadedData = data.GetJObject(Constants.SaveKey);
            loadedData ??= new JObject();

            CarChangerMod.Log("Saving...");
            ChangeManager.SaveData.Clear();
            ChangeManager.SaveData = CarSpawner.Instance.AllCars.Where(x => x.logicCar != null).ToDictionary(x => x.logicCar.carGuid,
                x => x.GetAppliedChanges().Where(x => x.Config != null).Select(x => x.Config!.ModificationId).ToList());

            loadedData.SetObjectViaJSON(Constants.SaveDataKey, new DataHolder(ChangeManager.SaveData));
            data.SetJObject(Constants.SaveKey, loadedData);
        }

        [HarmonyPatch(nameof(SaveGameManager.FindStartGameData)), HarmonyPostfix]
        public static void ExtractSaveData(SaveGameManager __instance)
        {
            if (__instance.data == null) return;

            var data = __instance.data.GetJObject(Constants.SaveKey);

            if (data == null) return;

            var holder = data.GetObjectViaJSON<DataHolder>(Constants.SaveDataKey);

            if (holder == null) return;

            ChangeManager.SaveData = holder.ToData();
        }

        private class DataHolder
        {
            public string[] Keys;
            public List<string>[] Values;

            public DataHolder()
            {
                Keys = new string[0];
                Values = new List<string>[0];
            }

            public DataHolder(Dictionary<string, List<string>> data)
            {
                Keys = data.Keys.ToArray();
                Values = data.Values.ToArray();
            }

            public Dictionary<string, List<string>> ToData()
            {
                Dictionary<string, List<string>> data = new Dictionary<string, List<string>>();
                int length = Keys.Length;

                for (int i = 0; i < length; i++)
                {
                    data.Add(Keys[i], Values[i]);
                }

                return data;
            }
        }
    }
}
