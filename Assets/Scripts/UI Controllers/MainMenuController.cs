using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 
/// - Código vai no gameObject 'UIController' e trabalha em conjunto com o script OptionsMenuSettings
/// - Esse script em específico é repsonsável por carregar a cena do jogador ou realizar o quit no game
/// 
/// </summary>

public class MainMenuController : OptionsMenuSettings
{
    public void StartGame()
    {
        Debug.Log("Start Game");
        StartCoroutine(GoGame());
    }

    public void QuitGame()
    {
        Debug.Log("Quit Game");
        Application.Quit();
    }
   
    IEnumerator GoGame()
    {
        yield return new WaitForSeconds(5f);
        SceneManager.LoadScene(1);
    }
    
}
