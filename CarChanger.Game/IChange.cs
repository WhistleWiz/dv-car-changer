using UnityEngine;

namespace CarChanger.Game
{
    public interface IChange
    {
        public void Apply(GameObject? newObject);

        public void Unapply(GameObject? newObject);
    }
}
