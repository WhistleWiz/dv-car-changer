using CarChanger.Common.Configs;
using UnityEngine;

namespace CarChanger.Game.HeadlightChanges
{
    internal class LocoDH4670HeadlightChanger : HeadlightChanger
    {
        private LocoDH4670Config.HeadlightSettings _config;
        private MeshFilter _whiteHigh;
        private MeshFilter _whiteLow;
        private MeshFilter _red;
        private Mesh _originalWhiteHigh;
        private Mesh _originalWhiteLow;
        private Mesh _originalRed;

        protected override float BeamOffset => 0.045f;

        public LocoDH4670HeadlightChanger(LocoDH4670Config config, TrainCar car, HeadlightDirection direction) : base(car, direction)
        {
            _config = direction == HeadlightDirection.Front ? config.FrontSettings : config.RearSettings;

            _whiteHigh = Root.Find($"headlight_glass_{DirectionLetter}_HI").GetComponent<MeshFilter>();
            _whiteLow = Root.Find($"headlight_glass_{DirectionLetter}_LO").GetComponent<MeshFilter>();
            _red = Root.Find($"headlight_glass_red_{DirectionLetter}").GetComponent<MeshFilter>();
            _originalWhiteHigh = _whiteHigh.sharedMesh;
            _originalWhiteLow = _whiteLow.sharedMesh;
            _originalRed = _red.sharedMesh;
        }

        public override void Apply()
        {
            if (_config.WhiteHighBeamMesh != null)
            {
                _whiteLow.sharedMesh = _config.WhiteHighBeamMesh;
            }

            if (_config.WhiteLowBeamMesh != null)
            {
                _whiteLow.sharedMesh = _config.WhiteLowBeamMesh;
            }

            if (_config.RedMesh != null)
            {
                _red.sharedMesh = _config.RedMesh;
            }

            SetGlares(_config.TopGlarePosition,
                _config.TopGlarePosition,
                _config.LeftGlarePosition,
                _config.LeftGlarePosition,
                _config.RightGlarePosition,
                _config.RightGlarePosition,
                _config.RedLeftGlarePosition,
                _config.RedRightGlarePosition);

            //Lights[0].glare.transform.localPosition = _config.TopGlarePosition;
            //Lights[1].glare.transform.localPosition = _config.TopGlarePosition;
            //Lights[2].glare.transform.localPosition = _config.LeftGlarePosition;
            //Lights[3].glare.transform.localPosition = _config.LeftGlarePosition;
            //Lights[4].glare.transform.localPosition = _config.RightGlarePosition;
            //Lights[5].glare.transform.localPosition = _config.RightGlarePosition;
            //Lights[6].glare.transform.localPosition = _config.RedLeftGlarePosition;
            //Lights[7].glare.transform.localPosition = _config.RedRightGlarePosition;

            UpdateBeams();
        }

        public override void Unapply()
        {
            _whiteHigh.sharedMesh = _originalWhiteHigh;
            _whiteLow.sharedMesh = _originalWhiteLow;
            _red.sharedMesh = _originalRed;

            ResetGlares();
            UpdateBeams();
        }

        protected override Transform GetRoot(TrainCar car)
        {
            return Direction == HeadlightDirection.Front ?
                car.transform.Find("[headlights_dh4]/FrontSide") :
                car.transform.Find("[headlights_dh4]/RearSide");
        }
    }
}
