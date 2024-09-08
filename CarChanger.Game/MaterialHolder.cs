using CarChanger.Common;
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
        public Material Glass = null!;
        public Material BodyExploded = null!;
        public Material InteriorExploded = null!;
        public Material InteriorExtraExploded = null!;
        public Material GlassBroken = null!;

        public MaterialHolder(TrainCar car)
        {
            Car = car;
        }

        public Material GetMaterial(SourceMaterial material, string path = "") => material switch
        {
            SourceMaterial.BodyDefault => Body,
            SourceMaterial.InteriorDefault => Interior,
            SourceMaterial.InteriorExtra => InteriorExtra,
            SourceMaterial.Windows => Glass,
            SourceMaterial.BodyExploded => BodyExploded,
            SourceMaterial.InteriorExploded => InteriorExploded,
            SourceMaterial.InteriorExtraExploded => InteriorExtraExploded,
            SourceMaterial.BrokenWindows => GlassBroken,
            SourceMaterial.FromPath => GetFromPath(path),
            _ => null!,
        };

        public Material GetFromPath(string path)
        {
            var t = Car.transform.Find(path);

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
