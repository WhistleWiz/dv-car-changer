using UnityEngine;

namespace CarChanger.Game.InteriorChanges
{
    internal interface IInteriorChanger
    {
        public abstract void Apply(GameObject interior);

        public abstract void Unapply(GameObject interior);
    }
}
