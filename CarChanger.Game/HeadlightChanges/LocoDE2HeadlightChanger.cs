using CarChanger.Common.Configs;
using UnityEngine;

namespace CarChanger.Game.HeadlightChanges
{
    internal class LocoDE2HeadlightChanger : HeadlightChanger
    {
        private LocoDE2Config.HeadlightSettings _config;
        private MeshFilter _white;
        private MeshFilter _red;
        private Mesh _originalWhite;
        private Mesh _originalRed;

        protected override float BeamOffset => 0.055f;

        public LocoDE2HeadlightChanger(LocoDE2Config config, TrainCar car, HeadlightDirection direction) : base(car, direction)
        {
            if (direction == HeadlightDirection.Front)
            {
                _config = config.FrontSettings;
            }
            else
            {
                _config = config.RearSettings;
            }

            _white = Root.Find($"ext headlights_glass_{GetDirectionLetter(direction)}").GetComponent<MeshFilter>();
            _red = Root.Find($"ext headlights_glass_red_{GetDirectionLetter(direction)}").GetComponent<MeshFilter>();
            _originalWhite = _white.sharedMesh;
            _originalRed = _red.sharedMesh;

            static string GetDirectionLetter(HeadlightDirection dir) => dir == HeadlightDirection.Front ? "F" : "R";
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
    }
}
