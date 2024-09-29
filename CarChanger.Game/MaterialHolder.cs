using CarChanger.Common;
using CarChanger.Common.Components;
using DV.ThingTypes;
using DV.ThingTypes.TransitionHelpers;
using UnityEngine;

namespace CarChanger.Game
{
    // Terrible class, but very useful.
    public class MaterialHolder
    {
        private static Texture? s_explodeTex;
        private static Texture ExplodeTexture => Helpers.GetCached(ref s_explodeTex, () =>
            TrainCarType.LocoShunter.ToV2().explodedExternalInteractablesPrefab.transform.Find(
                "DoorsWindows/C_DoorR/ext cab_door1a").GetComponent<Renderer>().material.mainTexture);

        public TrainCar Car;
        public Material Body = null!;
        public Material Interior = null!;
        public Material Windows = null!;
        //public Material Bogie = null!;
        public Material Extra1 = null!;
        public Material Extra2 = null!;
        public Material BodyExploded = null!;
        public Material InteriorExploded = null!;
        public Material WindowsBroken = null!;
        //public Material BogieExploded = null!;
        public Material Extra1Exploded = null!;
        public Material Extra2Exploded = null!;

        public MaterialHolder(TrainCar car)
        {
            Car = car;
        }

        public Material GetMaterial(UseDefaultMaterial comp)
        {
            return GetMaterial(comp.Material, comp.MaterialObjectPath, comp.FromInterior, comp.GenerateExplodedMaterialProcedurally);
        }

        public Material GetMaterial(SourceMaterial material, string path = "", bool isInterior = false, bool proceduralExplodeMaterials = false) => material switch
        {
            SourceMaterial.Body => Body,
            SourceMaterial.Interior => Interior,
            SourceMaterial.Windows => Windows,

            SourceMaterial.Extra1 => Extra1,
            SourceMaterial.Extra2 => Extra2,

            SourceMaterial.BodyExploded => Helpers.GetCached(ref BodyExploded!, () => proceduralExplodeMaterials ? GenerateProceduralExplosionMaterial(Body) : null!),
            SourceMaterial.InteriorExploded => Helpers.GetCached(ref InteriorExploded!, () => proceduralExplodeMaterials ? GenerateProceduralExplosionMaterial(Interior) : null!),
            SourceMaterial.BrokenWindows => WindowsBroken,

            SourceMaterial.Extra1Exploded => Helpers.GetCached(ref Extra1Exploded!, () => proceduralExplodeMaterials ? GenerateProceduralExplosionMaterial(Extra1) : null!),
            SourceMaterial.Extra2Exploded => Helpers.GetCached(ref Extra2Exploded!, () => proceduralExplodeMaterials ? GenerateProceduralExplosionMaterial(Extra2) : null!),

            SourceMaterial.FromPath => GetFromPath(path, isInterior, proceduralExplodeMaterials),
            _ => null!
        };

        public Material GetFromPath(string path, bool isInterior, bool proceduralExplodeMaterials = false)
        {
            var t = isInterior ? Car.interior.transform.Find(path) : Car.transform.Find(path);

            if (t == null)
            {
                return null!;
            }

            var renderer = t.GetComponent<Renderer>();

            if (renderer == null)
            {
                return null!;
            }

            if (proceduralExplodeMaterials)
            {
                return GenerateProceduralExplosionMaterial(renderer.material);
            }

            return renderer.material;
        }

        public static Material GenerateProceduralExplosionMaterial(Material original)
        {
            var mat = new Material(original);
            mat.mainTexture = ExplodeTexture;
            return mat;
        }
    }
}
