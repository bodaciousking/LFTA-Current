using UnityEngine;
using FishNet.Managing;
using FishNet.Transporting.Tugboat;

public class NetworkStarter : MonoBehaviour
{
    [SerializeField] private NetworkManager networkManager;
    private LobbyID lID;
    private void Start()
    {
        lID = LobbyID.instance;
    }

    public void StartHost()
    {
        lID.ConfirmName();
        networkManager.ServerManager.StartConnection();
        networkManager.ClientManager.StartConnection();
    }

    public void StartClient()
    {
        lID.ConfirmName();
        networkManager.ClientManager.StartConnection();
    }
}
