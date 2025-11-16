using FishNet;
using FishNet.Object;
using FishNet.Managing;
using FishNet.Managing.Scened;
using FishNet.Connection;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    public NetworkObject playerDataPrefab;
    private int nextTeamID = 0;

    private void Awake()
    {
        InstanceFinder.SceneManager.OnClientLoadedStartScenes += HandleClientLoadedScene;
    }

    private void OnDestroy()
    {
        InstanceFinder.SceneManager.OnClientLoadedStartScenes -= HandleClientLoadedScene;
    }

    private void HandleClientLoadedScene(NetworkConnection conn, bool asServer)
    {
        if (!asServer)
            return;

        NetworkObject playerObj = Instantiate(playerDataPrefab);
        PlayerData data = playerObj.GetComponent<PlayerData>();
        data.PlayerID.Value = conn.ClientId;
        data.TeamID.Value = nextTeamID++;

        InstanceFinder.ServerManager.Spawn(playerObj, conn);
    }
}
