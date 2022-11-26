using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// - Esse script verifica todas as posi��es do tabuleiro, para saber se elas j� est�o ocupadas com alguma carta ou n�o
/// - Caso algum slot esteja ocupado a vari�vel 'slotIsBusy' que est� no script GameBoard ficar� true
/// - Essa verifica��o serve para confirmar se alguma carta espec�fica j� foi abaixada, caso sim, as cartas que necessitam desta para abaixerem agora podem faz�-lo
/// - O Script pode ir em qualquer gameObject vazio, de prefer�ncia um chamado 'Card & Board Manager'
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
