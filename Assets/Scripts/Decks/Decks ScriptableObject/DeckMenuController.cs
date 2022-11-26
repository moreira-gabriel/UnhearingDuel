using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckMenuController : MonoBehaviour
{
    [SerializeField] CardViewer element;
    [SerializeField] DeckData deck;
    [SerializeField] List<GameObject> cards = new List<GameObject>();

    public void InstantiateElements()
    {
        for(int i = 0; i < deck.cards.Count; i++)
        {
            CardViewer newElement = Instantiate(element, transform);
            newElement.SetValues(deck.cards[i]);
            cards.Add(newElement.gameObject);
        }
    }

    public void DestroyElements()
    {
        for (int i = 0; i < cards.Count; i++) Destroy(cards[i]);

        cards.Clear();
    }

}
