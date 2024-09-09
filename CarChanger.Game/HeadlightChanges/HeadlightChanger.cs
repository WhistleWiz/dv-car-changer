using DV.Simulation.Cars;
using System.Linq;
using UnityEngine;

namespace CarChanger.Game.HeadlightChanges
{
    internal abstract class HeadlightChanger
    {
        protected Transform Root;
        protected Headlight[] Lights;

        private Vector3[] _originalGlares;

        public HeadlightDirection Direction { get; }

        protected string DirectionLetter => GetDirectionLetter(Direction);

        protected virtual float BeamOffset => 0.0f;

        public HeadlightChanger(TrainCar car, HeadlightDirection direction)
        {
            Direction = direction;
            Root = GetRoot(car);
            Lights = Root.GetComponentsInChildren<Headlight>();
            _originalGlares = Lights.Select(x => x.glare.transform.localPosition).ToArray();
        }

        public abstract void Apply();

        public abstract void Unapply();

        protected virtual Transform GetRoot(TrainCar car)
        {
            return Direction == HeadlightDirection.Front ?
                car.transform.Find("[headlights]/FrontSide") :
                car.transform.Find("[headlights]/RearSide");
        }

        protected void SetGlares(params Vector3[] glares)
        {
            for (int i = 0; i < glares.Length; i++)
            {
                Lights[i].glare.transform.localPosition = glares[i];
            }
        }

        protected void ResetGlares()
        {
            for (int i = 0; i < Lights.Length; i++)
            {
                Lights[i].glare.transform.localPosition = _originalGlares[i];
            }
        }

        protected void UpdateBeams()
        {
            Vector3 offset = new Vector3(0, 0, Direction == HeadlightDirection.Front ? -BeamOffset : BeamOffset);

            for (int i = 0; i < Lights.Length; i++)
            {
                if (Lights[i].beamData.beam != null)
                {
                    Lights[i].beamData.beam.transform.localPosition = Lights[i].glare.transform.localPosition + offset;
                }
            }
        }

        protected static string GetDirectionLetter(HeadlightDirection dir) => dir == HeadlightDirection.Front ? "F" : "R";
    }

    public enum HeadlightDirection
    {
        Front,
        Rear
    }
}
