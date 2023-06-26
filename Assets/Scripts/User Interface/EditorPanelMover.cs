using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorPanelMover : MonoBehaviour
{
    public Transform pos1, pos2;
    bool letMove;
    public bool inScreen;

    public void GoToPos()
    {
        StopCoroutine(Hide()); 
        StopCoroutine(Show());

        if (!inScreen)
            StartCoroutine(Show());
        else
            StartCoroutine(Hide());
    }

    public void StartHide()
    {
        StopCoroutine(Show()); 
        StopCoroutine(Hide()); 
        if(inScreen)
            StartCoroutine(Hide());
    }

    IEnumerator Hide()
    {
        letMove = false;
        int frameCount = 20;

        for (int i = 0; i < frameCount; i++)
        {
            transform.position = Vector3.Lerp(pos2.position, pos1.position, (float)i / (float)frameCount);
            yield return null;
        }

        inScreen = false;
        letMove = true;
    }

    IEnumerator Show()
    {
        letMove = false;
        int frameCount = 20;

        for (int i = 0; i < frameCount; i++)
        {
            transform.position = Vector3.Lerp(pos1.position, pos2.position, (float)i / (float)frameCount);
            yield return null;
        }
        letMove = true;
        inScreen = true;
    }


}
