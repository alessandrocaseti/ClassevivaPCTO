﻿using Windows.ApplicationModel.Resources;

namespace ClassevivaPCTO.Helpers
{
    internal static class ResourceExtensions
    {
        private static ResourceLoader _resLoader = new ResourceLoader();

        public static string GetLocalized(this string resourceKey)
        {
            return _resLoader.GetString(resourceKey);
        }

        public static string GetLocalized(this string resourceKey, string tag)
        {
            resourceKey += tag;
            return GetLocalized(resourceKey);
        }
    }
}