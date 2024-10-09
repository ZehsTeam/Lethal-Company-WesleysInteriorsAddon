using com.github.zehsteam.WesleysInteriorsAddon.MonoBehaviours;
using UnityEngine;

namespace com.github.zehsteam.WesleysInteriorsAddon;

internal static class Content
{
    // Prefabs
    public static GameObject NetworkHandlerPrefab;

    public static void Load()
    {
        LoadAssetsFromAssetBundle();
    }

    private static void LoadAssetsFromAssetBundle()
    {
        try
        {
            AssetBundle assetBundle = LoadAssetBundle("wesleysinteriorsaddon_assets");

            // Prefabs
            NetworkHandlerPrefab = assetBundle.LoadAsset<GameObject>("NetworkHandler");
            NetworkHandlerPrefab.AddComponent<PluginNetworkBehaviour>();

            Plugin.logger.LogInfo("Successfully loaded assets from AssetBundle!");
        }
        catch (System.Exception e)
        {
            Plugin.logger.LogError($"Failed to load assets from AssetBundle.\n\n{e}");
        }
    }

    private static AssetBundle LoadAssetBundle(string fileName)
    {
        var dllFolderPath = System.IO.Path.GetDirectoryName(Plugin.Instance.Info.Location);
        var assetBundleFilePath = System.IO.Path.Combine(dllFolderPath, fileName);
        return AssetBundle.LoadFromFile(assetBundleFilePath);
    }
}
