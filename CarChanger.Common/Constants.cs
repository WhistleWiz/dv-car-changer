﻿namespace CarChanger.Common
{
    public static class Constants
    {
        public const string MainModId = "DVCarChanger";
        public const string ModInfo = "Info.json";
        public const string Bundle = "change_bundle";
        public const string ConfigFile = "config.json";
        public const string SaveKey = "dv_car_changer";
        public const string SaveDataKey = "data";

        public const string BogieName = "bogie_car";
        public const string BogieF = "BogieF";
        public const string BogieR = "BogieR";
        public const string BogieBrakePadsPath = "bogie_car/bogie2brakes/Bogie2BrakePads";
        public const string BogieContactPointsPath = "bogie_car/ContactPoints";
        public const string Axle = "[axle]";

        public static class Wheels
        {
            public const float RadiusDefault = 0.459f;
            public const float RadiusDE2480 = 0.5025f;
            public const float RadiusDE6 = 0.5335f;
            public const float RadiusDH4 = 0.535f;
            public const float RadiusDM1P = 0.42f;
            public const float RadiusBE2260 = 0.325f;
            public const float RadiusHandcar = 0.290f;
        }

        public static class MenuOrderConstants
        {
            public const int Pack = 0;
            public const int Unpowered = 100;
            public const int Diesel = 200;
            public const int Steam = 300;
            public const int Electric = 400;
            public const int Extras = 500;
            public const int Other = 1000;
        }

        private static string[] s_materialNames = new[]
        {
            "AmpLimiter",
            "AntiWheelslipComputer",
            "ArmedHelicopter",
            "ArrowCareerManagerScreen",
            "AutomaticTrainStop",
            "BallastMed",
            "BallastNew",
            "BallastOld",
            "Barlow-SemiBold SDF Material/Barlow-SemiBold SDF Atlas",
            "BeaconAmber",
            "BeaconBlue",
            "BeaconRed",
            "bogie2",
            "BookCover",
            "BookPaper",
            "BookPaperBack",
            "Boombox",
            "BottlePlastic",
            "box",
            "BrakeCylinderLEDBar",
            "BusCity80s_01_new_orange",
            "BusCity80s_01_new_white",
            "Cabinet",
            "canisters",
            "car_highlight_invalid",
            "car_highlight_valid",
            "CarAutorackBlue",
            "CarAutorackGreen",
            "CarAutorackRed",
            "CarAutorackYellow",
            "CarBoxcar_Brown",
            "CarBoxcar_Green",
            "CarBoxcar_Pink",
            "CarBoxcar_Red",
            "CarBoxcarMilitary",
            "CarCabooseRed_Body",
            "CarCabooseRed_Interior",
            "CarCity80s_01_Cargo",
            "CareerManager",
            "CareerManager_Green",
            "CarFlatcarCBBulkheadStakes_Brown",
            "CarFlatcarCBBulkheadStakes_Military",
            "CarFlatcarShort",
            "CarGondolaGreen",
            "CarGondolaGrey",
            "CarGondolaRed",
            "CarHopperBrown",
            "CarHopperCovered",
            "CarHopperTeal",
            "CarHopperYellow",
            "CarMidsize90s_02_Cargo",
            "CarNuclearFlask",
            "CarOffroad80s_01_military",
            "CarPassengerBlue",
            "CarPassengerGreen",
            "CarPassengerRed",
            "CarRefrigerator",
            "CarStockcar_Brown",
            "CarStockcar_Green",
            "CarStockcar_Red",
            "CarTankBlack",
            "CarTankBlue",
            "CarTankChrome",
            "CarTanker_WhiteMilk",
            "CarTankOrange",
            "CarTankWhite",
            "CarTankYellow",
            "cartonbox",
            "CassetteTapeAlbum",
            "CassetteTapePlaylist",
            "chicken_mat",
            "Chicken_v2_mat",
            "Chicken_v3_mat",
            "Chrome",
            "ClawHammer",
            "Clinometer",
            "ClinometerGauge",
            "coal",
            "Coal",
            "CommsRadio",
            "CommsRadio_Arrow",
            "CommsRadioLEDGlare",
            "Compass",
            "CompassDial",
            "ConcurrentJobs1_Tex_1",
            "ConcurrentJobs1Info_Tex_1",
            "ConcurrentJobs2_Tex_1",
            "ConcurrentJobs2Info_Tex_1",
            "ContainerMilitary",
            "Containers_d",
            "CrimpingTool",
            "DE2_Tex_1",
            "DE2Info_Tex_1",
            "DE6_Tex_1",
            "DE6Info_Tex_1",
            "DebrisMetal3D",
            "Default-Material",
            "Desk",
            "DieselSmoke",
            "Digit SDF Material",
            "DigitalClock",
            "DigitalDisplayGreen",
            "DigitalSpeedometer",
            "DigitalSpeedometerDigit",
            "Dispatcher1_Tex_1",
            "Dispatcher1Info_Tex_1",
            "DistanceTracker",
            "DistanceTrackerDisplay",
            "Domestic_pig_mat",
            "Domestic_sheep_mat",
            "DuctTape",
            "ElectricFlash",
            "ElectricHandDrill",
            "ElectricLightning",
            "ElectricSparks",
            "ember",
            "Ember",
            "EOTLantern",
            "EOTLanternGlass",
            "ExcavatorNewChock",
            "ExhaustSmokeBlack",
            "Explosion",
            "Explosion2",
            "ExplosionSmokeBlack",
            "FactorySmoke",
            "FarmTractor",
            "FeeInventory",
            "FillerGun",
            "Fire",
            "Fire2",
            "Fireball",
            "FireballGray",
            "FlagMarkerBlue",
            "FlagMarkerCyan",
            "FlagMarkerGreen",
            "FlagMarkerOrange",
            "FlagMarkerPurple",
            "FlagMarkerRed",
            "FlagMarkerWhite",
            "FlagMarkerYellow",
            "Flame",
            "FlameCloud",
            "FlameEOTLantern",
            "Flashlight",
            "FlashlightGlare",
            "FlashlightGlass",
            "FlashlightGlass_Lit",
            "FlaskContainer",
            "FlatbedSemiTrailerFactoryNew",
            "Flatcar_ContainerAcetyleneMedium",
            "Font Material",
            "Fragile_Tex_1",
            "FragileInfo_Tex_1",
            "FreightHaul_Tex_1",
            "FreightHaulInfo_Tex_1",
            "GadgetLight_Lit",
            "GasLeak",
            "GasWave",
            "Glass/LocoS282A_Windows_01d",
            "Glass/window_d",
            "GlassIndoors",
            "GlassSteamOil",
            "goat_mat",
            "GoldenShovel",
            "gooddog-plain-regular SDF Material",
            "HandheldGameConsole",
            "HandheldGameScreen",
            "Hanger",
            "hat",
            "Hazmat1_Tex_1",
            "Hazmat1Info_Tex_1",
            "Hazmat2_Tex_1",
            "Hazmat2Info_Tex_1",
            "Hazmat3_Tex_1",
            "Hazmat3Info_Tex_1",
            "HazmatPlacards",
            "Headlight",
            "headlights",
            "headlights_lit",
            "Headlights_NoEmissive",
            "headlights_red",
            "headlights_red_lit",
            "HeadlightsGlare",
            "HeatTemperatureIconAlpha",
            "Hidden/BlitCopy",
            "HighlightCab",
            "Hole",
            "hose_cock_icon",
            "IndicatorGlare",
            "InfraredThermometer",
            "InfraredThermometerGauge",
            "iron_age_cattle_col3",
            "IronOre",
            "ISOTank",
            "JobInventory",
            "JobOverviewInventory",
            "JobTrashBin",
            "JobTrashBin_LabelJobPaperDiscardArrow",
            "JunctionRemoteLCDLeft",
            "key",
            "Labels",
            "Lamp_LED",
            "Lamps_01",
            "Lamps_02",
            "Laser",
            "LCDBackgroundBlack",
            "LED_bar_01d",
            "LED_bar_01e",
            "LED_bar_02d",
            "LED_bar_02e",
            "LegacyFlameCloud",
            "LiberationSans SDF Material",
            "LightBar",
            "LightBarBlue",
            "LightBarCyan",
            "LightBarGreen",
            "LightBarOrange",
            "LightBarPurple",
            "LightBarRed",
            "LightBarWhite",
            "LightBarYellow",
            "LightEmission",
            "Lighter",
            "LighterSpark",
            "LiquidBlob",
            "LiquidSplat",
            "LiquidTrailLight",
            "LocoDE2_Body",
            "LocoDE2_Gauges",
            "LocoDE2_Interior",
            "LocoDE2_InteriorCab",
            "LocoDE2DVRTNew_Body",
            "LocoDE2DVRTNew_Interior",
            "LocoDE2DVRTNew_InteriorCab",
            "LocoDE2Exploded",
            "LocoDE2Exploded_Interior",
            "LocoDE2Exploded_InteriorCab",
            "LocoDE2MuseumAbandoned_Body",
            "LocoDE2MuseumAbandoned_Interior",
            "LocoDE2MuseumAbandoned_InteriorCab",
            "LocoDE2MuseumNew_Body",
            "LocoDE2MuseumNew_Interior",
            "LocoDE2MuseumNew_InteriorCab",
            "LocoDE2Primer_Body",
            "LocoDE2Primer_Interior",
            "LocoDE2Primer_InteriorCab",
            "LocoDE6_Body",
            "LocoDE6_Bogie",
            "LocoDE6_Engine",
            "LocoDE6_EngineNew",
            "LocoDE6_Gauges",
            "LocoDE6_Interior",
            "LocoDE6DVRTNew_Body",
            "LocoDE6DVRTNew_Interior",
            "LocoDE6Exploded",
            "LocoDE6Exploded_Bogie",
            "LocoDE6Exploded_Engine",
            "LocoDE6Exploded_Interior",
            "LocoDE6MuseumAbandoned_Body",
            "LocoDE6MuseumAbandoned_Bogie",
            "LocoDE6MuseumAbandoned_Interior",
            "LocoDE6MuseumNew_Body",
            "LocoDE6MuseumNew_Interior",
            "LocoDE6New_Bogie",
            "LocoDE6Primer_Body",
            "LocoDE6Primer_Bogie",
            "LocoDE6Primer_Interior",
            "LocoDH4_Body",
            "LocoDH4_Bogie",
            "LocoDH4_Gauges",
            "LocoDH4_Interior",
            "LocoDH4DVRTNew_Body",
            "LocoDH4DVRTNew_Interior",
            "LocoDH4Exploded",
            "LocoDH4Exploded_Bogie",
            "LocoDH4Exploded_Interior",
            "LocoDH4MuseumAbandoned_Body",
            "LocoDH4MuseumAbandoned_Bogie",
            "LocoDH4MuseumAbandoned_Interior",
            "LocoDH4MuseumNew_Body",
            "LocoDH4MuseumNew_Interior",
            "LocoDH4New_Bogie",
            "LocoDH4Primer_Body",
            "LocoDH4Primer_Bogie",
            "LocoDH4Primer_Interior",
            "LocoDM1P_Interior",
            "LocoDM1U_Gauges",
            "LocoDM1U-150_Body_01d",
            "LocoDM1U-150_Bogie",
            "LocoDM1U-150_Interior_TT_Metal",
            "LocoDM1U-150_Interior_TT_MetalDirty",
            "LocoDM1U-150_Interior_TT_MetalTrim",
            "LocoDM1U-150_Interior_TT_PaintBeige",
            "LocoDM1U-150_Interior_TT_PlasticWhite",
            "LocoDM1U-150_Interior_TT_RubberFloor",
            "LocoDM1U-150_Interior_TT_Seats",
            "LocoDM3_Body",
            "LocoDM3_Gauges",
            "LocoDM3_GearPatternPlaque",
            "LocoDM3_Interior",
            "LocoDM3DVRTNew_Body",
            "LocoDM3DVRTNew_Interior",
            "LocoDM3Exploded",
            "LocoDM3Exploded_Interior",
            "LocoDM3MuseumAbandoned_Body",
            "LocoDM3MuseumAbandoned_Interior",
            "LocoDM3MuseumNew_Body",
            "LocoDM3MuseumNew_Interior",
            "LocoDM3Primer_Body",
            "LocoDM3Primer_Interior",
            "LocoExploded",
            "LocoHandcar",
            "LocoMicroshunter_Body",
            "LocoMicroshunter_Gauges",
            "LocoMicroshunter_Interior",
            "LocoMicroshunterExploded",
            "LocoMicroshunterExploded_Interior",
            "LocoS060_Gauges",
            "LocoS060_Interior",
            "LocoS060_SightGlass",
            "LocoS060_TenderWater",
            "LocoS060DVRTNew_Body",
            "LocoS060DVRTNew_Interior",
            "LocoS060Exploded_Body",
            "LocoS060Exploded_Interior",
            "LocoS060Green_Body",
            "LocoS060MuseumAbandoned_Body",
            "LocoS060MuseumAbandoned_Interior",
            "LocoS060MuseumNew_Body",
            "LocoS060MuseumNew_Interior",
            "LocoS060Primer_Body",
            "LocoS060Primer_Interior",
            "LocoS282A_Body",
            "LocoS282A_Gauges",
            "LocoS282A_Interior",
            "LocoS282A_SightGlass",
            "LocoS282A_Windows",
            "LocoS282ADVRTNew_Body",
            "LocoS282ADVRTNew_Interior",
            "LocoS282AExploded_Body",
            "LocoS282AExploded_Interior",
            "LocoS282AMuseumAbandoned_Body",
            "LocoS282AMuseumAbandoned_Interior",
            "LocoS282AMuseumNew_Body",
            "LocoS282AMuseumNew_Interior",
            "LocoS282APrimer_Body",
            "LocoS282APrimer_Interior",
            "LocoS282B_Body",
            "LocoS282B_TenderWater",
            "LocoS282BDVRTNew_Body",
            "LocoS282BMuseumAbandoned_Body",
            "LocoS282BMuseumNew_Body",
            "LocoS282BPrimer_Body",
            "LogisticalHaul_Tex_1",
            "LogisticalHaulInfo_Tex_1",
            "LogsSteelWood",
            "ManualService_Tex_1",
            "ManualServiceInfo_Tex_1",
            "MapCase",
            "MapItem",
            "MapSchematicPreview",
            "MapTMP",
            "MapUI_Caboose",
            "MapUI_Home",
            "MapUI_Location",
            "MapUI_LocationBorderBlack",
            "MapUI_Player",
            "MapUI_ServiceCoal",
            "MapUI_ServiceDiesel",
            "MapUI_ServiceElectricCharger",
            "MapUI_ServiceRepair",
            "MapUI_Shop",
            "MapUI_Train",
            "MB_LinoleumGray",
            "MB_Metal",
            "MB_MetalBrown",
            "MB_PaintFlatYellow",
            "MB_TrimMetal",
            "MB_TrimMetal2",
            "MetalBrushed",
            "Military1_Tex_1",
            "Military1Info_Tex_1",
            "Military2_Tex_1",
            "Military2Info_Tex_1",
            "Military3_Tex_1",
            "Military3Info_Tex_1",
            "MiningTruckNew",
            "MiningTruckWheels",
            "Missile",
            "Money",
            "MonitorDisplay",
            "Mud_pig_v3_mat",
            "MultipleUnit_Tex_1",
            "MultipleUnitInfo_Tex_1",
            "MuseumCS_Tex_1",
            "NeedleRed",
            "NewWindowDropletsGrabPassGrabber",
            "NotoSans-Regular Atlas Material",
            "Office",
            "Oiler",
            "OilLantern",
            "OilLanternGlass",
            "OilLanternGlass_NoEmissive",
            "OutlineSeeThrough",
            "OverheatingProtection",
            "PaintCan",
            "PaintCan_DVRT",
            "PaintCan_Relic",
            "PaintCan_Sand",
            "PaintedMetal",
            "PaintMatteGray",
            "PaintSprayer",
            "pallet",
            "PitchBlack",
            "PlasticMatteBlack",
            "PlasticMatteRed",
            "PlasticMatteYellow",
            "PlasticRoughBlack",
            "PlasticRoughGray",
            "PlayIconAlpha",
            "RailMed",
            "RailNew",
            "RailOld",
            "RearlightsGlare",
            "RectangleBlobAO_LOD0",
            "RectangleBlobAO_LOD1",
            "RED_display",
            "RegisterScanner",
            "RemoteController_d",
            "RemoteControllerCharger",
            "Renegade_Mistress SDF Material",
            "ReportInventory",
            "Rope",
            "RouteMap",
            "SaniTrixieSans SDF Material",
            "SaniTrixieSans_SDF_Surface",
            "ScannerLaser",
            "ScrapMetal",
            "ScrapWood",
            "SH282_Tex_1",
            "SH282Info_Tex_1",
            "Shelf",
            "ShovelExpert",
            "ShovelShort",
            "Shunting_Tex_1",
            "ShuntingInfo_Tex_1",
            "SleeperNew",
            "SleeperOld",
            "SmokeboxHolesAlpha",
            "SmokeCloudDense",
            "SmokeDenseWhite",
            "SmokeWhiteFull",
            "SolderingGun",
            "spark",
            "spark_wheelslip",
            "SplashScreen-Background",
            "SplashScreen-Foreground",
            "Sprites/Mask",
            "Sprites-Default",
            "SteamOil",
            "StickyTapeRubber",
            "Stopwatch",
            "StopwatchDial",
            "SunVisor",
            "SunVisorGlass",
            "swan_goose_mat",
            "Switches",
            "SwitchSetter",
            "SwivelLight",
            "SwivelLightGlare",
            "TankMilitary",
            "tire",
            "TrainCarAccessories",
            "TrainCarAccessoriesInterior",
            "TrainDriveInfo_Tex_1",
            "TrainDriver_Tex_1",
            "TrainLength1_Tex_1",
            "TrainLength1Info_Tex_1",
            "TrainLength2_Tex_1",
            "TrainLength2Info_Tex_1",
            "TramNewBlue",
            "TramNewOrange",
            "TruckMilitary",
            "TT_CP_Curtains",
            "TT_CP_Metal",
            "TT_CP_MetalDirty",
            "TT_CP_MetalTrim",
            "TT_CP_PaintBeige",
            "TT_CP_PlasticWhite",
            "TT_CP_RubberFloor",
            "TT_CP_Seats",
            "TT_TramNew_Metal",
            "TT_TramNew_MetalTrim",
            "TT_TramNew_PaintBeige",
            "TT_TramNew_PlasticRed",
            "TT_TramNew_PlasticWhite",
            "TT_TramNew_Rubber",
            "TutorialSummary",
            "VehicleWheels",
            "wallet_brown",
            "WindowDropletMaterialHigh",
            "windows_broken",
            "windows_broken_scalex10",
            "WirelessMUController",
            "WirelessMUController_Logo",
            "WispyCloudWhite",
            "WoodChips"
        };
        public static string[] MaterialNames => s_materialNames;
    }
}
