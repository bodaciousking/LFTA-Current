using UnityEngine;

public class b_done : MonoBehaviour
{
    MapEditing mE;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mE = MapEditing.instance;
    }

    public void ExportMap()
    {
        mE.ExportMap();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
