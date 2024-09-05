using CarChanger.Common.Configs;
using UnityEngine;

namespace CarChanger.Game.HeadlightChanges
{
    internal class LocoDE6HeadlightChanger : HeadlightChanger
    {
        private LocoDE6Config.HeadlightSettings _config;
        private MeshFilter _white;
        private MeshFilter _red;
        private Mesh _originalWhite;
        private Mesh _originalRed;

        protected override float BeamOffset => 0.055f;

        public  LocoDE6HeadlightChanger(LocoDE6Config config, TrainCar car, HeadlightDirection direction) : base(car, direction)
        {
            if (direction == HeadlightDirection.Front)
            {
                _config = config.FrontSettings;
            }
            else
            {
                _config = config.RearSettings;
            }

            _white = Root.Find("headlight_glass").GetComponent<MeshFilter>();
            _red = Root.Find("headlight_glass_red").GetComponent<MeshFilter>();
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

            Lights[0].glare.transform.localPosition = _config.TopGlarePosition;
            Lights[1].glare.transform.localPosition = _config.LeftGlarePosition;
            Lights[2].glare.transform.localPosition = _config.RightGlarePosition;
            Lights[3].glare.transform.localPosition = _config.TopGlarePosition;
            Lights[4].glare.transform.localPosition = _config.LeftGlarePosition;
            Lights[5].glare.transform.localPosition = _config.RightGlarePosition;
            Lights[6].glare.transform.localPosition = _config.RedGlarePosition;

            UpdateBeams();
        }

        public override void Unapply()
        {
            _white.sharedMesh = _originalWhite;
            _red.sharedMesh = _originalRed;

            ResetGlares();
            UpdateBeams();
        }
    }
}
