using UnityEngine;

public class B_Color : MonoBehaviour
{
    MapEditing mE;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mE = MapEditing.instance;
    }


    public void Colorize()
    {
        mE.ChooseRecolor();
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
