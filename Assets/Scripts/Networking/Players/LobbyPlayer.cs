using FishNet.CodeGenerating;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;

public class LobbyPlayer : NetworkBehaviour
{
    public readonly SyncVar<string> PlayerName = new();
    public readonly SyncVar<bool> IsReady = new();
    public string Debug_PlayerName;
    public override void OnStartClient()
    {
        base.OnStartClient();
        //Debug.Log($"[LobbyPlayer] Player joined: {PlayerName.Value}, Ready: {IsReady.Value}");

        // Optional: add value change listeners
        PlayerName.OnChange += OnNameChanged;
        IsReady.OnChange += OnReadyChanged;
    }
    private void Update()
    {
        Debug_PlayerName = PlayerName.Value; 
    }
    [ServerRpc(RequireOwnership = false)]
    public void SetPlayerNameServerRpc(string name) => PlayerName.Value = name;

    private void OnNameChanged(string oldValue, string newValue, bool asServer)
    {
        Debug.Log($"[Client] Player name changed to: {newValue}");
    }

    private void OnReadyChanged(bool oldValue, bool newValue, bool asServer)
    {
        Debug.Log($"[Client] Player ready changed to: {newValue}");
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetPlayerName(string name)
    {
        PlayerName.Value = name;
        Debug.Log($"[Server] Name set to {PlayerName.Value}");
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetPlayerReady(bool ready)
    {
        IsReady.Value = ready;
        Debug.Log($"[Server] Ready status set to {IsReady.Value}");
    }
}
