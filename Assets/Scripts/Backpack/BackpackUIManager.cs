using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BackpackUIManager : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] Animator backpackUI;

    [Header("Contents")]
    [SerializeField] GameObject deck1;
    [SerializeField] GameObject deck2;
    [SerializeField] GameObject cards;
    [SerializeField] GameObject collection;

    [Header("Buttons")]
    [SerializeField] Button deck1Button;
    [SerializeField] Button deck2Button;
    [SerializeField] Button collectionButton;
    [SerializeField] Button cardsButton;
    [SerializeField] Sprite pressedButtonSprite;
    [SerializeField] Sprite unpressedButtonSprite;

    [Header("Controllers")]
    [SerializeField] DeckMenuController deck1controller;
    [SerializeField] DeckMenuController deck2controller;

    [Header("Audios")]
    [SerializeField] AudioSource openBackpackAudio;
    [SerializeField] AudioSource closeBackpackAudio;
    // [SerializeField] AudioSource clickAudio;
     
    [Header("Options")]
    [Tooltip("Uncheck this box in scenes when the backpack can't be opened")]
    [SerializeField] bool canOpenBackpack = true;
    
    bool backpackIsOpen = true;
    
    void Update() 
    {
        if(Input.GetKeyDown(KeyCode.Space) && canOpenBackpack) OpenCloseBackpack();
    }
    public void OpenCloseBackpack()
    {
        backpackIsOpen = !backpackIsOpen;

        //Abre a mochila
        if(!backpackIsOpen)
        {
            openBackpackAudio.Play();
            OpenDeck1();
            backpackUI.SetBool("Open Backpack", true);
            Debug.Log("Abre a mochila");
            return;
        }

        //Fecha a mochila
        if(backpackIsOpen)
        {
            closeBackpackAudio.Play();
            backpackUI.SetBool("Open Backpack", false);

            if(deck1.activeSelf) deck1controller.DestroyElements();
            else if(deck2.activeSelf) deck2controller.DestroyElements();         

            return;
        }
    }
    

    public void OpenDeck1()
    {
        deck1Button.image.sprite = pressedButtonSprite;

        if(deck2.activeSelf)
        {
            deck2Button.image.sprite = unpressedButtonSprite;
            deck2controller.DestroyElements();
            deck2.SetActive(false);
            deck1.SetActive(true);
            deck1controller.InstantiateElements();
        }      
        else if(cards.activeSelf)
        {
            cardsButton.image.sprite = unpressedButtonSprite;
            cards.SetActive(false);
            deck1.SetActive(true);
            deck1controller.InstantiateElements();
            return;
        }
        else if(collection.activeSelf)
        {
            collectionButton.image.sprite = unpressedButtonSprite;
            deck1Button.image.sprite = pressedButtonSprite;
            collection.SetActive(false);
            deck1.SetActive(true);
            deck1controller.InstantiateElements();
            return;
        }
        else
        {
            deck1.SetActive(true);
            deck1controller.InstantiateElements();
        }

    }

    public void OpenDeck2()
    {
        deck2Button.image.sprite = pressedButtonSprite;

        if(deck1.activeSelf)
        {
            deck1Button.image.sprite = unpressedButtonSprite;
            deck1controller.DestroyElements();
            deck1.SetActive(false);
            deck2.SetActive(true);
            deck2controller.InstantiateElements();
        }
        else if(cards.activeSelf)
        {
            cardsButton.image.sprite = unpressedButtonSprite;
            cards.SetActive(false);
            deck2.SetActive(true);
            deck2controller.InstantiateElements();
            return;
        }
        else if(collection.activeSelf)
        {
            collectionButton.image.sprite = unpressedButtonSprite;
            collection.SetActive(false);
            deck2.SetActive(true);
            deck2controller.InstantiateElements();
            return;
        }
        else
        {
            deck2.SetActive(true);
            deck2controller.InstantiateElements();
        }
    }

    public void OpenCards()
    {
        cardsButton.image.sprite = pressedButtonSprite;
        
        if(deck1.activeSelf)
        {
            deck1Button.image.sprite = unpressedButtonSprite;
            deck1controller.DestroyElements();
            deck1.SetActive(false);
            cards.SetActive(true);
        }
        else if(deck2.activeSelf)
        {
            deck2Button.image.sprite = unpressedButtonSprite;
            deck2controller.DestroyElements();
            deck2.SetActive(false);
            cards.SetActive(true);
        }
        else if(collection.activeSelf)
        {
            collectionButton.image.sprite = unpressedButtonSprite;
            collection.SetActive(false);
            cards.SetActive(true);
        }
        else
        {
            cards.SetActive(true);
        }
    }

    public void OpenCollection()
    {
        collectionButton.image.sprite = pressedButtonSprite;

        if(deck1.activeSelf)
        {
            deck1Button.image.sprite = unpressedButtonSprite;
            deck1controller.DestroyElements();
            deck1.SetActive(false);
            collection.SetActive(true);
        }
        else if(deck2.activeSelf)
        {
            deck2Button.image.sprite = unpressedButtonSprite;
            deck2controller.DestroyElements();
            deck2.SetActive(false);
            collection.SetActive(true);
        }
        else if(cards.activeSelf)
        {
            cardsButton.image.sprite = unpressedButtonSprite;
            cards.SetActive(false);
            collection.SetActive(true);
        }
        else
        {
            collection.SetActive(true);
        }
    }
}
