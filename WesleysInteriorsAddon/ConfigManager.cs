using BepInEx.Configuration;
using System.Collections.Generic;
using System.Reflection;

namespace com.github.zehsteam.WesleysInteriorsAddon;

internal class ConfigManager
{
    // General Settings
    public ConfigEntry<bool> ExtendedLogging;

    // Toy Store Nutcracker Settings
    public ConfigEntry<bool> ToyStoreNutcracker_Enabled;
    public ConfigEntry<int> ToyStoreNutcracker_MaxSpawnCount;
    public ConfigEntry<bool> ToyStoreNutcracker_SpawnNearPlayers;
    public ConfigEntry<float> ToyStoreNutcracker_MinInitialSpawnDelay;
    public ConfigEntry<float> ToyStoreNutcracker_MaxInitialSpawnDelay;
    public ConfigEntry<float> ToyStoreNutcracker_MinSpawnDelay;
    public ConfigEntry<float> ToyStoreNutcracker_MaxSpawnDelay;

    public ConfigManager()
    {
        BindConfigs();
        ClearUnusedEntries();
    }

    private void BindConfigs()
    {
        ConfigHelper.SkipAutoGen();

        // General Settings
        ExtendedLogging = ConfigHelper.Bind("General Settings", "ExtendedLogging", defaultValue: false, requiresRestart: false, "Enable extended logging.");

        // Toy Store Nutcracker Settings
        string section1 = "Toy Store Nutcracker Settings";
        ToyStoreNutcracker_Enabled =              ConfigHelper.Bind(section1, "Enabled",              defaultValue: true, requiresRestart: false, "If enabled, the Nutcracker statues will slowly awaken after the apparatus has been pulled.");
        ToyStoreNutcracker_MaxSpawnCount =        ConfigHelper.Bind(section1, "MaxSpawnCount",        defaultValue: 50,   requiresRestart: false, "The max amount of Nutcracker statues that can awaken.");
        ToyStoreNutcracker_SpawnNearPlayers =     ConfigHelper.Bind(section1, "SpawnNearPlayers",     defaultValue: true, requiresRestart: false, "If enabled, the Nutcracker statues will awaken near players.");
        ToyStoreNutcracker_MinInitialSpawnDelay = ConfigHelper.Bind(section1, "MinInitialSpawnDelay", defaultValue: 5f,   requiresRestart: false, "The min delay in seconds before the Nutcracker statues start to awaken after the apparatus has been pulled.");
        ToyStoreNutcracker_MaxInitialSpawnDelay = ConfigHelper.Bind(section1, "MaxInitialSpawnDelay", defaultValue: 20f,  requiresRestart: false, "The max delay in seconds before the Nutcracker statues start to awaken after the apparatus has been pulled.");
        ToyStoreNutcracker_MinSpawnDelay =        ConfigHelper.Bind(section1, "MinSpawnDelay",        defaultValue: 5f,   requiresRestart: false, "The min delay in seconds between Nutcracker statue activations.");
        ToyStoreNutcracker_MaxSpawnDelay =        ConfigHelper.Bind(section1, "MaxSpawnDelay",        defaultValue: 25f,  requiresRestart: false, "The max delay in seconds between Nutcracker statue activations.");
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
