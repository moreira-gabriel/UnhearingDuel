using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;

public class DialogueManager : MonoBehaviour
{
    public delegate void activeFade(); //Delegate criado para uso conjunto ao evento 'fadeStarted'.
    public event activeFade fadeStarted; //Evento respons�vel por disparar o fade no c�digo 'FadeManager'.

    [Header("COMPONENTS")]
    public GameObject dialogueBox; //Objeto que cont�m o Text Mesh do di�logo e todas as outras informa��es, como background, imagens, etc.
    [SerializeField] private Image actorImage;
    [SerializeField] private Button nextButtonObject; //Bot�o pr�ximo
    public TMP_Text speechText; //Text Mesh onde ir� aparecer todo o di�logo escrito no scriptable object 'DialogueObject'.
    public TMP_Text actorNameText; //Text Mesh com o nome do personagem que est� falando.
    public DialogueObject dialogueObject; //Objeto de tradu��o de di�logo ('DialogueObject')

    [Space]
    [Header("OPTIONS")]
    [SerializeField] private bool finalWithFade; //Caso deseje que no final do di�logo aconte�a um fade.
    public float typingSpeed; //Velocidade com que o texto aparece na caixa de di�logo.
    public idiom language;
    [SerializeField] private List<GameObject> optionsBox; //Caixa de op��es 
    public DialogueObject[] optionA; //Op��o A caso o player escolha 'sim' (por exemplo)
    public DialogueObject[] optionB; //Op��o A caso o player escolha 'nao' (por exemplo)

    [Space]
    [Header("ACTIONS")]
    [Tooltip("Dialog index in the scriptable object that will fade.")]
    [SerializeField] private List<int> indexToFade = new List<int>(); //Caso deseje que durante alguma parte do di�logo aconte�a um fade, basta colocar nesse valor o mesmo valor do �ndice do di�logo.
    [Tooltip("Dialog index that will appear options for the player to choose.")]
    [SerializeField] private List<int> indexToCondition = new List<int>(); //Caso deseje que durante alguma parte do di�logo apare�a um caixa de op��es para o player selecionar basta colocar o �ndice do di�logo.
    [SerializeField] private List<int> indexToCoinTransaction = new List<int>();

    [Space]
    [Header("LISTS")]
    public Image backgroundImage;
    public List<Image> backgrounds = new List<Image>(); //Lista com todos os brackgrounds de acordo com a fala

    [Space]
    [Header("EVENTS")]
    [SerializeField] private UnityEvent finalEvent;

    //------VARI�VEIS PRIVADAS------//
    private bool isShowing; //Verifica se a caixa de di�logo est� ativa ou n�o, caso false o jogador pode apertar a telca 'E' para habilitar a caixa e come�ar o di�logo.
    private bool isOption; //Caso o di�logo seja respondendo alguma op��o que o player selecionou. Se verdade ele ir� voltar para o di�logo principal assim que acabarem as senten�as.
    private int index; //�ndice respons�vel por pular para os pr�ximos di�logos seguindo a lista.
    private int indexOption; //�ndice dos scriptable objects das op��es A e B. Caso exista mais op��es o c�digo ir� ler automaticamente de acordo com o indice.
    private int placeHolderIndexToCondition; //Essa vari�vel guarda o �ndice do di�logo que vai iniciar a caixa de op��es, assim quando o di�logo secund�rio acabar ele volta ao di�logo principal 1 di�logo a frente de onde tinha parado.
    private string[] sentences; //Lista privada com as mesmas senten�as da lista 'sentence' logo abaixo.
    

    
    //------LISTAS PRIVADAS------//
    public static DialogueManager instance;
    private List<string> sentence = new List<string>(); //Lista com todos os di�logos da cena
    private List<Image> actorList = new List<Image>(); //Lista com todos os sprites dos personagens


    [SerializeField] private List<int> indexToEvent = new List<int>();


