using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionsMenuController : OptionsMenuSettings
{
    [SerializeField] private GameObject optionsPanel;
    private bool isActive = false;

    private void Start() 
    {
        optionsPanel.SetActive(isActive);
    }

    void Update()
    {
        OpenCloseMenu();
    }

    public void OpenCloseMenu()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !isActive)
        {
            isActive = true;
            optionsPanel.SetActive(true);
            return;
        }

        if (Input.GetKeyDown(KeyCode.Escape) && isActive)
        {
            isActive = false;
            optionsPanel.SetActive(false);
            return;
        }    
    }
}
