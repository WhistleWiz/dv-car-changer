using CarChanger.Common.Configs;
using CarChanger.Game.HeadlightChanges;
using CarChanger.Game.InteractablesChanges;
using CarChanger.Game.InteriorChanges;
using UnityEngine;

namespace CarChanger.Game.Components
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

            Helpers.DestroyGameObjectIfNotNull(_explosionHandler);
            Helpers.DestroyGameObjectIfNotNull(_explosionHandlerInteriorLod);

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
                Body = _originalBody[0].GetComponentInChildren<Renderer>().material,
                Windows = TrainCar.transform.Find(
                    "CarPassenger/CarPassengerWindowsSide").GetComponent<Renderer>().material
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
                Windows = TrainCar.transform.Find(
                    "CarCaboose_exterior/CabooseWindowsStatic/CabooseWindowsStatic01").GetComponent<Renderer>().material
            };

            if (config.UseCustomBogies)
            {
                _bogiesChanged = true;
                ChangeBogies(TrainCar, config.FrontBogie, config.RearBogie, config.WheelRadius);
            }

            ChangeBody(config.BodyPrefab, config.HideOriginalBody);
            ChangeInterior(new BasicInteriorChanger(config, MatHolder, "CabooseInterior"));
            ChangeInteractables(new CabooseInteractablesChanger(config, MatHolder));

            _colliderHolder = new ColliderHolder(TrainCar, config.CollisionCollider, config.WalkableCollider, config.ItemsCollider);
        }

        private void ApplyDE2480(LocoDE2480Config config)
        {
            LogChange();

            MatHolder = new MaterialHolder(TrainCar)
            {
                Body = _originalBody[0].GetComponentInChildren<Renderer>().material,
                Interior = TrainCar.transform.Find(
                    "[interior LOD]/InteriorLOD/cab_LOD1").GetComponent<Renderer>().material,
                Windows = TrainCar.transform.Find(
                    "LocoDE2_Body/windows/window_01").GetComponent<Renderer>().material,
                Extra1 = TrainCar.transform.Find(
                    "[interior LOD]/InteriorLOD/deck_LOD1").GetComponent<Renderer>().material,
                BodyExploded = TrainCar.carLivery.explodedExternalInteractablesPrefab.transform.Find(
                    "DoorsWindows/C_DoorR/ext cab_door1a").GetComponent<Renderer>().material,
                InteriorExploded = TrainCar.carLivery.explodedInteriorPrefab.transform.Find(
                    "Cab").GetComponent<Renderer>().material,
                WindowsBroken = TrainCar.transform.Find(
                    "LocoDE2_Body/broken_windows").GetComponent<Renderer>().material,
                Extra1Exploded = TrainCar.carLivery.explodedInteriorPrefab.transform.Find(
                    "Deck").GetComponent<Renderer>().material
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
            ChangeInteriorLod(config.InteriorLODPrefab, config.HideOriginalInteriorLOD);
            ChangeInterior(new BasicInteriorChanger(config, MatHolder, config.HideControlDeck ? new[] { "Cab", "Deck" } : new[] { "Cab" }));
            ChangeInteractables(new LocoDE2480InteractablesChanger(config, MatHolder));

            if (config.UseCustomFrontHeadlights)
            {
                _frontHeadlights = new LocoDE2480HeadlightChanger(config, TrainCar, HeadlightDirection.Front);
                _frontHeadlights.Apply();
            }

            if (config.UseCustomRearHeadlights)
            {
                _rearHeadlights = new LocoDE2480HeadlightChanger(config, TrainCar, HeadlightDirection.Rear);
                _rearHeadlights.Apply();
            }

            _colliderHolder = new ColliderHolder(TrainCar, config.CollisionCollider, config.WalkableCollider, config.ItemsCollider);
        }

        private void ApplyDE6860(LocoDE6860Config config)
        {
            LogChange();

            MatHolder = new MaterialHolder(TrainCar)
            {
                Body = _originalBody[0].GetComponentInChildren<Renderer>().material,
                Interior = TrainCar.transform.Find(
                    "[interior LOD]/LocoDE6_InteriorLOD/cab_LOD1").GetComponent<Renderer>().material,
                Windows = TrainCar.transform.Find(
                    "LocoDE6_Body/windows/window_01").GetComponent<Renderer>().material,
                Extra1 = TrainCar.transform.Find(
                    "[interior LOD]/LocoDE6_InteriorLOD/engine_LOD1").GetComponent<Renderer>().material,
                BodyExploded = TrainCar.carLivery.explodedExternalInteractablesPrefab.transform.Find(
                    "DoorsWindows/DoorR/C_DoorR/cab_door01a").GetComponent<Renderer>().material,
                InteriorExploded = TrainCar.carLivery.explodedInteriorPrefab.transform.Find(
                    "Cab").GetComponent<Renderer>().material,
                WindowsBroken = TrainCar.transform.Find(
                    "LocoDE6_Body/broken_windows").GetComponent<Renderer>().material,
                Extra1Exploded = TrainCar.carLivery.explodedInteriorPrefab.transform.Find(
                    "EngineBay").GetComponent<Renderer>().material
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
            ChangeInteriorLod(config.InteriorLODPrefab, config.HideOriginalInteriorLOD);
            ChangeInterior(new BasicInteriorChanger(config, MatHolder, "Cab"));
            ChangeInteractables(new LocoDE6860InteractablesChanger(config, MatHolder));

            if (config.UseCustomFrontHeadlights)
            {
                _frontHeadlights = new LocoDE6860HeadlightChanger(config, TrainCar, HeadlightDirection.Front);
                _frontHeadlights.Apply();
            }

            if (config.UseCustomRearHeadlights)
            {
                _rearHeadlights = new LocoDE6860HeadlightChanger(config, TrainCar, HeadlightDirection.Rear);
                _rearHeadlights.Apply();
            }

            _colliderHolder = new ColliderHolder(TrainCar, config.CollisionCollider, config.WalkableCollider, config.ItemsCollider);
        }

        private void ApplyDH4670(LocoDH4670Config config)
        {
            LogChange();

            MatHolder = new MaterialHolder(TrainCar)
            {
                Body = _originalBody[0].GetComponentInChildren<Renderer>().material,
                Interior = TrainCar.transform.Find(
                    "[interior LOD]/InteriorLOD/cab_LOD1").GetComponent<Renderer>().material,
                Windows = TrainCar.transform.Find(
                    "LocoDH4_Body/windows/dh4_window_FL").GetComponent<Renderer>().material,
                BodyExploded = TrainCar.carLivery.explodedExternalInteractablesPrefab.transform.Find(
                    "DoorsWindows/DoorR/C_DoorR/dh4_exterior_door_R").GetComponent<Renderer>().material,
                InteriorExploded = TrainCar.carLivery.explodedInteriorPrefab.transform.Find(
                    "Cab").GetComponent<Renderer>().material,
                WindowsBroken = TrainCar.transform.Find(
                    "LocoDH4_Body/windows broken").GetComponent<Renderer>().material
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
            ChangeInteriorLod(config.InteriorLODPrefab, config.HideOriginalInteriorLOD);
            ChangeInterior(new BasicInteriorChanger(config, MatHolder, "Cab"));
            ChangeInteractables(new LocoDH4670InteractablesChanger(config, MatHolder));

            if (config.UseCustomFrontHeadlights)
            {
                _frontHeadlights = new LocoDH4670HeadlightChanger(config, TrainCar, HeadlightDirection.Front);
                _frontHeadlights.Apply();
            }

            if (config.UseCustomRearHeadlights)
            {
                _rearHeadlights = new LocoDH4670HeadlightChanger(config, TrainCar, HeadlightDirection.Rear);
                _rearHeadlights.Apply();
            }

            _colliderHolder = new ColliderHolder(TrainCar, config.CollisionCollider, config.WalkableCollider, config.ItemsCollider);
        }

        private void ApplyDM3540(LocoDM3540Config config)
        {
            LogChange();

            MatHolder = new MaterialHolder(TrainCar)
            {
                Body = _originalBody[0].GetComponentInChildren<Renderer>().material,
                Interior = TrainCar.transform.Find(
                    "[interior LOD]/LocoDM3_InteriorLOD/dm3_interior_LOD1").GetComponent<Renderer>().material,
                Windows = TrainCar.transform.Find(
                    "LocoDM3_Body/windows/window1").GetComponent<Renderer>().material,
                BodyExploded = TrainCar.carLivery.explodedExternalInteractablesPrefab.transform.Find(
                    "DoorsWindows/C_DoorL/ext cab_door2a").GetComponent<Renderer>().material,
                InteriorExploded = TrainCar.carLivery.explodedInteriorPrefab.transform.Find(
                    "Cab").GetComponent<Renderer>().material,
                WindowsBroken = TrainCar.transform.Find(
                    "LocoDM3_Body/broken_windows").GetComponent<Renderer>().material
            };

            ChangeBody(config.BodyPrefab, config.HideOriginalBody);
            ChangeInteriorLod(config.InteriorLODPrefab, config.HideOriginalInteriorLOD);
            ChangeInterior(new BasicInteriorChanger(config, MatHolder, config.HideGearPlaque ? new[] { "Cab", "GearPatternPlaque" } : new[] { "Cab" }));
            ChangeInteractables(new LocoDM3540InteractablesChanger(config, MatHolder));

            if (config.UseCustomFrontHeadlights)
            {
                _frontHeadlights = new LocoDM3540HeadlightChanger(config, TrainCar, HeadlightDirection.Front);
                _frontHeadlights.Apply();
            }

            if (config.UseCustomRearHeadlights)
            {
                _rearHeadlights = new LocoDM3540HeadlightChanger(config, TrainCar, HeadlightDirection.Rear);
                _rearHeadlights.Apply();
            }

            _colliderHolder = new ColliderHolder(TrainCar, config.CollisionCollider, config.WalkableCollider, config.ItemsCollider);
        }

        private void ApplyS060440(LocoS060440Config config)
        {
            LogChange();

            MatHolder = new MaterialHolder(TrainCar)
            {
                Body = _originalBody[0].GetComponentInChildren<Renderer>().material,
                Interior = TrainCar.transform.Find(
                    "[interior LOD]/LocoS060_InteriorLOD/s060_cab_LOD1").GetComponent<Renderer>().material,
                Windows = TrainCar.transform.Find(
                    "LocoS060_Body/windows/s060_cab_window_CL").GetComponent<Renderer>().material,
                Extra1 = TrainCar.carLivery.externalInteractablesPrefab.transform.Find(
                    "Indicators/I_CoalStorage/LocoS060_coal1").GetComponent<Renderer>().material,
                Extra2 = TrainCar.carLivery.externalInteractablesPrefab.transform.Find(
                    "Indicators/I_WaterStorageR/scaler/water").GetComponent<Renderer>().material,
                BodyExploded = TrainCar.carLivery.explodedExternalInteractablesPrefab.transform.Find(
                    "Interactables/Doors/DoorL/C_DoorL/s060_ext_door_L").GetComponent<Renderer>().material,
                InteriorExploded = TrainCar.carLivery.explodedInteriorPrefab.transform.Find(
                    "Static/s060_cab").GetComponent<Renderer>().material,
                WindowsBroken = TrainCar.transform.Find(
                    "LocoS060_Body/broken_windows").GetComponent<Renderer>().material
            };

            ChangeBody(config.BodyPrefab, config.HideOriginalBody);
            ChangeInteriorLod(config.InteriorLODPrefab, config.HideOriginalInteriorLOD);
            ChangeInterior(new BasicInteriorChanger(config, MatHolder, new[] { "Static/s060_cab", "Static/s060_cab_extras" }));
            ChangeInteractables(new LocoS060440InteractablesChanger(config, MatHolder));

            if (config.UseCustomFrontHeadlights)
            {
                _frontHeadlights = new LocoS060440HeadlightChanger(config, TrainCar, HeadlightDirection.Front);
                _frontHeadlights.Apply();
            }

            if (config.UseCustomRearHeadlights)
            {
                _rearHeadlights = new LocoS060440HeadlightChanger(config, TrainCar, HeadlightDirection.Rear);
                _rearHeadlights.Apply();
            }

            _colliderHolder = new ColliderHolder(TrainCar, config.CollisionCollider, config.WalkableCollider, config.ItemsCollider);
        }

        private void ApplyS282730A(LocoS282730AConfig config)
        {
            LogChange();

            MatHolder = new MaterialHolder(TrainCar)
            {
                Body = _originalBody[0].GetComponentInChildren<Renderer>().material,
                Interior = TrainCar.transform.Find(
                    "[interior LOD]/LocoS282A_InteriorLOD/s282_cab_LOD1").GetComponent<Renderer>().material,
                Windows = TrainCar.carLivery.externalInteractablesPrefab.transform.Find(
                    "Interactables/WindowR/C_WindowR/model/s282_window_glass_R").GetComponent<Renderer>().material,
                BodyExploded = TrainCar.carLivery.explodedExternalInteractablesPrefab.transform.Find(
                    "Interactables/Toolbox/C_Toolbox/model/s282_toolbox_lid_LOD1").GetComponent<Renderer>().material,
                InteriorExploded = TrainCar.carLivery.explodedInteriorPrefab.transform.Find(
                    "Static/Cab").GetComponent<Renderer>().material
            };

            ChangeBody(config.BodyPrefab, config.HideOriginalBody);
            ChangeInteriorLod(config.InteriorLODPrefab, config.HideOriginalInteriorLOD);
            ChangeInterior(new BasicInteriorChanger(config, MatHolder, new[] { "Static/Cab", "Static/Things" }));
            ChangeInteractables(new LocoS282730AInteractablesChanger(config, MatHolder));

            if (config.UseCustomHeadlights)
            {
                _frontHeadlights = new LocoS282730AHeadlightChanger(config, TrainCar);
                _frontHeadlights.Apply();
            }

            _colliderHolder = new ColliderHolder(TrainCar, config.CollisionCollider, config.WalkableCollider, config.ItemsCollider);
        }

        private void ApplyS282730B(LocoS282730BConfig config)
        {
            LogChange();

            MatHolder = new MaterialHolder(TrainCar)
            {
                Body = TrainCar.transform.Find(
                    "LocoS282B_Body/LOD0/s282_tender").GetComponent<Renderer>().material,
                Interior = TrainCar.transform.Find(
                    "LocoS282B_Body/LOD0/s282_tender_cab").GetComponent<Renderer>().material,
                Extra1 = TrainCar.carLivery.externalInteractablesPrefab.transform.Find(
                    "Coal&Water/I_TenderCoal/coal1").GetComponent<Renderer>().material,
                Extra2 = TrainCar.carLivery.externalInteractablesPrefab.transform.Find(
                    "Coal&Water/I_TenderWater/tender_water_scaler/tender_water").GetComponent<Renderer>().material
            };

            if (config.UseCustomBogies)
            {
                _bogiesChanged = true;
                ChangeBogies(TrainCar, config.FrontBogie, config.RearBogie, config.WheelRadius);
            }

            ChangeBody(config.BodyPrefab, config.HideOriginalBody);
            ChangeInteractables(new LocoS282730BInteractablesChanger(config, MatHolder));

            if (config.UseCustomHeadlights)
            {
                _rearHeadlights = new LocoS282730BHeadlightChanger(config, TrainCar);
                _rearHeadlights.Apply();
            }

            _colliderHolder = new ColliderHolder(TrainCar, config.CollisionCollider, config.WalkableCollider, config.ItemsCollider);
        }

        private void ApplyBE2260(LocoBE2260Config config)
        {
            LogChange();

            MatHolder = new MaterialHolder(TrainCar)
            {
                Body = _originalBody[0].GetComponentInChildren<Renderer>().material,
                Interior = TrainCar.transform.Find(
                    "[interior LOD]/LocoMicroshunter_InteriorLOD/microshunter_int_cab_LOD1").GetComponent<Renderer>().material,
                Windows = TrainCar.transform.Find(
                    "LocoMicroshunter_Body/windows/microshunter_window_FL").GetComponent<Renderer>().material,
                BodyExploded = TrainCar.carLivery.explodedExternalInteractablesPrefab.transform.Find(
                    "DoorsWindows/Door/C_Door/microshunter_ext_door").GetComponent<Renderer>().material,
                InteriorExploded = TrainCar.carLivery.explodedInteriorPrefab.transform.Find(
                    "Cab").GetComponent<Renderer>().material,
                WindowsBroken = TrainCar.transform.Find(
                    "LocoMicroshunter_Body/windows broken").GetComponent<Renderer>().material
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
            ChangeInteriorLod(config.InteriorLODPrefab, config.HideOriginalInteriorLOD);
            ChangeInterior(new BasicInteriorChanger(config, MatHolder, "Cab"));
            ChangeInteractables(new LocoBE2260InteractablesChanger(config, MatHolder));

            if (config.UseCustomFrontHeadlights)
            {
                _frontHeadlights = new LocoBE2260HeadlightChanger(config, TrainCar, HeadlightDirection.Front);
                _frontHeadlights.Apply();
            }

            if (config.UseCustomRearHeadlights)
            {
                _rearHeadlights = new LocoBE2260HeadlightChanger(config, TrainCar, HeadlightDirection.Rear);
                _rearHeadlights.Apply();
            }

            _colliderHolder = new ColliderHolder(TrainCar, config.CollisionCollider, config.WalkableCollider, config.ItemsCollider);
        }

        private void ApplyDE6860Slug(LocoDE6860SlugConfig config)
        {
            LogChange();

            MatHolder = new MaterialHolder(TrainCar)
            {
                Body = _originalBody[0].GetComponentInChildren<Renderer>().material
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
                _frontHeadlights = new LocoDE6860SlugHeadlightChanger(config, TrainCar, HeadlightDirection.Front);
                _frontHeadlights.Apply();
            }

            if (config.UseCustomRearHeadlights)
            {
                _rearHeadlights = new LocoDE6860SlugHeadlightChanger(config, TrainCar, HeadlightDirection.Rear);
                _rearHeadlights.Apply();
            }

            _colliderHolder = new ColliderHolder(TrainCar, config.CollisionCollider, config.WalkableCollider, config.ItemsCollider);
        }

        private void ApplyHandcar(LocoHandcarConfig config)
        {
            LogChange();

            MatHolder = new MaterialHolder(TrainCar)
            {
                Body = _originalBody[0].GetComponentInChildren<Renderer>().material
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

            _colliderHolder = new ColliderHolder(TrainCar, config.CollisionCollider, config.WalkableCollider, config.ItemsCollider);
        }

        private void ApplyCustomCar(CustomCarConfig config)
        {
            LogChange();

            MatHolder = new MaterialHolder(TrainCar);
            ChangeBody(config.BodyPrefab, false);
            ChangeInteriorLod(config.InteriorLODPrefab, false);
            ChangeInterior(new CustomCarInteriorChanger(config, MatHolder));

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
    }
}
