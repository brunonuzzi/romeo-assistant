using Microsoft.Extensions.Configuration;
using romeo_assistant_core.Models.Configuration;

namespace romeo_assistant_core
{
    public class ValidationConfiguration
    {
        public static void Validate(IConfigurationRoot? secrets,AppSettings appSettings)
        {
            TrySetSecrets(secrets, appSettings);

            if (string.IsNullOrEmpty(appSettings.OpenAi?.ApiKey))
            {
                throw new InvalidOperationException("Missing OpenIA api key , create one over here https://platform.openai.com/account/api-keys and update the config.json file");
            }

            if (string.IsNullOrEmpty(appSettings.Maytapi?.ProductId))
            {
                throw new InvalidOperationException("Missing MayTapi Product Id, create one over here https://console.maytapi.com/developers/token and update the config.json file");
            }

            if (string.IsNullOrEmpty(appSettings.Maytapi?.ApiKey))
            {
                throw new InvalidOperationException("Missing MayTapi Token Id, create one over here https://console.maytapi.com/developers/token and update the config.json file");
            }

            if (string.IsNullOrEmpty(appSettings.Supabase?.Url))
            {
                throw new InvalidOperationException("Missing supaBase url (Host), See documentation https://supabase.com/docs/guides/database/connecting-to-postgres#connecting-with-ssl");
            }

            if (string.IsNullOrEmpty(appSettings.Supabase?.Key))
            {
                throw new InvalidOperationException("Missing supaBase Key, See documentation https://docs.draftbit.com/docs/supabase#get-the-restful-endpoint-and-project-api-key");
            }
        }

        private static void TrySetSecrets(IConfigurationRoot? secrets, AppSettings? appSettings)
        {
            if (secrets == null || appSettings == null)
            {
                return;
            }
            
            if (!string.IsNullOrEmpty(secrets["OPENAI_API_KEY"]))
            {
                appSettings.OpenAi!.ApiKey = secrets["OPENAI_API_KEY"];
            }
            if (!string.IsNullOrEmpty(secrets["MAYTAPI_PRODUCT_ID"]))
            {
                appSettings.Maytapi!.ProductId = secrets["MAYTAPI_PRODUCT_ID"];
            }
            if (!string.IsNullOrEmpty(secrets["MAYTAPI_API_KEY"]))
            {
                appSettings.Maytapi!.ApiKey = secrets["MAYTAPI_API_KEY"];
            }
            if (!string.IsNullOrEmpty(secrets["SUPABASE_URL"]))
            {
                appSettings.Supabase!.Url = secrets["SUPABASE_URL"];
            }
            if (!string.IsNullOrEmpty(secrets["SUPABASE_KEY"]))
            {
                appSettings.Supabase!.Key = secrets["SUPABASE_KEY"];
            }
        }
    }
}
