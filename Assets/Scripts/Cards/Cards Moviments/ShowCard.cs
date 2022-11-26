using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 
/// - Script responsável por mostrar a carta no meio da tela assim que se expõe a um click do mouse
/// - Esse script deve ir dentro do gameObject das cartas
/// - Caso a carta esteja na mão do player ele pode clicar sobre ela e ela aparece no centro da tela
/// - Caso ele clique fora da carta ela volta a sua posição no tabuleiro
/// 
/// </summary>

public class ShowCard : MonoBehaviour/*,IClicker*/
{
    //-----------OBJETOS-----------//
    [SerializeField] private GameObject middleObject; //Centro da tela onde a carta será visualizada
    private DragDrop dragDrop;

    //-----------VERIFICADORES DE ESTADO DA CARTA-----------//
    private bool mouseOnObject = false; //Caso o mouse esteja sobre a carta
    private bool isShowing = false; //Caso a carta esteja sendo visualizada no centro da tela


    //-----------POSIÇÃO,ROTAÇÃO E ESCALA INICIAS DA CARTA-----------//
    private Vector2 startPosition;
    private Quaternion startRotation;
    private Vector3 startScale;


    void Start()
    {
        startPosition = transform.position;
        startRotation = transform.rotation;
        startScale = transform.localScale;

        dragDrop = GetComponent<DragDrop>();
    }

    void Update()
    {
        VerifyClick();
    }


    //-----------QUANDO O MOUSE SOBREPOR A CARTA-----------//
    private void OnMouseEnter()
    {
        mouseOnObject = true;
    }


    //-----------QUANDO O MOUSE SAIR DA CARTA-----------//
    private void OnMouseExit()
    {
        mouseOnObject = false;
    }


    //-----------MÉTODO RESPONSÁVEL POR COLOCAR A CARTA NO CENTRO DA TELA PARA SER VISUALIZADA ANTES DE JOGAR-----------//
    public void DraftCard()
    {
        if (!dragDrop.lowered)
        {
            isShowing = true;
            transform.position = middleObject.transform.position;
            transform.rotation = new Quaternion(0, 0, 0, 0);
            transform.localScale = new Vector3(1,1,1);
        }
    }


    //-----------CASO OCORRA OUTRO CLICK NA CARTA APÓS ESTAR NO CENTRO DA TELA ESSE MÉTODO IRÁ OU VOLTAR A CARTA PARA A POSIÇÃO ORIGINAL OU BAIXÁ-LA-----------//
    void VerifyClick()
    {
        if (isShowing) //Caso a carta esteja no centro da tela.
        {
            //Caso a carta estiver no centro da tela o box collider fica do tamanho da carta
            gameObject.GetComponent<BoxCollider2D>().size = new Vector2(gameObject.GetComponent<RectTransform>().rect.width, gameObject.GetComponent<RectTransform>().rect.height);
            if (dragDrop.lowered == false) //Caso a carta esteja em jogo ainda
            {
                if (Input.GetMouseButtonDown(0) && mouseOnObject)
                {
                    return;
                }
                else if (Input.GetMouseButtonDown(0) && mouseOnObject == false)
                {
                    transform.position = startPosition;
                    transform.rotation = startRotation;
                    transform.localScale = startScale;
                    isShowing = false;
                }
            }

        }
        else
        {
            gameObject.GetComponent<BoxCollider2D>().size = new Vector2(50,50);
        }
    }


    //----------- -----------//
}
