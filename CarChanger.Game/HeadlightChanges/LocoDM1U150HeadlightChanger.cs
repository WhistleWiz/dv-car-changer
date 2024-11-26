using CarChanger.Common.Configs;
using UnityEngine;

namespace CarChanger.Game.HeadlightChanges
{
    internal class LocoDM1U150HeadlightChanger : HeadlightChanger
    {
        private LocoDM1U150Config.HeadlightSettings _config;
        private MeshFilter? _whiteTop;
        private MeshFilter _whiteLow;
        private MeshFilter _red;
        private Mesh? _originalWhiteTop;
        private Mesh _originalWhiteLow;
        private Mesh _originalRed;

        protected override float BeamOffset => 0.0f;

        public LocoDM1U150HeadlightChanger(LocoDM1U150Config config, TrainCar car, HeadlightDirection direction) : base(car, direction)
        {
            _config = direction == HeadlightDirection.Front ? config.FrontSettings : config.RearSettings;

            // Only the front has a top headlight.
            if (direction == HeadlightDirection.Front)
            {
                _whiteTop = Root.Find($"dm1u-150_headlight_glass_{DirectionLetter}_HI").GetComponent<MeshFilter>();
                _originalWhiteTop = _whiteTop.sharedMesh;
            }

            _whiteLow = Root.Find($"dm1u-150_headlight_glass_{DirectionLetter}_LO").GetComponent<MeshFilter>();
            _red = Root.Find($"dm1u-150_headlight_glass_red_{DirectionLetter}").GetComponent<MeshFilter>();
            _originalWhiteLow = _whiteLow.sharedMesh;
            _originalRed = _red.sharedMesh;
        }

        public override void Apply()
        {
            // Check if the top headlight exists.
            if (_whiteTop != null && _config.WhiteTopMesh != null)
            {
                _whiteLow.sharedMesh = _config.WhiteTopMesh;
            }

            if (_config.WhiteLowMesh != null)
            {
                _whiteLow.sharedMesh = _config.WhiteLowMesh;
            }

            if (_config.RedMesh != null)
            {
                _red.sharedMesh = _config.RedMesh;
            }

            // Same as before, this time for the glares.
            if (_whiteTop != null)
            {
                SetGlares(_config.LeftGlarePosition,
                    _config.RightGlarePosition,
                    _config.TopGlarePosition,
                    _config.LeftGlarePosition,
                    _config.RightGlarePosition,
                    _config.TopGlarePosition,
                    _config.RedLeftGlarePosition,
                    _config.RedRightGlarePosition);
            }
            else
            {
                SetGlares(_config.LeftGlarePosition,
                    _config.RightGlarePosition,
                    _config.LeftGlarePosition,
                    _config.RightGlarePosition,
                    _config.RedLeftGlarePosition,
                    _config.RedRightGlarePosition);
            }

            UpdateBeams();
        }

        public override void Unapply()
        {
            if (_whiteTop != null)
            {
                _whiteTop.sharedMesh = _originalWhiteTop;
            }

            _whiteLow.sharedMesh = _originalWhiteLow;
            _red.sharedMesh = _originalRed;

            ResetGlares();
            UpdateBeams();
        }
    }
}
