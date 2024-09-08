using CarChanger.Common.Configs;
using UnityEngine;

namespace CarChanger.Game.HeadlightChanges
{
    internal class LocoS282AHeadlightChanger : HeadlightChanger
    {
        private LocoS282AConfig _config;
        private MeshFilter _mesh;
        private Mesh _originalMesh;

        protected override float BeamOffset => 0.0f;

        public LocoS282AHeadlightChanger(LocoS282AConfig config, TrainCar car) : base(car, HeadlightDirection.Front)
        {
            _config = config;

            _mesh = Root.Find("ext headlights_glass_F").GetComponent<MeshFilter>();
            _originalMesh = _mesh.sharedMesh;
        }

        public override void Apply()
        {
            if (_config.Mesh != null)
            {
                _mesh.sharedMesh = _config.Mesh;
            }

            Lights[0].glare.transform.localPosition = _config.BeamPosition;
            Lights[1].glare.transform.localPosition = _config.BeamPosition;

            UpdateBeams();
        }

        public override void Unapply()
        {
            _mesh.sharedMesh = _originalMesh;

            ResetGlares();
            UpdateBeams();
        }

        protected override Transform GetRoot(TrainCar car) => car.transform.Find("LocoS282A_Headlights/FrontSide");
    }
}
