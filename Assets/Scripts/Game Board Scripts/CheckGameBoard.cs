using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// - Esse script verifica todas as posições do tabuleiro, para saber se elas já estão ocupadas com alguma carta ou não
/// - Caso algum slot esteja ocupado a variável 'slotIsBusy' que está no script GameBoard ficará true
/// - Essa verificação serve para confirmar se alguma carta específica já foi abaixada, caso sim, as cartas que necessitam desta para abaixerem agora podem fazê-lo
/// - O Script pode ir em qualquer gameObject vazio, de preferência um chamado 'Card & Board Manager'
/// 
/// </summary>

public class CheckGameBoard : MonoBehaviour
{
    [SerializeField] private GameBoardList gameBoardList;

    public void OnCheckGameBoard()
    {
        for (int i = 0; i < gameBoardList.boards[0].slots.Count; i++)
        {
            if (gameBoardList.boards[0].slots[i].slotObject.transform.childCount > 0)
            {
                gameBoardList.boards[0].slots[i].slotIsBusy = true;
            }
            else
            {
                gameBoardList.boards[0].slots[i].slotIsBusy = false;
            }
        }
    }
}
