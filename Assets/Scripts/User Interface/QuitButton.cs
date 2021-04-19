using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuitButton : MonoBehaviour
{
    MenuNavigation menuNav;

    public void ReturnToMainMenu()
    {
        menuNav = MenuNavigation.instance;

        menuNav.GoToMenuScene();
    }
}
