﻿using Microsoft.Maui.ApplicationModel;

namespace EliasLogAnalyzer.MAUI.Resources
{
    public sealed class Theme
    {
        public static readonly Theme Dark = new(AppTheme.Dark, "Night Mode");
        public static readonly Theme Light = new(AppTheme.Light, "Day Mode");
        public static readonly Theme System = new(AppTheme.Unspecified, "Follow System");

        public static List<Theme> AvailableThemes { get; } = [Dark, Light, System];

        public AppTheme AppTheme { get; }
        public string DisplayName { get; }

        private Theme(AppTheme theme, string displayName)
        {
            AppTheme = theme;
            DisplayName = displayName;
        }
    }
}
