using UnityEngine;

namespace CarChanger.Game.InteriorChanges
{
    internal interface IInteriorChanger
    {
        public void Apply(GameObject interior);

        public void Unapply();
    }
}
