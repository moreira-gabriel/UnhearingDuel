using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// - Código criado para inscrever todos os eventos do projeto
/// - Basta criar as variáveis dos scripts que vão ser inscritos e fazer a inscrição de todos no método Start()
/// - Esse código pode ir em qualquer gameObject vazio na cena
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
