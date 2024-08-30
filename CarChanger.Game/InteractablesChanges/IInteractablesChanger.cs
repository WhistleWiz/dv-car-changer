using UnityEngine;

namespace CarChanger.Game.InteractablesChanges
{
    internal interface IInteractablesChanger
    {
        public void Apply(GameObject interactables);

        public void Unapply(GameObject interactables);
    }
}
