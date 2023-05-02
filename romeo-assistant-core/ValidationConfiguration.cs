using Microsoft.Extensions.Configuration;
using romeo_assistant_core.Models.Configuration;

namespace romeo_assistant_core
{
    public static class ValidationConfiguration
    {
        public static void Validate(IConfigurationRoot? secrets, AppSettings appSettings)
        {
            TrySetSecrets(secrets, appSettings);

            var settings = new List<(string settingValue, string errorMessage)>
            {
                (appSettings.OpenAi?.ApiKey, "Missing OpenIA api key , create one over here https://platform.openai.com/account/api-keys and update the config.json file")!,
                (appSettings.Maytapi?.ProductId, "Missing MayTapi Product Id, create one over here https://console.maytapi.com/developers/token and update the config.json file")!,
                (appSettings.Maytapi?.ApiKey, "Missing MayTapi Token Id, create one over here https://console.maytapi.com/developers/token and update the config.json file")!,
                (appSettings.Supabase?.Url, "Missing supaBase url (Host), See documentation https://supabase.com/docs/guides/database/connecting-to-postgres#connecting-with-ssl")!,
                (appSettings.Supabase?.Key, "Missing supaBase Key, See documentation https://docs.draftbit.com/docs/supabase#get-the-restful-endpoint-and-project-api-key")!
            };

            settings.ForEach(ValidateSetting);
        }

        private static void TrySetSecrets(IConfigurationRoot? secrets, AppSettings? appSettings)
        {
            if (secrets == null || appSettings == null)
            {
                return;
            }

            var secretSettings = new Dictionary<string, Action<string>>
            {
                { "OPENAI_API_KEY", value => appSettings.OpenAi!.ApiKey = value },
                { "MAYTAPI_PRODUCT_ID", value => appSettings.Maytapi!.ProductId = value },
                { "MAYTAPI_API_KEY", value => appSettings.Maytapi!.ApiKey = value },
                { "SUPABASE_URL", value => appSettings.Supabase!.Url = value },
                { "SUPABASE_KEY", value => appSettings.Supabase!.Key = value }
            };

            foreach (var setting in secretSettings)
            {
                var secretValue = secrets[setting.Key];
                if (!string.IsNullOrEmpty(secretValue))
                {
                    setting.Value(secretValue);
                }
            }
        }

        private static void ValidateSetting((string settingValue, string errorMessage) setting)
        {
            if (string.IsNullOrEmpty(setting.settingValue))
            {
                throw new InvalidOperationException(setting.errorMessage);
            }
        }
    }
}