    [System.Serializable] 
    public enum idiom //Enum que cont�m todos os idiomas dos di�logos
    {
        pt,
        eng,
    } 

   

    private void Awake()
    {
        instance = this;
        GetTexts(); //Pega os textos com a tradu��o selecionada antes de inicializar o game.
        GetActorSprite(); //Pega os sprites dos npcs antes de inicializar o game.
    }

    private void Start()
    {
        if(indexToCondition.Count > 0)
        {
            placeHolderIndexToCondition = indexToCondition[0] + 1;
        } //Caso existir alguma condi��o o placeHolder vai pegar o valor do �ndice do di�logo e guardar.
    }

    //-----------------RESPONS�VEL POR MOSTRAR LETRA POR LETRA DO DI�LOGO DE ACORDO COM A VELOCIDADE DEFINIDA PELA VARI�VEL 'typingSpeed'-----------------
    IEnumerator TypeSentence()
    {
        //-----------ADICIONAR O SPRITE DO FALANTE-----------
        if (actorList[index] != null) //Caso no indice da conversa tenha algum sprite de NPC ser� adicionado
        {
            actorImage.sprite = actorList[index].sprite;
            actorImage.color = new Color(1, 1, 1, 1);
        }
        else //Caso no indice da conversa n�o tenha nenhum sprite tudo ser� removido
        {
            actorImage.sprite = null;
            actorImage.color = new Color(0, 0, 0, 0);
        }



        foreach (char letter in sentences[index].ToCharArray())
        {
            speechText.text += letter;
            yield return new WaitForSeconds(typingSpeed);

            if (speechText.text.Length == sentences[index].Length) //Caso a frase seja completa normalmente, ativar o bot�o op��es (caso seja necess�rio)
            {
                if(indexToCondition.Count > 0 && index == indexToCondition[0])
                {
                    optionsBox[0].SetActive(true);
                    nextButtonObject.gameObject.SetActive(false);
                }
                else
                {
                    optionsBox[0].SetActive(false);
                    nextButtonObject.gameObject.SetActive(true);
                }
            }
        }
    }

