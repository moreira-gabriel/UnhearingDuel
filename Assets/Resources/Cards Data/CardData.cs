using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Empty Card", menuName = "Cards System/Card Data")]
public class CardData : ScriptableObject
{
    [Header("Components")]
    public CardType cardType;
    public string cardName;
    public Sprite icon;
    [TextArea] public string description;

    public enum CardType
    {
        Attack, Defense, Support, Consumable, Equipment, Soldier, Temporal
    }

    //-----PRIVADOS-----//
    public int id { get; private set; }

    void OnEnable() => id = GetInstanceID();
}