using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Empty Deck", menuName = "Cards System/Deck Data")]
public class DeckData : ScriptableObject
{
    [Tooltip("Deck name to display in the backpack")] 
    public string deckName;
    
    [Tooltip("Max number of cards in a deck")]
    public static int maxCards = 40;

    [Tooltip("Cards list ")]
    public List<CardData> cards = new List<CardData>();
    public int id {get; private set;}
    public Dictionary<int, int> cardDictionary = new Dictionary<int, int>();

    void OnEnable()
    {
        id = GetInstanceID();

        if(cards.Count > maxCards)
        {
            for(int i = (cards.Count - maxCards); i > 0; i--)
            {
                cards.Remove(cards[i]);
            }
        }

        foreach(CardData card in cards)
        {
            if(!cardDictionary.ContainsKey(card.id))
            {
                cardDictionary.Add(card.id, 0);
            }
            else
            {
                cardDictionary[card.id] += 1;
            }
        }
    }
}
