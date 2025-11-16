using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;

public class PlayerRepresentative : NetworkBehaviour
{
    public static PlayerRepresentative LocalInstance; 
    public readonly SyncVar<string> playerName = new("");
    public readonly SyncVar<int> playerID = new(-1);
    public readonly SyncVar<int> teamID = new(-1);

    private void Start()
    {
    }
    public override void OnStartClient()
    {
        if (IsOwner)
            LocalInstance = this;

        //LobbyManager.Instance?.RegisterPlayer(this);
    }

    public override void OnStopClient()
    {
        if (LocalInstance == this)
            LocalInstance = null;

        //LobbyManager.Instance?.UnregisterPlayer(this);
    }
    [ServerRpc(RequireOwnership = false)]
    public void SetLobbyDataRpc(string name, int team)
    {
        if (!IsServerInitialized) return;
        playerName.Value = name;
        teamID.Value = team;
    }
    
    private void OnEnable()
    {
        playerName.OnChange += OnNameChanged;
        teamID.OnChange += OnTeamChanged;
    }

    private void OnDisable()
    {
        playerName.OnChange -= OnNameChanged;
        teamID.OnChange -= OnTeamChanged;
    }
    private void OnNameChanged(string oldVal, string newVal, bool asServer)
    {
        UpdateLobbyUI();
    }

    private void OnTeamChanged(int oldVal, int newVal, bool asServer)
    {
        UpdateLobbyUI();
    }

    private void UpdateLobbyUI()
    {
        if (!IsClientInitialized) return;

        // Update your lobby UI here — maybe through a LobbyUIManager singleton?
    }

    private string _latestMap = null;
    private string _latestSpawns = null;

    private void Awake()
    {
       
    }

}