    //-----------------PULA PARA O PR�XIMO DI�LOGO, CASO TENHA UM FADE ENTRE OS DI�LOGOS ELE EXECUTA O FADE E PREPARA O PR�XIMO DI�LOGO-----------------//
    public void NextSentence()
    {
        //-------MUDA OS TEXTOS DE ACORDO COM ALGUMAS CONDI��ES-------//
        if (speechText.text != "")
        {
            if (speechText.text == sentences[index]) //Caso o texto esteja completo
            {
                if (index < sentences.Length - 1)
                {
                    //------------VERIFICA INDEX TO FADE CASO O TEXTO ESTEJA COMPLETO E CASO O INDEX TO CONDITION SEJA IGUAL A ZERO------------//
                    if(indexToCondition.Count == 0)
                    {
                        if (indexToFade.Count > 0 && index != indexToFade[0]) //CASO O INDEX TO FADE FOR DIFERENTE ELE IR� CONTINUAR PASSANDO OS DI�LOGOS
                        {
                            index++;
                            speechText.text = "";
                            StartCoroutine(TypeSentence());
                        }
                        else if (indexToFade.Count == 0)
                        {
                            index++;
                            speechText.text = "";
                            StartCoroutine(TypeSentence());
                        }
                        else //CASO O INDEX TO FADE FOR IGUAL ELE IR� INICIAR O FADE E ACABAR O DI�LOGO
                        {
                            speechText.text = "";
                            if (indexToFade != null)
                            {
                                indexToFade.RemoveAt(0);
                            }
                            index++;
                            dialogueBox.SetActive(false);
                            sentences = null;
                            isShowing = false;
                            fadeStarted();
                        }
                    }else if(indexToCondition.Count > 0) //CASO O INDEX TO CONDITION FOR MAIOR QUE ZERO
                    {
                        if(indexToFade.Count > 0) //Caso index to fade maior que zero
                        {
                            if (index != indexToFade[0] && index != indexToCondition[0]) //CASO O INDEX TO CONDITIOM E INDEX TO FADE FOREM DIFERENTES ELE IR� CONTINUAR PASSANDO OS DI�LOGOS
                            {
                                index++;
                                speechText.text = "";
                                StartCoroutine(TypeSentence());
                            }
                            else if (index == indexToCondition[0] || index == indexToFade[0])
                            {
                                if (indexToFade.Count > 0) //Caso index to fade maior que zero
                                {
                                    if (finalWithFade)
                                    {
                                        if (index == indexToFade[0]) //Caso igual � index to fade
                                        {
                                            speechText.text = "";
                                            if (indexToFade != null)
                                            {
                                                indexToFade.RemoveAt(0);
                                            }
                                            index++;
                                            dialogueBox.SetActive(false);
                                            sentences = null;
                                            isShowing = false;
                                            fadeStarted();
                                        }
                                        else if (index == indexToCondition[0]) //Caso igual a index to condition
                                        {
                                            optionsBox[0].SetActive(true);
                                            nextButtonObject.interactable = false;
                                        }
                                    }
                                    else
                                    {
                                        if (index == indexToFade[0]) //Caso igual � index to fade
                                        {
                                            speechText.text = "";
                                            if (indexToFade != null)
                                            {
                                                indexToFade.RemoveAt(0);
                                            }
                                            index++;
                                            dialogueBox.SetActive(false);
                                            sentences = null;
                                            isShowing = false;
                                            fadeStarted();
                                        }
                                        else if (index == indexToCondition[0]) //Caso igual a index to condition
                                        {
                                            optionsBox[0].SetActive(true);
                                            nextButtonObject.interactable = false;
                                        }
                                    }
                                   
                                }
                                else //Caso index to fade n�o for maior que zero
                                {
                                    if (index == indexToCondition[0])
                                    {
                                        optionsBox[0].SetActive(true);
                                        nextButtonObject.interactable = false;
                                    }
                                }
                            }
                        }
                        else //Caso index to fade for menor que zero
                        {
                            if (index != indexToCondition[0]) //CASO O INDEX TO CONDITIOM E INDEX TO FADE FOREM DIFERENTES ELE IR� CONTINUAR PASSANDO OS DI�LOGOS
                            {
                                index++;
                                speechText.text = "";
                                StartCoroutine(TypeSentence());
                            }
                            else if (index == indexToCondition[0])
                            {
                                optionsBox[0].SetActive(true);
                                nextButtonObject.interactable = false;
                            }
                        }
                    }
                }
                else //CASO SEJA O �LTIMO DI�LOGO DE ALGUM SCRIPTABLE OBJECT
                {
                    //------------CASO DESEJE QUE O DI�LOGO TERMINE COM FADE------------//
                    if (finalWithFade && fadeStarted != null)
                    {
                        speechText.text = "";
                        index ++;
                        dialogueBox.SetActive(false);
                        sentences = null;
                        isShowing = false;
                        fadeStarted();


                        //------------CASO SEJA O �LTIMO DI�LOGO DO SCRIPTABLE OBJECT DE OP��ES------------//
                        if (isOption)
                        {
                            sentence.Clear(); //Limpar a lista com as strings do Scriptable Object antigo
                            actorList.Clear(); //Limpar a lista com os sprites do Scriptable Object antigo

                            Array.Resize(ref sentences, 0);
                            speechText.text = "";

                            for (int i = 0; i < dialogueObject.dialogues.Count; i++)  //Caso seja o ultimo di�logo daquele scriptable object, ele vai pegar o scriptable principal
                            {
                                switch (language)
                                {
                                    case idiom.pt:
                                        sentence.Add(dialogueObject.dialogues[i].sentence.portuguese);
                                        break;
                                    case idiom.eng:
                                        sentence.Add(dialogueObject.dialogues[i].sentence.english);
                                        break;
                                }

                                actorList.Add(dialogueObject.dialogues[i].actorSprite); //Pega devolta os sprites dos NPC's na scriptable object principal
                            }

                            isOption = false;
                            index = placeHolderIndexToCondition; //Ele vai pegar o indice do di�logo que fez gerar as op��es e quando voltar vai continuar 1 di�logo a frente
                        }
                   
                    }
                    else //------------CASO TERMINE SEM FADE------------//
                    {
                        speechText.text = "";
                        index++;
                        dialogueBox.SetActive(false);
                        sentences = null;
                        isShowing = false;


                        //------------CASO SEJA O �LTIMO DI�LOGO DO SCRIPTABLE OBJECT DE OP��ES------------//
                        if (isOption)
                        {
                            fadeStarted();

                            sentence.Clear(); //Limpar a lista com as strings do Scriptable Object antigo
                            actorList.Clear(); //Limpar a lista com os sprites do Scriptable Object antigo

                            Array.Resize(ref sentences, 0);
                            speechText.text = "";

                            for (int i = 0; i < dialogueObject.dialogues.Count; i++)  //Caso seja o ultimo di�logo daquele scriptable object, ele vai pegar o scriptable principal
                            {
                                switch (language)
                                {
                                    case idiom.pt:
                                        sentence.Add(dialogueObject.dialogues[i].sentence.portuguese);
                                        break;
                                    case idiom.eng:
                                        sentence.Add(dialogueObject.dialogues[i].sentence.english);
                                        break;
                                }

                                actorList.Add(dialogueObject.dialogues[i].actorSprite); //Pega devolta os sprites dos NPC's na scriptable object principal
                            }

                            isOption = false;
                            index = placeHolderIndexToCondition; //Ele vai pegar o indice do di�logo que fez gerar as op��es e quando voltar vai continuar 1 di�logo a frente
                        }
                        else //Caso seja o �ltimo di�logo do scriptable object principal
                        {
                            if (finalEvent != null)
                            {
                                finalEvent.Invoke();
                            }
                        }
                    }
                   
                }
            }
            else //Caso o texto n�o esteja completo
            {
                StopAllCoroutines();
                speechText.text = "";
                speechText.text = sentences[index];
            }
        }
        VerifyText();
    }

