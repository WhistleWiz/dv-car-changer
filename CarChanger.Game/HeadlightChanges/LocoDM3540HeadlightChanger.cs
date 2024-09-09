using CarChanger.Common.Configs;
using UnityEngine;

namespace CarChanger.Game.HeadlightChanges
{
    internal class LocoDM3540HeadlightChanger : HeadlightChanger
    {
        private LocoDM3540Config.HeadlightSettings _config;
        private MeshFilter _white;
        private MeshFilter _red;
        private Mesh _originalWhite;
        private Mesh _originalRed;

        protected override float BeamOffset => 0.072f;

        public LocoDM3540HeadlightChanger(LocoDM3540Config config, TrainCar car, HeadlightDirection direction) : base(car, direction)
        {
            _config = direction == HeadlightDirection.Front ? config.FrontSettings : config.RearSettings;

            _white = Root.Find($"headlights_glass_{DirectionLetter}").GetComponent<MeshFilter>();
            _red = Root.Find($"headlights_glass_red_{DirectionLetter}").GetComponent<MeshFilter>();
            _originalWhite = _white.sharedMesh;
            _originalRed = _red.sharedMesh;
        }

        public override void Apply()
        {
            if (_config.WhiteMesh != null)
            {
                _white.sharedMesh = _config.WhiteMesh;
            }

            if (_config.RedMesh != null)
            {
                _red.sharedMesh = _config.RedMesh;
            }

            Lights[0].glare.transform.localPosition = _config.LeftGlarePosition;
            Lights[1].glare.transform.localPosition = _config.RightGlarePosition;
            Lights[2].glare.transform.localPosition = _config.LeftGlarePosition;
            Lights[3].glare.transform.localPosition = _config.RightGlarePosition;
            Lights[4].glare.transform.localPosition = _config.RedGlarePosition;

            UpdateBeams();
        }

        public override void Unapply()
        {
            _white.sharedMesh = _originalWhite;
            _red.sharedMesh = _originalRed;

            ResetGlares();
            UpdateBeams();
        }

        protected override Transform GetRoot(TrainCar car)
        {
            return Direction == HeadlightDirection.Front ?
                car.transform.Find("[headlights_dm3]/FrontSide") :
                car.transform.Find("[headlights_dm3]/RearSide");
        }
    }
}
