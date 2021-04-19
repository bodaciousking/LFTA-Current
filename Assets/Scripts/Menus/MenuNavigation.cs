using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuNavigation : MonoBehaviour
{
    MapInfo mapInfo;
    public static MenuNavigation instance;

    private void Awake()
    {
        if(instance != null)
        {
            Destroy(instance.gameObject);
            instance = this;
            return;
        }
        instance = this;
    }
    private void Start()
    {
        DontDestroyOnLoad(this);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if(scene.name == "BattleRoom")
        {
            SetupBattleRoom();
        }
        else if (scene.name == "MapEditorRoom")
        {
            //SetupEditorScene();
        }
    }

    void SetupBattleRoom()
    {
        GameObject loadManager = GameObject.Find("LoadManager");
        MapLoading mapLoading = loadManager.GetComponent<MapLoading>();
        mapInfo = MapInfo.instance;

        mapLoading.LoadMap(mapInfo.basicSectArray, mapInfo.spawnLocationLists.ToArray());
    }

    public void SetupEditorScene()
    {
        mapInfo = MapInfo.instance;
        mapInfo.ResetInfo();
    }
    public void GoToMenuScene()
    {
        SceneManager.LoadScene("Menus");
    }
    public void GoToMapEditorScene()
    {
        SceneManager.LoadScene("MapEditorRoom");
    }
    public void GoToBattleScene()
    {
        SceneManager.LoadScene("BattleRoom");
    }
}
