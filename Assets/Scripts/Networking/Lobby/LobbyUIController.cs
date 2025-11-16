using UnityEngine;
using UnityEngine.UI;
using TMPro;
using FishNet.Managing;
using FishNet.Managing.Client;
using FishNet.Managing.Server;
using FishNet.Transporting;

public class LobbyUIController : MonoBehaviour
{
    [Header("UI References")]
    public GameObject mainMenuPanel;
    public GameObject lobbyPanel;
    public GameObject hostJoin;
    public TextMeshProUGUI errorMessageText;
    public Transform playerListParent;
    public GameObject playerSlotPrefab;

    [Header("Network")]
    public NetworkManager networkManager;
    public bool isConnected;

    LobbyManager lM;
    LobbyID lID;
    public GameObject hostB, joinB;

    private void Awake()
    {
        if (networkManager == null)
            networkManager = FindObjectOfType<NetworkManager>();

        mainMenuPanel.SetActive(true);
        lobbyPanel.SetActive(false);
        errorMessageText.text = "";
    }
    private void Start()
    {
        lM = LobbyManager.instance;
        lID = LobbyID.instance;
    }

    private void Update()
    {
        if(lID.chosenName.text != "")
        {
            hostB.SetActive(true);
            joinB.SetActive(true);
        }
        else
        {
            hostB.SetActive(false );
            joinB.SetActive(false );
        }
    }

    private void OnEnable()
    {
        networkManager.ClientManager.OnClientConnectionState += OnClientConnectionState;
        networkManager.ServerManager.OnServerConnectionState += OnServerConnectionState;
    }

    private void OnDisable()
    {
        networkManager.ClientManager.OnClientConnectionState -= OnClientConnectionState;
        networkManager.ServerManager.OnServerConnectionState -= OnServerConnectionState;
    }

    public void OnHostButton()
    {
        errorMessageText.text = "";

        if (networkManager.ServerManager.StartConnection())
            networkManager.ClientManager.StartConnection();
        else
            errorMessageText.text = "Failed to start host.";
    }

    public void OnJoinButton()
    {
        errorMessageText.text = "";

        if (!networkManager.ClientManager.StartConnection())
            errorMessageText.text = "Failed to connect as client.";
    }

    private void OnClientConnectionState(ClientConnectionStateArgs args)
    {
        if (args.ConnectionState == LocalConnectionState.Started)
        {
            //Debug.Log("[UI] Client connected.");
            isConnected = true;
            ShowLobbyUI(); 
        }
        else if (args.ConnectionState == LocalConnectionState.Stopped)
        {
            //Debug.Log("[UI] Client disconnected.");
            errorMessageText.text = "Disconnected from server.";
            isConnected = false;
            ShowMainMenuUI();
        }
    }

    private void OnServerConnectionState(ServerConnectionStateArgs args)
    {
        if (args.ConnectionState == LocalConnectionState.Stopped)
        {
            Debug.Log("[UI] Server stopped.");
            errorMessageText.text = "Server stopped.";
            isConnected = false;
            ShowMainMenuUI();
        }
    }
    public void OnPlayOnlineButton()
    {
        if (isConnected)
        { 
            ShowLobbyUI(); 
        } // Already connected
        else
            ShowHostJoinUI();  // Not connected yet
    }
    private void ShowHostJoinUI()
    {
        mainMenuPanel.SetActive(false);
        hostJoin.SetActive(true);
        lobbyPanel.SetActive(false);
    }

    private void ShowLobbyUI()
    {
        mainMenuPanel.SetActive(false);
        hostJoin.SetActive(false); 
        lobbyPanel.SetActive(true);
        RebuildLobby();
    }

    private void ShowMainMenuUI()
    {
        mainMenuPanel.SetActive(true);
        lobbyPanel.SetActive(false);
    }

    public void RebuildLobby()
    {
        // Clear existing UI
        foreach (Transform child in playerListParent)
            Destroy(child.gameObject);

        if (lM != null)
        {
            //Debug.Log("rebuilding UI");
            for (int i = 0; i < lM.lobbyPlayers.Count; i++)
            {
                Debug.Log("drawing player");
                GameObject slot = Instantiate(playerSlotPrefab, playerListParent);
                slot.GetComponentInChildren<TextMeshProUGUI>().text = (i + 1).ToString(); // Display ID
            }
        }
    }
}
