using CarChanger.Common;
using DV.Localization;
using DV.ThingTypes;
using DVLangHelper.Data;

namespace CarChanger.Game
{
    public static class Localization
    {
        private const string EnableKey = "carchanger/enable";
        private const string DisableKey = "carchanger/disable";
        private const string OverrideKey = "carchanger/override";
        private const string RadioBeginKey = "carchanger/radio_begin";
        private const string IncompatibleModificationKey = "carchanger/incomp_mod";

        public static string Enable => LocalizationAPI.L(EnableKey);
        public static string Disable => LocalizationAPI.L(DisableKey);
        public static string Override => LocalizationAPI.L(OverrideKey);
        public static string RadioBegin => LocalizationAPI.L(RadioBeginKey);
        public static string IncompatibleModification => LocalizationAPI.L(IncompatibleModificationKey);

        internal static void Inject()
        {
            CarChangerMod.Translations.AddTranslation(EnableKey, DVLanguage.English, "Enable?");
            CarChangerMod.Translations.AddTranslation(DisableKey, DVLanguage.English, "Disable?");
            CarChangerMod.Translations.AddTranslation(OverrideKey, DVLanguage.English, "Override?");

            CarChangerMod.Translations.AddTranslation(RadioBeginKey, DVLanguage.English,
                "Point at a car to begin");
            CarChangerMod.Translations.AddTranslation(IncompatibleModificationKey, DVLanguage.English,
                "Modification is incompatible with others");
        }

        public static string GetLocalizedName(TrainCarLivery livery) => LocalizationAPI.L(livery.localizationKey);

        public static string GetLocalizedName(ModelConfig config) => LocalizationAPI.L(config.LocalizationKey);
    }
}
