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
        StopCoroutine(SwitchPos());
        StartCoroutine(SwitchPos());
    }

    IEnumerator SwitchPos()
    {
        letMove = false;
        int frameCount = 20;

        for (int i = 0; i < frameCount; i++)
        {
            if (!inScreen)
            {
                transform.position = Vector3.Lerp(pos1.position, pos2.position, (float)i / (float)frameCount);
                //transform.rotation = Quaternion.Lerp(closePos.rotation, defaultPos.rotation, (float)i / (float)frameCount);
            }
            else if (inScreen)
            {
                transform.position = Vector3.Lerp(pos2.position, pos1.position, (float)i / (float)frameCount);
                //transform.rotation = Quaternion.Lerp(defaultPos.rotation, closePos.rotation, (float)i / (float)frameCount);
            }
            yield return null;
        }

        inScreen = !inScreen;
        letMove = true;
    }
}
