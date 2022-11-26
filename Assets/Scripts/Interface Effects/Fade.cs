using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 
/// - Script responsável pelo fade entre cenas durante os diálogos
/// - O código vai no gameObject do painel fade
/// - O script atual em conjunto com o código 'DialogueManager', que informa em qual parte do diálogo deverá haver um fade
/// 
/// </summary>

public class Fade : MonoBehaviour
{
    private bool fadeIn;
    private bool fadeOut;

    [SerializeField] private CanvasGroup canvasGroup;
 
    private void Start()
    {
        HideUI();
    }

    private void ShowUI()
    {
        fadeIn = true;
    }

    private void HideUI()
    {
        //-------MUDA PARA O PRÓXIMO BACKGROUND-------//
        DialogueManager.instance.backgroundImage.sprite = DialogueManager.instance.backgrounds[0].sprite;
        DialogueManager.instance.backgrounds.RemoveAt(0);

        //-------EXIBE A CAIXA DE DIÁLOGO APÓS O FADE-------//
        DialogueManager.instance.ShowDialogue();

        //Inicia o fadeOut
        fadeOut = true; 
    }

    private void Update()
    {
        //Caso Fade In
        if (fadeIn)
        {
            if (canvasGroup.alpha < 1)
            {

                canvasGroup.alpha += Time.deltaTime;

                if (canvasGroup.alpha >= 1)
                {
                    fadeIn = false;
                }
            }
        }

        //Caso Fade Out
        if (fadeOut)
        {
            canvasGroup.alpha -= Time.deltaTime;

            if (canvasGroup.alpha == 0)
            {
                fadeOut = false;
            }
        }
    }

    public void OnFadeStarted()
    {
        ShowUI();
        StartCoroutine(WaitSeconds());
    }

    IEnumerator WaitSeconds()
    {
        yield return new WaitForSeconds(6f);
        HideUI();
    }

  
}
