using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

/// <summary>
/// 
/// - O Código ativa/desativa as cartas do menu inicial que não foram selecionadas
/// - Assim que o jogador clica em alguma das 4 principais cartas do menu, as outras desaparecem para não ficarem sobressaindo sobre a carta que foi aberta
/// 
/// </summary>

public class DisableButtonsUI : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private AudioClip buttonEffect;
    [Tooltip("Add here the buttons that will be disabled when this gameobject is activated.")]
    [EnumData(typeof(Buttons))]
    [SerializeField] private GameObject[] buttons;

    [Header("Conditions")]
    [Tooltip("Condition responsible for activating/deactivating the cards.")]

    [SerializeField] private RectTransform cardTransform;


    [Header("DotWeen")]
    [SerializeField] private Vector3 targetLocation = Vector3.zero;
    [Range(0,10)]
    [SerializeField] private float moveDuration = 1.0f;
    [SerializeField] private Ease moveEase = Ease.Linear;
   

    //Enum com o nome de todos os botões que serão Desabilitados. O enum serve para nomear os elementos em ordem no array 'buttons'
    public enum Buttons
    {
        Play,
        Config,
        Credits,
        Exit
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Delete))
        {
            BackCard();
        }
    }

    //------------É responsável por ativar/desativar as cartas do menu------------//
    public void Click(GameObject obj)
    {
        //Dispara o Áudio de click
        SoundManager.instance.PlaySound(buttonEffect);

        //Pega o objeto que sofreu o click do mouse
        cardTransform = null;
        cardTransform = obj.GetComponent<RectTransform>();
        targetLocation = cardTransform.anchoredPosition; //Guarda a posição inicial do Botão para voltá-lo ao normal mais tarde

        CardAnimation(); //Inicia a animação
        DisableButtons(obj); //Desativa os outros botões
    }
    
    private void DisableButtons(GameObject obj)
    {
        //Verifica quais foram os objetos que não foram clicados e os desativa
        for (int i = 0; i < buttons.Length; i++)
        {
            if (buttons[i].gameObject == obj)
                continue;
            else
                buttons[i].SetActive(false);
        }
    }

    //------------Garante que quando algum menu for fechado as outras cartas vão ser ativadas------------//
    private void ActiveButtons()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            if (buttons[i].gameObject == cardTransform.gameObject)
                continue;
            else
                buttons[i].SetActive(true);
        }
    }

    void CardAnimation()
    {
        //CASO SEJA O BOTÃO 'JOGAR'
        if (cardTransform.gameObject == buttons[0].gameObject)
        {
            var sequence = cardTransform.DOMove(new Vector3(0, 0, 0), moveDuration).SetEase(moveEase);

            sequence.OnComplete(() => //Completa o Movimento
            {
                cardTransform.DORotate(new Vector3(0, 90, 0), moveDuration / 2).SetEase(moveEase).OnComplete(() => //Completa a rotação
                {
                    ////Alterar os components
                    cardTransform.GetComponent<Button>().enabled = false;
                    cardTransform.GetComponent<Image>().enabled = false;

                    ////Ativa o conteúdo
                    cardTransform.GetChild(0).gameObject.SetActive(true);
                    cardTransform.GetChild(1).gameObject.SetActive(false);

                    cardTransform.DORotate(new Vector3(0, 180, 90), moveDuration).SetEase(moveEase).OnComplete(() => //Completa a escala
                    {
                        ////Reescala o conteúdo
                        cardTransform.GetChild(0).transform.DOScale(new Vector3(1, 1, 1), moveDuration).SetEase(Ease.OutBack);
                 
                        RectTransform contentRect = cardTransform.GetChild(0).GetComponent<RectTransform>();
                        contentRect.DOSizeDelta(new Vector2(Screen.currentResolution.height * 2,Screen.currentResolution.width * 2), moveDuration * 2);
                    });
                });
            });
        }

        //CASO SEJA O BOTÃO 'CONFIG' OU 'CREDIT' OU 'SAIR'
        if (cardTransform.gameObject == buttons[1] || cardTransform.gameObject == buttons[2] || cardTransform.gameObject == buttons[3].gameObject)
        {
            var sequence = cardTransform.DOMove(new Vector3(0, 0, 0), moveDuration).SetEase(moveEase);
            
            sequence.OnComplete(() => //Completa o Movimento
            {
                cardTransform.DORotate(new Vector3(0, 90, 0), moveDuration/2).SetEase(moveEase).OnComplete(() => //Completa a rotação
                {
                    ////Alterar os components
                    cardTransform.GetComponent<Button>().enabled = false;
                    cardTransform.GetComponent<Image>().enabled = false;

                    ////Ativa o conteúdo
                    cardTransform.GetChild(0).gameObject.SetActive(true);
                    cardTransform.GetChild(1).gameObject.SetActive(false);
           
                    cardTransform.DORotate(new Vector3(0, 0, 90), moveDuration).SetEase(moveEase).OnComplete(() => //Completa a escala
                    {
                        ////Reescala o conteúdo
                        cardTransform.GetChild(0).DOScale(new Vector3(1, 1, 1), moveDuration * 3).SetEase(Ease.OutBack);
                        cardTransform.GetChild(0).DORotate(new Vector3(0, 0, 0), moveDuration);
                        
                        cardTransform.DOSizeDelta(new Vector2(1300, 800), moveDuration);
                    });
                });
            });
           
        }
    }

    public void BackCard()
    {
        //Reativar os botões desativados
        ActiveButtons();

        //Reativar os components
        cardTransform.GetComponent<Button>().enabled = true;
        cardTransform.GetComponent<Image>().enabled = true;
        cardTransform.GetChild(1).gameObject.SetActive(true);

        //Voltar as escalas originais
        cardTransform.DORotate(new Vector3(0, 0, 0), moveDuration *1.5f).SetEase(moveEase);
        cardTransform.DOSizeDelta(new Vector2(300, 440), moveDuration * 1.5f).SetEase(moveEase);
        cardTransform.DOAnchorPos(targetLocation, moveDuration);
   

        //Desativa o conteúdo
        cardTransform.GetChild(0).gameObject.SetActive(false);
       
        //Reescala o conteúdo
        cardTransform.GetChild(0).transform.DOScale(new Vector3(0.3f, 0.3f, 1), moveDuration).SetEase(Ease.OutBack);

    }


}