    //-----------------ATIVA A CAIXA DE DI�LOGO E INICIALIZA O TEXTO-----------------//
    public void Speech(string[] txt)
    {
        if (!isShowing) //Caso a caixa de di�logo esteja desativada
        {
            if (index < sentence.Count)
            {
                dialogueBox.SetActive(true);
                sentences = txt;
                StartCoroutine(TypeSentence());
                isShowing = true;
            }
        }
    }

    //-----------------APERTE 'E' PARA MOSTRAR O DI�LOGO-----------------//
    public void ShowDialogue()
    {
        Speech(sentence.ToArray());
    }

    //-----------------PEGAR OS TEXTOS DE ACORDO COM O IDIOMA-----------------//
    void GetTexts()
    {
        //------CASO PORTUGU�S------//
        if (PlayerPrefs.GetInt("Language") == 0)
        {
            for (int i = 0; i < dialogueObject.dialogues.Count; i++)
            {
                sentence.Add(dialogueObject.dialogues[i].sentence.portuguese);
            }
        }

        //------CASO INGL�S------//
        if (PlayerPrefs.GetInt("Language") == 1)
        {
            for (int i = 0; i < dialogueObject.dialogues.Count; i++)
            {
                sentence.Add(dialogueObject.dialogues[i].sentence.english);
            }
        }
        
    }

    //-----------------PEGAR OS SPRITES DO NPC QUE ESTIVER FALANDO DE ACORDO COM A FALA-----------------//
    void GetActorSprite()
    {
        for (int i = 0; i < dialogueObject.dialogues.Count; i++)
        {
            actorList.Add(dialogueObject.dialogues[i].actorSprite);
        }
    }

