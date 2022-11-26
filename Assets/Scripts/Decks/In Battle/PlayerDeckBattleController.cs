using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class PlayerDeckBattleController : MonoBehaviour
{
    // TO DO
    // Set Values to new cards
    // Integrate with the CardData system

    [SerializeField] DeckData playerDeck;
    Stack<CardData> grimory = new Stack<CardData>();
    [SerializeField] int initialCardsNumber = 6;
    public List<CardData> playerCardsData = new List<CardData>();
    public int cardToRemove;
    [SerializeField] GameObject cardPrefab;
    [SerializeField] Transform cardHolder;
    [SerializeField] List<GameObject> playerCardsObjects = new List<GameObject>();

    void Start() 
    {
        RandomizeGrimory();
        SelectInitialCards();
        InstantiateInitialCards();
    }

    void RandomizeGrimory()
    {
        Dictionary<int, int> auxDictionary = new Dictionary<int, int>();

        foreach(CardData card in playerDeck.cards)
        {
            if(!auxDictionary.ContainsKey(card.id))
            {
                auxDictionary.Add(card.id, 1);
            }
            else
            {
                auxDictionary[card.id] += 1;
            }
        }

        while(grimory.Count < DeckData.maxCards)
        {
            int randomIndex = (int)UnityEngine.Random.Range(0, DeckData.maxCards);
            CardData card = playerDeck.cards[randomIndex];

            if(auxDictionary[card.id] > 0)
            {
                grimory.Push(card);
                auxDictionary[card.id] -= 1;
            }
        }
    }

    void SelectInitialCards()
    {
        for(int i = 0; i < initialCardsNumber; i++)
        {
            PickGrimoryCard();
        }
    }

    public void PickGrimoryCard()
    {
        if(playerCardsData.Count < 6 && grimory.Count != 0)
        {
            playerCardsData.Add(grimory.Peek());
            grimory.Pop();
        }
        else if(playerCardsData.Count == 6)
        {
            Debug.Log("Hand is full");
        }
        else if(grimory.Count == 0)
        {
            Debug.Log("Grimory is Empty");
        }

    }

    public void RemoveHandCard(CardData card)
    {
        if(playerCardsData.Count > 0)
        {
            if(playerCardsData.Contains(card))
            {
                playerCardsData.Remove(card);
            }
            else
            {
                Debug.LogException(new System.Exception("Try to remove the non-existent card from the player's hand"));
            }
        }
        else
        {
            Debug.LogException(new System.Exception("Player's hand is empty"));
        }
    }

    void InstantiateInitialCards()
    {        
        for(int i = 0; i < initialCardsNumber; i++)
        {
            GameObject newCard = Instantiate(cardPrefab);
            newCard.transform.SetParent(cardHolder, false);
            newCard.transform.localScale = new Vector3(1,1,1);
            playerCardsObjects.Add(newCard);

            // SET VALUES
        }
    }

    public void InstantiateCard()
    {
        GameObject newCard = Instantiate(cardPrefab);
        newCard.transform.SetParent(cardHolder, false);
        newCard.transform.localScale = new Vector3(1,1,1);
        playerCardsObjects.Add(newCard);
    }

}


#if UNITY_EDITOR
[CustomEditor(typeof(PlayerDeckBattleController))]
public class PlayerDeckEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var playerDeck = (PlayerDeckBattleController)target;
        
        DrawDefaultInspector();

        if(GUILayout.Button("Remove Card"))
        {
            playerDeck.RemoveHandCard(playerDeck.playerCardsData[playerDeck.cardToRemove]);          
        }

        if(GUILayout.Button("Add Card"))
        {
            playerDeck.PickGrimoryCard();
            if(playerDeck.playerCardsData.Count < 6) playerDeck.InstantiateCard();
            else Debug.Log("Player's Hand is full");
        }

    }
}
#endif