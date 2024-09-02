using CarChanger.Common.Configs;
using CarChanger.Game.HeadlightChanges;
using CarChanger.Game.InteractablesChanges;
using CarChanger.Game.InteriorChanges;
using UnityEngine;

namespace CarChanger.Game
{
    internal partial class AppliedChange : MonoBehaviour
    {
        private void ReturnToDefault()
        {
            // No need to output anything if the car isn't in the world anymore.
            if (TrainCar.logicCar != null)
            {
                if (Config != null)
                {
                    CarChangerMod.Log($"Removing change {Config.ModificationId} from [{TrainCar.ID}|{TrainCar.carLivery.id}]");
                }
                else
                {
                    CarChangerMod.Log($"Returning to default [{TrainCar.ID}|{TrainCar.carLivery.id}]");
                }
            }

            ResetBogies();
            ResetBody();
            ResetHeadlights();
            ResetInterior();
            ResetInteractables();

            if (_explosionHandler != null)
            {
                Destroy(_explosionHandler.gameObject);
            }

            Config?.Unapplied(TrainCar.gameObject);
            _changeApplied = false;
        }

        private void ApplyWagon(WagonConfig config)
        {
            CarChangerMod.Log($"Applying change {config.ModificationId} to [{TrainCar.ID}|{TrainCar.carLivery.id}]");

            MatHolder = new MaterialHolder(TrainCar)
            {
                Body = _originalBody.GetComponentInChildren<Renderer>().material
            };

            if (config.UseCustomBogies)
            {
                _bogiesChanged = true;
                ChangeBogies(TrainCar, config.FrontBogie, config.RearBogie, config.WheelRadius);
            }

            _bodyHidden = config.HideOriginalBody;
            ChangeBody(config.BodyPrefab, config.HideOriginalBody);
        }

        private void ApplyGroup(ModificationGroupConfig config)
        {
            foreach (var item in config.ModificationsToActivate)
            {
                gameObject.AddComponent<AppliedChange>().Config = item;
            }

            // The group modification itself is empty, so it gets removed.
            Destroy(this);
        }

        private void ApplyDE6(LocoDE6Config config)
        {
            CarChangerMod.Log($"Applying change {config.ModificationId} to [{TrainCar.ID}|{TrainCar.carLivery.id}]");

            MatHolder = new MaterialHolder(TrainCar)
            {
                Body = _originalBody.GetComponentInChildren<Renderer>().material,
                Interior = TrainCar.transform.Find(
                    "[interior LOD]/LocoDE6_InteriorLOD/cab_LOD1").GetComponent<Renderer>().material,
                Glass = TrainCar.transform.Find(
                    "LocoDE6_Body/windows/window_01").GetComponent<Renderer>().material,
                BodyExploded = TrainCar.carLivery.explodedExternalInteractablesPrefab.transform.Find(
                    "DoorsWindows/DoorR/C_DoorR/cab_door01a").GetComponent<Renderer>().material,
                InteriorExploded = TrainCar.carLivery.explodedInteriorPrefab.transform.Find(
                    "Cab").GetComponent<Renderer>().material,
                GlassBroken = TrainCar.transform.Find(
                    "LocoDE6_Body/broken_windows").GetComponent<Renderer>().material,
            };

            if (config.UseCustomBogies)
            {
                _bogiesChanged = true;
                _bogiesPowered = true;

                var wheelStates = GetCurrentPoweredWheelStates();

                ChangeBogies(TrainCar, config.FrontBogie, config.RearBogie, config.WheelRadius);
                MakeBogiesPowered(TrainCar, wheelStates, config.WheelRadius);
            }

            ChangeBody(config.BodyPrefab, config.HideOriginalBody);

            if (config.UseCustomFrontHeadlights)
            {
                _frontHeadlights = new LocoDE6HeadlightChanger(config, TrainCar, HeadlightDirection.Front);
                _frontHeadlights.Apply();
            }

            if (config.UseCustomRearHeadlights)
            {
                _rearHeadlights = new LocoDE6HeadlightChanger(config, TrainCar, HeadlightDirection.Rear);
                _rearHeadlights.Apply();
            }

            ChangeInterior(new LocoDE6InteriorChanger(config, MatHolder));
            ChangeInteractables(new LocoDE6InteractablesChanger(config, MatHolder));
        }

        private void ApplyCustomCar(CustomCarConfig config)
        {
            CarChangerMod.Log($"Applying change {config.ModificationId} to [{TrainCar.ID}|{TrainCar.carLivery.id}]");

            MatHolder = new MaterialHolder(TrainCar);
            ChangeBody(config.BodyPrefab, false);
        }
    }
}
