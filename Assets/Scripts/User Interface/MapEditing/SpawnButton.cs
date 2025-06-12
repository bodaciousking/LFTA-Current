using UnityEngine;

public class SpawnButton : MonoBehaviour
{
    MapEditing mE;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mE = MapEditing.instance;
    }

    public void ChooseSpawn()
    {
        mE.action = MapEditing.Choice.ChoosingSpawnSpots;
    }
}
