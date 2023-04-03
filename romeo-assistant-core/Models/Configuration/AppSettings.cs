﻿namespace romeo_assistant_core.Models.Configuration
{
    public class AppSettings
    {
        public OpenAiSettings? OpenAi { get; set; }
        public MaytapiSettings? Maytapi { get; set; }
        public RomeoSetupSettings? RomeoSetup { get; set; }
        public SupabaseSettings? Supabase { get; set; }


        public class OpenAiSettings
        {
            public string? ApiKey { get; set; }
        }

        public class MaytapiSettings
        {
            public string? ApiKey { get; set; }
            public string? ProductId { get; set; }
            public string? BaseApiUrl { get; set; }
        }

        public class RomeoSetupSettings
        {
            public string? BasicPromptMessage { get; set; }
            public string? PromptResetMessage { get; set; }
            public string? ResetPromptCommand { get; set; }
            public string? ResetPromptSuccess { get; set; }
            public int TokenMaxSize { get; set; }
        }

        public class SupabaseSettings
        {
            public string? Url { get; set; }
            public string? Key { get; set; }
        }
    }
}