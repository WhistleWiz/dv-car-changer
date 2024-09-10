using CarChanger.Common;
using CarChanger.Common.Components;
using UnityEngine;

namespace CarChanger.Game
{
    // Terrible class, but very useful.
    public class MaterialHolder
    {
        public TrainCar Car;
        public Material Body = null!;
        public Material Interior = null!;
        public Material InteriorExtra = null!;
        public Material Windows = null!;
        public Material BodyExploded = null!;
        public Material InteriorExploded = null!;
        public Material InteriorExtraExploded = null!;
        public Material WindowsBroken = null!;

        public MaterialHolder(TrainCar car)
        {
            Car = car;
        }

        public Material GetMaterial(UseBodyMaterial comp)
        {
            return GetMaterial(comp.Material, comp.MaterialObjectPath, comp.FromInterior);
        }

        public Material GetMaterial(SourceMaterial material, string path = "", bool isInterior = false) => material switch
        {
            SourceMaterial.BodyDefault => Body,
            SourceMaterial.InteriorDefault => Interior,
            SourceMaterial.InteriorExtra => InteriorExtra,
            SourceMaterial.Windows => Windows,
            SourceMaterial.BodyExploded => BodyExploded,
            SourceMaterial.InteriorExploded => InteriorExploded,
            SourceMaterial.InteriorExtraExploded => InteriorExtraExploded,
            SourceMaterial.BrokenWindows => WindowsBroken,
            SourceMaterial.FromPath => GetFromPath(path, isInterior),
            _ => null!,
        };

        public Material GetFromPath(string path, bool isInterior)
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

            else return renderer.material;
        }
    }
}
