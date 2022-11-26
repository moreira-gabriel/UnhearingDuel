using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// - C�digo criado para inscrever todos os eventos do projeto
/// - Basta criar as vari�veis dos scripts que v�o ser inscritos e fazer a inscri��o de todos no m�todo Start()
/// - Esse c�digo pode ir em qualquer gameObject vazio na cena
/// 
/// </summary>
public class EventManager : MonoBehaviour
{
    [SerializeField] private Fade fade;
    [SerializeField] private DialogueManager dialogueManager;

    void Start()
    {
        if(dialogueManager != null && fade != null)
        {
            dialogueManager.fadeStarted += fade.OnFadeStarted;
        }
    }

}
