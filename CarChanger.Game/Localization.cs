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
        private const string NoModificationsKey = "carchanger/incomp_mod";

        public static string Enable => LocalizationAPI.L(EnableKey);
        public static string Disable => LocalizationAPI.L(DisableKey);
        public static string Override => LocalizationAPI.L(OverrideKey);
        public static string RadioBegin => LocalizationAPI.L(RadioBeginKey);
        public static string IncompatibleModification => LocalizationAPI.L(IncompatibleModificationKey);
        public static string NoModifications => LocalizationAPI.L(NoModificationsKey);

        internal static void Inject()
        {
            //CarChangerMod.Translations.AddTranslation(EnableKey, DVLanguage.English, "Enable?");
            //CarChangerMod.Translations.AddTranslation(DisableKey, DVLanguage.English, "Disable?");
            //CarChangerMod.Translations.AddTranslation(OverrideKey, DVLanguage.English, "Override?");

            CarChangerMod.Translations.AddTranslations(EnableKey, new[]
            {
                new TranslationItem(DVLanguage.English, "Enable?"),
                new TranslationItem(DVLanguage.Portuguese, "Activar?"),
                new TranslationItem(DVLanguage.Portuguese_BR, "Activar?"),
                new TranslationItem(DVLanguage.French, "Activer?"),
                new TranslationItem(DVLanguage.Spanish, "Habilitar?"),
                new TranslationItem(DVLanguage.Italian, "Abilitare?"),
            });

            CarChangerMod.Translations.AddTranslations(DisableKey, new[]
            {
                new TranslationItem(DVLanguage.English, "Disable?"),
                new TranslationItem(DVLanguage.Portuguese, "Desactivar?"),
                new TranslationItem(DVLanguage.Portuguese_BR, "Desactivar?"),
                new TranslationItem(DVLanguage.French, "Désactiver?"),
                new TranslationItem(DVLanguage.Spanish, "Desabilitar?"),
                new TranslationItem(DVLanguage.Italian, "Disabilitare?"),
            });

            CarChangerMod.Translations.AddTranslations(OverrideKey, new[]
            {
                new TranslationItem(DVLanguage.English, "Override?"),
                new TranslationItem(DVLanguage.Portuguese, "Substituir?"),
                new TranslationItem(DVLanguage.Portuguese_BR, "Substituir?"),
                new TranslationItem(DVLanguage.French, "Remplacer?"),
                new TranslationItem(DVLanguage.Spanish, "Reemplazar?"),
                new TranslationItem(DVLanguage.Italian, "Sostituire?"),
            });

            CarChangerMod.Translations.AddTranslation(RadioBeginKey, DVLanguage.English,
                "Point at a car to begin");
            CarChangerMod.Translations.AddTranslation(IncompatibleModificationKey, DVLanguage.English,
                "Modification is incompatible with others");
            CarChangerMod.Translations.AddTranslation(NoModificationsKey, DVLanguage.English,
                "No changes available");
        }

        public static string GetLocalizedName(TrainCarLivery livery) => LocalizationAPI.L(livery.localizationKey);

        public static string GetLocalizedName(ModelConfig config) => LocalizationAPI.L(config.LocalizationKey);
    }
}
