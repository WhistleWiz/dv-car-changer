using CarChanger.Common.Configs;
using DV.Simulation.Cars;
using System.Linq;
using UnityEngine;

namespace CarChanger.Game.HeadlightChanges
{
    internal class LocoDE6HeadlightChanger : IHeadlightChanger
    {
        private const float BeamOffset = 0.055f;

        public HeadlightDirection Direction { get; set; }

        private LocoDE6Config.HeadlightSettings _config;
        private Transform _root;
        private MeshFilter _white;
        private MeshFilter _red;
        private Mesh _originalWhite;
        private Mesh _originalRed;
        private Headlight[] _lights;
        private Vector3[] _originalGlares;

        public  LocoDE6HeadlightChanger(LocoDE6Config config, TrainCar car, HeadlightDirection direction)
        {
            Direction = direction;

            if (direction == HeadlightDirection.Front)
            {
                _config = config.FrontSettings;
                _root = car.transform.Find("[headlights]/FrontSide");
            }
            else
            {
                _config = config.RearSettings;
                _root = car.transform.Find("[headlights]/RearSide");
            }

            _white = _root.Find("headlight_glass").GetComponent<MeshFilter>();
            _red = _root.Find("headlight_glass_red").GetComponent<MeshFilter>();
            _originalWhite = _white.sharedMesh;
            _originalRed = _red.sharedMesh;

            _lights = _root.GetComponentsInChildren<Headlight>();
            _originalGlares = _lights.Select(x => x.glare.transform.localPosition).ToArray();
        }

        public void Apply()
        {
            if (_config.White != null)
            {
                _white.sharedMesh = _config.White;
            }

            if (_config.Red != null)
            {
                _red.sharedMesh = _config.Red;
            }

            _lights[0].glare.transform.localPosition = _config.TopGlarePosition;
            _lights[1].glare.transform.localPosition = _config.LeftGlarePosition;
            _lights[2].glare.transform.localPosition = _config.RightGlarePosition;
            _lights[3].glare.transform.localPosition = _config.TopGlarePosition;
            _lights[4].glare.transform.localPosition = _config.LeftGlarePosition;
            _lights[5].glare.transform.localPosition = _config.RightGlarePosition;
            _lights[6].glare.transform.localPosition = _config.RedGlarePosition;

            UpdateBeams();
        }

        public void Unapply()
        {
            _white.sharedMesh = _originalWhite;
            _red.sharedMesh = _originalRed;

            for (int i = 0; i < _lights.Length; i++)
            {
                _lights[i].glare.transform.localPosition = _originalGlares[i];
            }

            UpdateBeams();
        }

        private void UpdateBeams()
        {
            Vector3 offset = new Vector3(0, 0, Direction == HeadlightDirection.Front ? -BeamOffset : BeamOffset);

            for (int i = 0; i < _lights.Length; i++)
            {
                if (_lights[i].beamData.beam != null)
                {
                    _lights[i].beamData.beam.transform.localPosition = _lights[i].glare.transform.localPosition + offset;
                }
            }
        }
    }
}
