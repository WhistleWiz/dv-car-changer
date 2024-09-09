namespace CarChanger.Common
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
            public const float RadiusDE2 = 0.5025f;
            public const float RadiusDE6 = 0.5335f;
            public const float RadiusDH4 = 0.535f;
            public const float RadiusHandcar = 0.290f;
            public const float RadiusMicroshunter = 0.325f;
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
    }
}
