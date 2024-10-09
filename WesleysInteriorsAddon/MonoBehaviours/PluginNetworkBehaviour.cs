using Unity.Netcode;
using UnityEngine;

namespace com.github.zehsteam.WesleysInteriorsAddon.MonoBehaviours;

internal class PluginNetworkBehaviour : NetworkBehaviour
{
    public static PluginNetworkBehaviour Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    [ClientRpc]
    public void HideNutcrackerStatueClientRpc(Vector3 position)
    {
        if (NetworkUtils.IsServer) return;

        ToyStore.HideNutcrackerStatueOnLocalClient(position);
    }
}
