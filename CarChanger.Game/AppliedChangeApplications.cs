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
            ResetInteriorLod();
            ResetHeadlights();
            ResetInterior();
            ResetInteractables();
            ResetColliders();

            if (_explosionHandler != null)
            {
                Destroy(_explosionHandler.gameObject);
            }

            Config?.Unapplied(TrainCar.gameObject);
            _changeApplied = false;
        }

        private void ApplyWagon(WagonConfig config)
        {
            LogChange();

            MatHolder = new MaterialHolder(TrainCar)
            {
                Body = _originalBody[0].GetComponentInChildren<Renderer>().material
            };

            if (config.UseCustomBogies)
            {
                _bogiesChanged = true;
                ChangeBogies(TrainCar, config.FrontBogie, config.RearBogie, config.WheelRadius);
            }

            ChangeBody(config.BodyPrefab, config.HideOriginalBody);

            _colliderHolder = new ColliderHolder(TrainCar, config.CollisionCollider, config.WalkableCollider, config.ItemsCollider);
        }

        private void ApplyPassenger(PassengerConfig config)
        {
            LogChange();

            MatHolder = new MaterialHolder(TrainCar)
            {
                Body = _originalBody[0].GetComponentInChildren<Renderer>().material
            };

            if (config.UseCustomBogies)
            {
                _bogiesChanged = true;
                ChangeBogies(TrainCar, config.FrontBogie, config.RearBogie, config.WheelRadius);
            }

            ChangeBody(config.BodyPrefab, config.HideOriginalBody);
            ChangeInteriorLod(null, config.HideOriginalInterior);

            _colliderHolder = new ColliderHolder(TrainCar, config.CollisionCollider, config.WalkableCollider, config.ItemsCollider);
        }

        private void ApplyCaboose(CabooseConfig config)
        {
            LogChange();

            MatHolder = new MaterialHolder(TrainCar)
            {
                Body = _originalBody[0].GetComponentInChildren<Renderer>().material,
                Interior = TrainCar.transform.Find(
                    "[interior LOD]/CabooseInterior_LOD1").GetComponent<Renderer>().material,
                Glass = TrainCar.transform.Find(
                    "CarCaboose_exterior/CabooseWindowsStatic/CabooseWindowsStatic01").GetComponent<Renderer>().material
            };

            ChangeBody(config.BodyPrefab, config.HideOriginalBody);
            ChangeInterior(new BasicInteriorChanger(config, MatHolder, "CabooseInterior"));
            ChangeInteractables(new CabooseInteractablesChanger(config, MatHolder));

            _colliderHolder = new ColliderHolder(TrainCar, config.CollisionCollider, config.WalkableCollider, config.ItemsCollider);
        }

        private void ApplyGroup(ModificationGroupConfig config)
        {
            LogChange();

            foreach (var item in config.ModificationsToActivate)
            {
                gameObject.AddComponent<AppliedChange>().Config = item;
            }

            // The group modification itself is empty, so it gets removed.
            Destroy(this);
        }

        private void ApplyDE6(LocoDE6Config config)
        {
            LogChange();

            MatHolder = new MaterialHolder(TrainCar)
            {
                Body = _originalBody[0].GetComponentInChildren<Renderer>().material,
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

            ChangeInterior(new BasicInteriorChanger(config, MatHolder, "Cab"));
            ChangeInteractables(new LocoDE6InteractablesChanger(config, MatHolder));

            _colliderHolder = new ColliderHolder(TrainCar, config.CollisionCollider, config.WalkableCollider, config.ItemsCollider);
        }

        private void ApplyS282A(LocoS282AConfig config)
        {
            LogChange();

            MatHolder = new MaterialHolder(TrainCar)
            {
                Body = _originalBody[0].GetComponentInChildren<Renderer>().material,
                Interior = TrainCar.transform.Find(
                    "[interior LOD]/LocoS282A_InteriorLOD/s282_cab_LOD1").GetComponent<Renderer>().material,
                Glass = TrainCar.carLivery.externalInteractablesPrefab.transform.Find(
                    "Interactables/WindowR/C_WindowR/model/s282_window_glass_R").GetComponent<Renderer>().material,
                BodyExploded = TrainCar.carLivery.explodedExternalInteractablesPrefab.transform.Find(
                    "Interactables/Toolbox/C_Toolbox/model/s282_toolbox_lid_LOD1").GetComponent<Renderer>().material,
                InteriorExploded = TrainCar.carLivery.explodedInteriorPrefab.transform.Find(
                    "Static/Cab").GetComponent<Renderer>().material,
            };

            ChangeBody(config.BodyPrefab, config.HideOriginalBody);

            if (config.UseCustomHeadlights)
            {
                _frontHeadlights = new LocoS282AHeadlightChanger(config, TrainCar);
                _frontHeadlights.Apply();
            }

            ChangeInterior(new BasicInteriorChanger(config, MatHolder, "Static"));
            ChangeInteractables(new LocoS282AInteractablesChanger(config, MatHolder));

            _colliderHolder = new ColliderHolder(TrainCar, config.CollisionCollider, config.WalkableCollider, config.ItemsCollider);
        }

        private void ApplyS282B(LocoS282BConfig config)
        {
            LogChange();

            MatHolder = new MaterialHolder(TrainCar)
            {
                Body = TrainCar.transform.Find(
                    "LocoS282B_Body/LOD0/s282_tender").GetComponent<Renderer>().material,
                Interior = TrainCar.transform.Find(
                    "LocoS282B_Body/LOD0/s282_tender_cab").GetComponent<Renderer>().material
            };

            ChangeBody(config.BodyPrefab, config.HideOriginalBody);

            if (config.UseCustomHeadlights)
            {
                _frontHeadlights = new LocoS282BHeadlightChanger(config, TrainCar);
                _frontHeadlights.Apply();
            }

            ChangeInteractables(new LocoS282BInteractablesChanger(config, MatHolder));

            _colliderHolder = new ColliderHolder(TrainCar, config.CollisionCollider, config.WalkableCollider, config.ItemsCollider);
        }

        private void ApplyCustomCar(CustomCarConfig config)
        {
            LogChange();

            MatHolder = new MaterialHolder(TrainCar);
            ChangeBody(config.BodyPrefab, false);
        }
    }
}
