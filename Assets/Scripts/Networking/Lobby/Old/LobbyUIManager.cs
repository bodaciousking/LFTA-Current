using UnityEngine;
using TMPro;
using NUnit.Framework;

public class LobbyUIManager : MonoBehaviour
{/*
    public static LobbyUIManager Instance;
    LobbyManager manager;
    public Transform playerListParent;
    public GameObject playerSlotPrefab;

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        manager = LobbyManager.Instance;
    }

    public void RebuildLobby()
    {
        // Clear existing UI
        foreach (Transform child in playerListParent)
            Destroy(child.gameObject);

        if (manager != null)
        {
            for (int i = 0; i < manager.PlayerReps.Count; i++)
            {
                GameObject slot = Instantiate(playerSlotPrefab, playerListParent);
                slot.GetComponentInChildren<TextMeshProUGUI>().text = (i + 1).ToString(); // Display ID
            }
        }
    }*/
}