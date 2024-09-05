using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace CarChanger.Common
{
    [CreateAssetMenu(menuName = "DVCarChanger/Change Pack", order = 0)]
    public class CarChangePack : ScriptableObject
    {
        public string ModId = string.Empty;
        public string ModName = string.Empty;
        public string Author = string.Empty;
        public string Version = "1.0.0";
        public string HomePage = string.Empty;
        public string Repository = string.Empty;
        [Tooltip("Additional mod IDs that are required for this mod to work")]
        public string[] AdditionalRequirements = new string[0];

        public ModelConfig[] PackConfigs = new ModelConfig[0];

        public JObject GetModInfo()
        {
            List<string> reqs = new List<string> { Constants.MainModId };
            reqs.AddRange(AdditionalRequirements);

            var modInfo = new JObject
            {
                { "Id", ModId },
                { "DisplayName", ModName },
                { "Version", Version },
                { "Author", Author },
                { "ManagerVersion", "0.27.3" },
                { "Requirements", JToken.FromObject(reqs.ToArray()) },
            };

            // If a homepage was defined, also add the link.
            if (!string.IsNullOrEmpty(HomePage))
            {
                modInfo.Add("HomePage", HomePage);
            }

            // If a repository was defined, also add the link.
            if (!string.IsNullOrEmpty(Repository))
            {
                modInfo.Add("Repository", Repository);
            }

            return modInfo;
        }
    }
}
