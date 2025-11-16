using FishNet;

using FishNet.Managing;
using FishNet.Transporting;
using FishNet.Object;
using UnityEngine;

public class NetworkInit : MonoBehaviour
{
    public NetworkObject lobbyManagerPrefab;

    private void Start()
    {
        InstanceFinder.ServerManager.OnServerConnectionState += HandleServerConnectionState;
        Debug.Log($"[NetworkInit] Is server: {InstanceFinder.IsServerStarted}");
    }

    private void HandleServerConnectionState(ServerConnectionStateArgs args)
    {
        if (args.ConnectionState == LocalConnectionState.Started)
        {
            Debug.Log("[NetworkInit] Server started. Spawning LobbyManager.");

            if (lobbyManagerPrefab == null)
            {
                Debug.LogError("[NetworkInit] No LobbyManager prefab assigned!");
                return;
            }

            var obj = Instantiate(lobbyManagerPrefab);
            InstanceFinder.ServerManager.Spawn(obj);

            Debug.Log("[NetworkInit] LobbyManager instantiated and spawned.");
        }
    }

    private void OnDestroy()
    {
        if (InstanceFinder.ServerManager != null)
            InstanceFinder.ServerManager.OnServerConnectionState -= HandleServerConnectionState;
    }
}
