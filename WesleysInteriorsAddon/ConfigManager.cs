using BepInEx.Configuration;
using System.Collections.Generic;
using System.Reflection;

namespace com.github.zehsteam.WesleysInteriorsAddon;

internal class ConfigManager
{
    // General Settings
    private ConfigEntry<bool> ExtendedLoggingCfg;

    // Toystore Settings
    private ConfigEntry<float> ToystoreNutcrackerMinInitialActivateTimeCfg;
    private ConfigEntry<float> ToystoreNutcrackerMaxInitialActivateTimeCfg;
    private ConfigEntry<float> ToystoreNutcrackerMinActivateTimeCfg;
    private ConfigEntry<float> ToystoreNutcrackerMaxActivateTimeCfg;

    // General Settings
    internal bool ExtendedLogging { get { return ExtendedLoggingCfg.Value; } set { ExtendedLoggingCfg.Value = value; } }

    // Toystore Settings
    internal float ToystoreNutcrackerMinInitialActivateTime { get { return ToystoreNutcrackerMinInitialActivateTimeCfg.Value; } set { ToystoreNutcrackerMinInitialActivateTimeCfg.Value = value; } }
    internal float ToystoreNutcrackerMaxInitialActivateTime { get { return ToystoreNutcrackerMaxInitialActivateTimeCfg.Value; } set { ToystoreNutcrackerMaxInitialActivateTimeCfg.Value = value; } }
    internal float ToystoreNutcrackerMinActivateTime { get { return ToystoreNutcrackerMinActivateTimeCfg.Value; } set { ToystoreNutcrackerMinActivateTimeCfg.Value = value; } }
    internal float ToystoreNutcrackerMaxActivateTime { get { return ToystoreNutcrackerMaxActivateTimeCfg.Value; } set { ToystoreNutcrackerMaxActivateTimeCfg.Value = value; } }

    public ConfigManager()
    {
        BindConfigs();
        ClearUnusedEntries();
    }

    private void BindConfigs()
    {
        ConfigFile configFile = Plugin.Instance.Config;

        // General Settings
        ExtendedLoggingCfg = configFile.Bind(
            new ConfigDefinition("General Settings", "ExtendedLogging"),
            false,
            new ConfigDescription("Enable extended logging.")
        );

        // Toystore Settings
        ToystoreNutcrackerMinInitialActivateTimeCfg = configFile.Bind(
            new ConfigDefinition("Toystore Settings", "ToystoreNutcrackerMinInitialActivateTime"),
            15f,
            new ConfigDescription("The minimum amount of time in seconds before the Nutcrackers start activating. (After pulling the apparatus)")
        );
        ToystoreNutcrackerMaxInitialActivateTimeCfg = configFile.Bind(
            new ConfigDefinition("Toystore Settings", "ToystoreNutcrackerMaxInitialActivateTime"),
            30f,
            new ConfigDescription("The maximum amount of time in seconds before the Nutcrackers start activating. (After pulling the apparatus)")
        );
        ToystoreNutcrackerMinActivateTimeCfg = configFile.Bind(
            new ConfigDefinition("Toystore Settings", "ToystoreNutcrackerMinActivateTime"),
            10f,
            new ConfigDescription("The minimum amount of time in seconds between Nutcracker activation. (After pulling the apparatus)")
        );
        ToystoreNutcrackerMaxActivateTimeCfg = configFile.Bind(
            new ConfigDefinition("Toystore Settings", "ToystoreNutcrackerMaxActivateTime"),
            15f,
            new ConfigDescription("The maximum amount of time in seconds between Nutcracker activation. (After pulling the apparatus)")
        );
    }

    private void ClearUnusedEntries()
    {
        ConfigFile configFile = Plugin.Instance.Config;

        // Normally, old unused config entries don't get removed, so we do it with this piece of code. Credit to Kittenji.
        PropertyInfo orphanedEntriesProp = configFile.GetType().GetProperty("OrphanedEntries", BindingFlags.NonPublic | BindingFlags.Instance);
        var orphanedEntries = (Dictionary<ConfigDefinition, string>)orphanedEntriesProp.GetValue(configFile, null);
        orphanedEntries.Clear(); // Clear orphaned entries (Unbinded/Abandoned entries)
        configFile.Save(); // Save the config file to save these changes
    }
}
