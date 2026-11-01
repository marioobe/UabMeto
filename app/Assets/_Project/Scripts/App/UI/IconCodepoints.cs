using System.Collections.Generic;

namespace UabMeto.App.UI
{
    /// <summary>
    /// Peta nama ikon ke codepoint Material Symbols. Hanya ikon yang dipakai
    /// yang didaftarkan; sumber codepoint ada di
    /// Assets/ThirdParty/Icons/codepoints.txt.
    /// </summary>
    public static class IconCodepoints
    {
        private static readonly Dictionary<string, int> Map = new Dictionary<string, int>
        {
            { "home", 0xE9B2 },
            { "menu_book", 0xEA19 },
            { "leaderboard", 0xF20C },
            { "person", 0xF0D3 },
            { "school", 0xE80C },
            { "wifi_off", 0xE648 },
            { "cloud_off", 0xE2C1 },
            { "local_fire_department", 0xEF55 },
            { "bolt", 0xEA0B },
            { "favorite", 0xE87E },
            { "arrow_back", 0xE5C4 },
            { "arrow_forward", 0xE5C8 },
            { "chevron_right", 0xE5CC },
            { "volume_up", 0xE050 },
            { "play_arrow", 0xE037 },
            { "pause", 0xE034 },
            { "mic", 0xE31D },
            { "check_circle", 0xF0BE },
            { "cancel", 0xE888 },
            { "close", 0xE5CD },
            { "lock", 0xE899 },
            { "lock_open", 0xE898 },
            { "refresh", 0xE5D5 },
            { "settings", 0xE8B8 },
            { "logout", 0xE9BA },
            { "edit", 0xF097 },
            { "verified", 0xEF76 },
            { "schedule", 0xEFD6 },
            { "translate", 0xE8E2 },
            { "quiz", 0xF04C },
            { "workspace_premium", 0xE7AF },
            { "stars", 0xE8D0 },
            { "info", 0xE88E },
            { "warning", 0xF083 },
            { "error", 0xF8B6 },
            { "search", 0xEF7A },
            { "notifications", 0xE7F5 },
            { "language", 0xEA07 },
            { "offline_bolt", 0xE932 },
            { "waving_hand", 0xE766 },
            { "badge", 0xEA67 },
            { "family_restroom", 0xF1A2 },
            { "checklist", 0xE6B1 },
            { "restaurant", 0xE56C },
            { "chair", 0xEFED },
            { "map", 0xE55B },
            { "shopping_basket", 0xE8CB },
            { "texture", 0xE421 },
            { "health_and_safety", 0xE1D5 },
            { "book", 0xE86E },
            { "check", 0xE668 },
            { "radio_button_unchecked", 0xE836 },
            { "help_outline", 0xE8FD }
        };

        public static string Glyph(string name)
        {
            if (string.IsNullOrWhiteSpace(name) || !Map.TryGetValue(name, out var codepoint))
            {
                codepoint = Map["help_outline"];
            }

            return char.ConvertFromUtf32(codepoint);
        }

        public static bool Has(string name)
        {
            return !string.IsNullOrWhiteSpace(name) && Map.ContainsKey(name);
        }
    }
}
