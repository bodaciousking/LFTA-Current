using UnityEngine;
using FishNet.Managing;
using FishNet.Object;
using FishNet.Transporting;
using FishNet.Connection;
using FishNet.Broadcast;
using System.Linq;
using System.Collections;

public class LobbySpawner : MonoBehaviour
{
    [SerializeField] private NetworkManager networkManager;
    [SerializeField] private NetworkObject lobbyPlayerPrefab;
    LobbyManager lM;
    private NetworkObject hostLobbyPlayer;
    private void Start()
    {
        lM = LobbyManager.instance;
    }
    private void Awake()
    {
        if (networkManager == null)
            networkManager = GetComponent<NetworkManager>();
    }

    private void OnEnable()
    {
        networkManager.ServerManager.OnRemoteConnectionState += HandleRemoteConnectionState;
        networkManager.ServerManager.OnServerConnectionState += HandleServerConnectionState;
        if (networkManager != null)
        {
            networkManager.ServerManager.OnRemoteConnectionState += HandleRemoteConnectionState;
        }
        networkManager.ClientManager.OnClientConnectionState += HandleLocalClientReady;
    }

    private void OnDisable()
    {
        networkManager.ServerManager.OnRemoteConnectionState -= HandleRemoteConnectionState;
        networkManager.ServerManager.OnServerConnectionState -= HandleServerConnectionState;
        if (networkManager != null)
        {
            networkManager.ServerManager.OnRemoteConnectionState -= HandleRemoteConnectionState;
        }
        networkManager.ClientManager.OnClientConnectionState -= HandleLocalClientReady;
    }
    private void HandleLocalClientReady(ClientConnectionStateArgs args)
    {
        if (args.ConnectionState == LocalConnectionState.Started && networkManager.IsServerStarted)
        {
            Debug.Log("[LobbySpawner] Host client is ready. Using stored reference to broadcast.");

            if (hostLobbyPlayer != null)
            {
                lM.InvokeClientSpawnBroadcast(
                    new PlayerSpawnedBroadcast { Player = hostLobbyPlayer });
            }
            else
            {
                Debug.LogWarning("[LobbySpawner] HostLobbyPlayer is still null!");
            }
        }
    }
    private void HandleServerConnectionState(ServerConnectionStateArgs args)
    {
        if (args.ConnectionState == LocalConnectionState.Started &&
            networkManager.IsClientStarted) // This means we're hosting
        {
            Debug.Log("[LobbySpawner] Hosting started. Spawning host LobbyPlayer.");

            NetworkObject obj = Instantiate(lobbyPlayerPrefab);
            networkManager.ServerManager.Spawn(obj, networkManager.ClientManager.Connection);

            // Broadcast for remote clients
            networkManager.ServerManager.Broadcast(new PlayerSpawnedBroadcast { Player = obj });

            // Manually simulate broadcast for the host client
            lM.InvokeClientSpawnBroadcast(new PlayerSpawnedBroadcast { Player = obj }); 
            Debug.Log("[LobbySpawner] Manually invoked client broadcast on host.");
        }
    }
    private void HandleRemoteConnectionState(NetworkConnection conn, RemoteConnectionStateArgs args)
    {
        if (args.ConnectionState == RemoteConnectionState.Started)
        {
            Debug.Log($"[LobbySpawner] Client {conn.ClientId} connected. Spawning LobbyPlayer.");

            NetworkObject obj = Instantiate(lobbyPlayerPrefab);
            //obj.gameObject.GetComponent<LobbyPlayer>().SetPlayerName(lID)
            networkManager.ServerManager.Spawn(obj, conn);
            networkManager.ServerManager.Broadcast(new PlayerSpawnedBroadcast { Player = obj });


            Debug.Log("[LobbyManager] Local LobbyPlayer spawned — sending name.");
            StartCoroutine(SendNameAfterSpawn());

            if (networkManager.IsServerStarted && networkManager.IsClientStarted)
            {
                Debug.Log("[LobbySpawner] Host mode — storing hostLobbyPlayer.");
                hostLobbyPlayer = obj;
            }
        }
    }
    private IEnumerator SendNameAfterSpawn()
    {
        // Wait to ensure ownership and network readiness
        yield return new WaitForSeconds(0.25f);

        string name = LobbyID.instance.confirmedName;

        LobbyPlayer lp = FindObjectsByType<LobbyPlayer>(FindObjectsSortMode.None)
            .FirstOrDefault(p => p.IsOwner);

        if (lp != null)
        {
            lp.SetPlayerName(name);
            Debug.Log($"[LobbyManager] Sent player name: {name}");
        }
        else
        {
            Debug.LogWarning("[LobbyManager] Could not find local LobbyPlayer to send name.");
        }
    }
}
public struct PlayerConnectData
{
    public string playerName;
}