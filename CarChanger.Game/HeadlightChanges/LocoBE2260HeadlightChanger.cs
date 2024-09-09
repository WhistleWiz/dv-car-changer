using CarChanger.Common.Configs;
using UnityEngine;

namespace CarChanger.Game.HeadlightChanges
{
    internal class LocoBE2260HeadlightChanger : HeadlightChanger
    {
        private LocoBE2260Config _config;
        private MeshFilter _white;
        private Mesh _originalWhite;

        public LocoBE2260HeadlightChanger(LocoBE2260Config config, TrainCar car, HeadlightDirection direction) : base(car, direction)
        {
            _config = config;
            _white = Root.Find($"microshunter_headlight_glass_{GetDirectionLetter(direction)}").GetComponent<MeshFilter>();
            _originalWhite = _white.sharedMesh;
        }

        public override void Apply()
        {
            bool isFront = Direction == HeadlightDirection.Front;
            Mesh? mesh = isFront ? _config.FrontMesh : _config.RearMesh;
            Vector3 pos = isFront ? _config.FrontBeamPosition : _config.RearBeamPosition;

            if (mesh != null)
            {
                _white.sharedMesh = mesh;
            }

            Lights[0].glare.transform.localPosition = pos;

            UpdateBeams();
        }

        public override void Unapply()
        {
            _white.sharedMesh = _originalWhite;

            ResetGlares();
            UpdateBeams();
        }

        protected override Transform GetRoot(TrainCar car)
        {
            return Direction == HeadlightDirection.Front ?
                car.transform.Find("[headlights_microshunter]/FrontSide") :
                car.transform.Find("[headlights_microshunter]/RearSide");
        }
    }
}
