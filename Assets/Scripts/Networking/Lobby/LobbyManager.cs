using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FishNet.Broadcast;
using FishNet.Connection;
using FishNet.Managing;
using FishNet.Object;
using FishNet.Transporting;
using UnityEngine;

public class LobbyManager : MonoBehaviour
{
    [SerializeField] private NetworkManager networkManager;

    public readonly List<LobbyPlayer> lobbyPlayers = new List<LobbyPlayer>();
    public static LobbyManager instance;

    private void Awake()
    {
        if (networkManager == null)
            networkManager = GetComponent<NetworkManager>();

        if (instance != null)
        {
            Debug.Log("too many LobbyManagers!");
            return;
        }
        else instance = this;
    }

    private void OnEnable()
    {
        networkManager.ServerManager.RegisterBroadcast<PlayerSpawnedBroadcast>(OnPlayerSpawned_Server);
        networkManager.ClientManager.RegisterBroadcast<PlayerSpawnedBroadcast>(OnPlayerSpawned_Client);

        networkManager.ServerManager.RegisterBroadcast<PlayerDespawnedBroadcast>(OnPlayerDespawned_Server);
        networkManager.ClientManager.RegisterBroadcast<PlayerDespawnedBroadcast>(OnPlayerDespawned_Client);

        networkManager.ServerManager.OnRemoteConnectionState += OnRemoteConnectionState;
    }

    private void OnDisable()
    {
        networkManager.ServerManager.UnregisterBroadcast<PlayerSpawnedBroadcast>(OnPlayerSpawned_Server);
        networkManager.ClientManager.UnregisterBroadcast<PlayerSpawnedBroadcast>(OnPlayerSpawned_Client);

        networkManager.ServerManager.UnregisterBroadcast<PlayerDespawnedBroadcast>(OnPlayerDespawned_Server);
        networkManager.ClientManager.UnregisterBroadcast<PlayerDespawnedBroadcast>(OnPlayerDespawned_Client);

        networkManager.ServerManager.OnRemoteConnectionState -= OnRemoteConnectionState;
    }
    public void OnPlayerSpawned_Client(PlayerSpawnedBroadcast broadcast, Channel channel)
    {
        LobbyPlayer lp = broadcast.Player.GetComponent<LobbyPlayer>();
        if (lp != null && !lobbyPlayers.Contains(lp))
        {
            lobbyPlayers.Add(lp);
            Debug.Log($"[LobbyManager] Player added. Total players: {lobbyPlayers.Count}");

            // Subscribe to IsReady changes
            lp.IsReady.OnChange += OnPlayerReadyChanged; 
        }
        Debug.Log("[LobbyManager] OnPlayerSpawned_Client was triggered.");
    }
    public void OnPlayerSpawned_Server(NetworkConnection conn, PlayerSpawnedBroadcast broadcast, Channel channel)
    {
        LobbyPlayer lp = broadcast.Player.GetComponent<LobbyPlayer>();
        if (lp != null && !lobbyPlayers.Contains(lp))
        {
            lobbyPlayers.Add(lp);
            Debug.Log($"[LobbyManager] Player added. Total players: {lobbyPlayers.Count}");

            // Subscribe to IsReady changes
            lp.IsReady.OnChange += OnPlayerReadyChanged;
        }
    }
    private void OnPlayerDespawned_Server(NetworkConnection conn, PlayerDespawnedBroadcast broadcast, Channel channel)
    {
        LobbyPlayer lp = broadcast.Player.GetComponent<LobbyPlayer>();
        if (lp != null)
        {
            lobbyPlayers.Remove(lp);
            Debug.Log($"[LobbyManager] Player removed. Total players: {lobbyPlayers.Count}");
            lp.IsReady.OnChange -= OnPlayerReadyChanged;
        }
    }
    private void OnPlayerDespawned_Client(PlayerDespawnedBroadcast broadcast, Channel channel)
    {
        LobbyPlayer lp = broadcast.Player.GetComponent<LobbyPlayer>();
        if (lp != null)
        {
            lobbyPlayers.Remove(lp);
            Debug.Log($"[LobbyManager] Player removed. Total players: {lobbyPlayers.Count}");
            lp.IsReady.OnChange -= OnPlayerReadyChanged;
        }
    }
    private void OnPlayerReadyChanged(bool oldVal, bool newVal, bool asServer)
    {
        Debug.Log("[LobbyManager] Player ready changed, checking lobby state.");
        CheckAllReady();
    }

    private void CheckAllReady()
    {
        if (lobbyPlayers.Count == 2 && lobbyPlayers.All(lp => lp.IsReady.Value))
        {
            Debug.Log("[LobbyManager] All players ready! Starting game...");
            // TODO: trigger game start here (e.g., load scene or send event)
        }
    }
    public void InvokeClientSpawnBroadcast(PlayerSpawnedBroadcast broadcast)
    {
        OnPlayerSpawned_Client(broadcast, Channel.Reliable);
    }
    private void OnRemoteConnectionState(NetworkConnection conn, RemoteConnectionStateArgs args)
    {
        if (args.ConnectionState == RemoteConnectionState.Stopped)
        {
            NetworkObject obj = conn.FirstObject;
            if (obj != null && obj.TryGetComponent(out LobbyPlayer lp))
            {
                Debug.Log($"[LobbyManager] Despawning LobbyPlayer for disconnected client {conn.ClientId}.");

                networkManager.ServerManager.Despawn(obj);
                networkManager.ServerManager.Broadcast(new PlayerDespawnedBroadcast { Player = obj });
            }
        }
    }
}

// Helper structs for broadcasting spawn/despawn events
public struct PlayerSpawnedBroadcast : IBroadcast
{
    public NetworkObject Player;
}

public struct PlayerDespawnedBroadcast : IBroadcast
{
    public NetworkObject Player;
}
