using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllUnitsLibrary : MonoBehaviour
{
    public static AllUnitsLibrary instance;
    public List<GameObject> heroList = new List<GameObject>();
    public List<GameObject> gruntList = new List<GameObject>();
    // Start is called before the first frame update

    private void Awake()
    {
        if (instance != null)
        {
            Debug.Log("Too many AULs!");
            Destroy(instance);
            instance = this;
            return;
        }
        else
            instance = this;

    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
