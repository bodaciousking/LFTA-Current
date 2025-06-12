using UnityEngine;

public class B_Finalize : MonoBehaviour
{
    MapEditing mE;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mE = MapEditing.instance;
    }


    public void FinalizeSelection()
    {
        mE.CreateBaseMap();
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
