using System;
using UnityModManagerNet;

namespace CarChanger.Game
{
    [Serializable]
    public class Settings : UnityModManager.ModSettings, IDrawable
    {
        [Draw("Spawn With No Modification Chance", Tooltip = "The chance for a car to spawn without any modification\n" +
            "Does not apply to cars with a default modification set", Type = DrawType.Slider, Min = 0.0, Max = 1.0)]
        public float NoModificationChance = 0.2f;

        public override void Save(UnityModManager.ModEntry modEntry)
        {
            Save(this, modEntry);
        }

        public void OnChange() { }
    }
}
