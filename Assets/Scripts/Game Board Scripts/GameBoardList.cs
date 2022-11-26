using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// - Script responsável por armazenar todos os slots do tabuleiro
/// - Com o script é possível armazenar todos os slots do tabuleiro do player e do inimigo
/// - Esse armazenamento serve para verificar todos os slots que estão ocupados no tabuleiro, podendo assim verificar quais cartas já foram abaixadas
/// - O script vai no gameObject do tabuleiro
/// 
/// </summary>

public class GameBoardList : MonoBehaviour
{
    public List<Board> boards = new List<Board>();
}

[System.Serializable]
public class Board
{
    public string GameBoardName;
    public List<Slots> slots = new List<Slots>();
}

[System.Serializable]
public class Slots
{
    public string SlotName;
    public GameObject slotObject;
    public bool slotIsBusy;
}