    //-----------------VERIFICAR SE O TEXTO DO DI�LOGO EST� COMPLETO, CASO ESTEJA ATIVAR MENU OP��ES SE NECESS�RIO-----------------//

    void VerifyText() //Caso o texto seja completo usando o bot�o 'pr�ximo'
    {
        IndexToFade();


        if (indexToCondition.Count > 0) //Caso index to condition for maior que zero
        {
            if (index == indexToCondition[0])
            {
                if(dialogueBox.activeSelf == true)
                {
                    if (speechText.text.Length == sentences[index].Length)
                    {
                        optionsBox[0].SetActive(true);
                        nextButtonObject.gameObject.SetActive(false);
                    }
                    else
                    {
                        optionsBox[0].SetActive(false);
                        nextButtonObject.gameObject.SetActive(true);
                    }
                }
            }
        }
    }

    //-----------------QUANDO O JOGADOR SELECIONAR ALGUMA OP��O ESSE M�TODO VAI PEGAR OS DI�LOGOS CORRESPONDENTES A OP��O ESCOLHIDA-----------------//

    void OptionSelected(string[] txt)
    {
        if (index < sentence.Count)
        {
            sentences = txt;
            StartCoroutine(TypeSentence());
        }
    }

    //-----------------CASO O JOGADOR ESCOLHA ALGUMA DAS OP��ES, EXECUTAR O DI�LOGO CORRESPONDENTE-----------------//

    public void OptionA() //Pega os textos que est�o no Scriptable Object (YesCondition)
    {
        sentence.Clear(); //Limpar a lista com as strings do Scriptable Object antigo
        actorList.Clear(); //Limpar a lista com os sprites do Scriptable Object antigo

        Array.Resize(ref sentences, 0);
        speechText.text = "";

        for (int i = 0; i < optionA[indexOption].dialogues.Count; i++)  //Adicionar as falas do novo Scriptable Object na lista
        {
            switch (language)
            {
                case idiom.pt:
                    sentence.Add(optionA[indexOption].dialogues[i].sentence.portuguese);
                    break;
                case idiom.eng:
                    sentence.Add(optionA[indexOption].dialogues[i].sentence.english);
                    break;
            }

            actorList.Add(optionA[indexOption].dialogues[i].actorSprite);
        }

        index = 0;
        isOption = true;
        indexToCondition.RemoveAt(0);
        indexOption++;
        OptionSelected(sentence.ToArray());
        optionsBox[0].SetActive(false);
        optionsBox.RemoveAt(0);
        nextButtonObject.gameObject.SetActive(true);
    } 

    public void OptionB() //Pega os textos que est�o no Scriptable Object (NoCondition)
    {
        sentence.Clear(); //Limpar a lista com as strings do Scriptable Object antigo
        actorList.Clear(); //Limpar a lista com os sprites do Scriptable Object antigo

        Array.Resize(ref sentences, 0);
        speechText.text = "";

        for (int i = 0; i < optionB[indexOption].dialogues.Count; i++)  //Adicionar as falas do novo Scriptable Object na lista
        {
            switch (language)
            {
                case idiom.pt:
                    sentence.Add(optionB[indexOption].dialogues[i].sentence.portuguese);
                    break;
                case idiom.eng:
                    sentence.Add(optionB[indexOption].dialogues[i].sentence.english);
                    break;
            }

            actorList.Add(optionB[indexOption].dialogues[i].actorSprite);
        }

        index = 0;
        isOption = true;
        indexToCondition.RemoveAt(0);
        indexOption++;
        OptionSelected(sentence.ToArray());
        optionsBox[0].SetActive(false);
        optionsBox.RemoveAt(0);
        nextButtonObject.gameObject.SetActive(true);
    }


    void IndexToFade()
    {
        if(indexToEvent[0] == index)
        {
            Debug.Log("Mesmo índice");
            indexToEvent.Remove(0);
        }
    }
}