using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ldp;

public class PreloaderScript : MonoBehaviour {
    public GameObject tutorialMenu;

    public bool ReloadOnlyMenuManager;

    private void Start()
    {
        // Ask for an instance of the managers to initialize them
        // also allows to retry the game by reloading the scenes
        // as it isn't a persistent object
        MenuManager mm = MenuManager.Get;
        mm.Init();
        if (!ReloadOnlyMenuManager)
        {
            GameManager gm = GameManager.Get;
            gm.NewGame();
            Debug.Log("New Game");
        }
        if (tutorialMenu != null && !GameManager.Get.tutorialShown)
        {
            MenuManager.Get.Open(tutorialMenu);
            GameManager.Get.tutorialShown = true;
        }
    }
}
