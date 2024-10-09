﻿using BepInEx.Configuration;
using com.github.zehsteam.WesleysInteriorsAddon.Dependencies;
using System;
using UnityEngine;

namespace com.github.zehsteam.WesleysInteriorsAddon;

internal static class ConfigHelper
{
    public static void SetModIcon(Sprite sprite)
    {
        if (LethalConfigProxy.Enabled)
        {
            LethalConfigProxy.SetModIcon(sprite);
        }
    }

    public static void SetModDescription(string description)
    {
        if (LethalConfigProxy.Enabled)
        {
            LethalConfigProxy.SetModDescription(description);
        }
    }

    public static void SkipAutoGen()
    {
        if (LethalConfigProxy.Enabled)
        {
            LethalConfigProxy.SkipAutoGen();
        }
    }

    public static ConfigEntry<T> Bind<T>(string section, string key, T defaultValue, bool requiresRestart, string description, AcceptableValueBase acceptableValues = null, Action<T> settingChanged = null, ConfigFile configFile = null)
    {
        configFile ??= Plugin.Instance.Config;

        var configEntry = acceptableValues == null
            ? configFile.Bind(section, key, defaultValue, description)
            : configFile.Bind(section, key, defaultValue, new ConfigDescription(description, acceptableValues));

        if (settingChanged != null)
        {
            configEntry.SettingChanged += (object sender, EventArgs e) => settingChanged?.Invoke(configEntry.Value);
        }

        if (LethalConfigProxy.Enabled)
        {
            if (acceptableValues == null)
            {
                LethalConfigProxy.AddConfig(configEntry, requiresRestart);
            }
            else
            {
                LethalConfigProxy.AddConfigSlider(configEntry, requiresRestart);
            }
        }

        return configEntry;
    }

    public static void AddButton(string section, string name, string description, string buttonText, Action callback)
    {
        if (LethalConfigProxy.Enabled)
        {
            LethalConfigProxy.AddButton(section, name, description, buttonText, callback);
        }
    }
}
