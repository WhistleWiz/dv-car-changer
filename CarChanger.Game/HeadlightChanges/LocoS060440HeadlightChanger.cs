using CarChanger.Common.Configs;
using UnityEngine;

namespace CarChanger.Game.HeadlightChanges
{
    internal class LocoS060440HeadlightChanger : HeadlightChanger
    {
        private LocoS060440Config.HeadlightSettings _config;
        private MeshFilter _whiteHigh;
        private MeshFilter _whiteLow;
        private Mesh _originalHigh;
        private Mesh _originalLow;

        protected override float BeamOffset => 0.045f;

        public LocoS060440HeadlightChanger(LocoS060440Config config, TrainCar car, HeadlightDirection direction) : base(car, direction)
        {
            _config = direction == HeadlightDirection.Front ? config.FrontSettings : config.RearSettings;

            _whiteHigh = Root.Find($"s060_headlight_glass_{DirectionLetter}_HI").GetComponent<MeshFilter>();
            _whiteLow = Root.Find($"s060_headlight_glass_{DirectionLetter}_LO").GetComponent<MeshFilter>();
            _originalHigh = _whiteHigh.sharedMesh;
            _originalLow = _whiteLow.sharedMesh;
        }

        public override void Apply()
        {
            if (_config.HighBeamMesh != null)
            {
                _whiteLow.sharedMesh = _config.HighBeamMesh;
            }

            if (_config.LowBeamMesh != null)
            {
                _whiteLow.sharedMesh = _config.LowBeamMesh;
            }

            SetGlares(_config.TopGlarePosition,
                _config.RightGlarePosition,
                _config.LeftGlarePosition);

            UpdateBeams();
        }

        public override void Unapply()
        {
            _whiteHigh.sharedMesh = _originalHigh;
            _whiteLow.sharedMesh = _originalLow;

            ResetGlares();
            UpdateBeams();
        }

        protected override Transform GetRoot(TrainCar car)
        {
            return Direction == HeadlightDirection.Front ?
                car.transform.Find("LocoS060_Headlights/FrontSide") :
                car.transform.Find("LocoS060_Headlights/RearSide");
        }
    }
}
