using HarmonyLib;

namespace CarChanger.Game.Patches
{
    [HarmonyPatch(typeof(TrainCarExplosion))]
    internal static class TrainCarExplosionPatches
    {
        [HarmonyPatch(nameof(TrainCarExplosion.UpdateModelToExploded)), HarmonyPostfix]
        private static void UpdateModelToExplodedPostfix(TrainCar trainCar)
        {
            var explosion = trainCar.GetComponent<CarChangerExplosionManager>();

            if (explosion == null) return;

            explosion.HandleExplode();
        }

        [HarmonyPatch(nameof(TrainCarExplosion.RevertModelToUnexploded)), HarmonyPostfix]
        private static void RevertModelToUnexplodedPostfix(TrainCar trainCar)
        {
            var explosion = trainCar.GetComponent<CarChangerExplosionManager>();

            if (explosion == null) return;

            explosion.HandleUnexplode();
        }
    }
}
