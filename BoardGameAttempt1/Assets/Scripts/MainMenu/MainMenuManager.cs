using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    public GameObject MainMenuButtonHolder;
    public GameObject LobbyHolder;
    public GameObject CustomizationHolder;

    public void btnLocalGamePressed()
    {
        OpenLobbyScreen();
    }

    public void btnOpenCustomizationScreen()
    {
        MainMenuButtonHolder.SetActive(false);
        CustomizationHolder.SetActive(true);        
    }

    private void OpenLobbyScreen()
    {
        MainMenuButtonHolder.SetActive(false);
        LobbyHolder.SetActive(true);
    }

    public void btnLocalGameBack()
    {
        BackToMainMenu();
    }    

    private void BackToMainMenu()
    {
        LobbyHolder.SetActive(false);
        CustomizationHolder.SetActive(false);
        MainMenuButtonHolder.SetActive(true);        
    }
}