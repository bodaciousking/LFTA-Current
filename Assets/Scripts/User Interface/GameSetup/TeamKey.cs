using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TeamKey : MonoBehaviour
{
    public Image bgImage, childImage;

    private void Awake()
    {
        bgImage = GetComponent<Image>();
        childImage = transform.Find("Panel").GetComponent<Image>();

    }
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
