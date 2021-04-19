using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MapInfo : MonoBehaviour
{
    public List<TeamManager.Team> teams = new List<TeamManager.Team>();
    public List<bool[,,]> spawnLocationLists = new List<bool[,,]>();
    public BasicSect[,,] basicSectArray;
    public Tile[,,] tileGrid;
    public static MapInfo instance;
    public Vector3 mapSize;

    private void Awake()
    {
        if(instance != null)
        {
            Debug.Log("Too many instances of MapInfo!");
            Destroy(instance.gameObject);
            instance = this;
            return;
        }
        instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    private void Start()
    {
       
    }
    
    public void ResetInfo()
    {
        teams.Clear();
        spawnLocationLists.Clear();
    }

    public struct MapData
    {
        public string mapName;
        public string spawnLocationLists;
        public string sectData;
        public Vector3 mapSize;

        public MapData(string _mapname, string _sectData, string _spawnLocationLists, Vector3 _mapSize)
        {
            mapName = _mapname;
            spawnLocationLists = _spawnLocationLists;
            sectData = _sectData;
            mapSize = _mapSize;
        }
    }
}
