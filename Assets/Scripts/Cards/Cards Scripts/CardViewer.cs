using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardViewer : MonoBehaviour
{
    public CardData cardData;
    Image cardIcon;
    [SerializeField] TMP_Text cardName;

    // [SerializeField] TMP_Text cardDescription;

    public void SetValues(CardData card)
    {
        cardData = card;
        // cardIcon.sprite = cardData.icon;
        cardName.text = cardData.cardName;
        // cardDescription.text = cardData.description;
    }
}
