using System.Collections.Generic;

namespace CarChanger.Game
{
    internal class DefaultConfigSettings
    {
        public class LiveryConfig
        {
            public string LiveryName = string.Empty;
            public List<string> DefaultIds = new List<string>();
        }

        public List<LiveryConfig> Configs = new List<LiveryConfig>();

        public bool TryGetLivery(string livery, out LiveryConfig liveryConfig)
        {
            return Configs.TryFind(x => x.LiveryName == livery, out liveryConfig);
        }
    }
}
