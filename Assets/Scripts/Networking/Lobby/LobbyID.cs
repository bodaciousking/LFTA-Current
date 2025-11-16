using UnityEngine;
using TMPro;
using System.Linq;
using System.Collections;

public class LobbyID : MonoBehaviour
{
    public TMP_InputField chosenName;
    public string confirmedName;
    public static LobbyID instance;
    private void Awake()
    {
        if(instance != null)
        {
            Debug.Log("Too many LobbyID!");
            return;
        }
        instance = this;
    }
    public void ConfirmName()
    {
        confirmedName = chosenName.text;
    }
}
