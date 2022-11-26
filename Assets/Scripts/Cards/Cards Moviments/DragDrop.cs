using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

/// <summary>
///  
/// - Esse c�digo vai nas cartas
///  - Basicamente serve para puxar e arrastar uma carta que esteja na m�o do player
///  - Assim que a carta � abaixada n�o � mais poss�vel arrast�-la
///  
/// </summary>


public class DragDrop : MonoBehaviour
{
    //-----------DELEGATES-----------//
    public delegate void BoardChecker();
    public BoardChecker boardChecker { get; private set; }

    //-----------OBJETOS-----------//
    [Header("COMPONENTS")]
    [SerializeField] CheckGameBoard checkGameBoard;
    [SerializeField] private AudioClip cardClip;
    private GameObject dropZone;


    //-----------VERIFICADORES DE ESTADO DA CARTA-----------//
    private bool isDragging = false;
    private bool isOverDropZone = false;
    [HideInInspector] public bool lowered = false;


    //-----------POSI��O,ROTA��O E ESCALA INICIAIS DA CARTA-----------//
    private Vector2 startPosition;
    private Quaternion startRotation;
    private Vector3 startScale;
    private RectTransform rectTransform;

    

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();

        //Registra os eventos a um delegate no script CheckGameBoard
        RegisterEvent();
    }

    private void Update()
    {
        //Verifica se a carta est� sendo arrastada.
        VerifyDraggin();
    }

    //------------------------------------////------------------------------------////------------------------------------//------------------------------------//
    //------------------------------------////-------M�TODOS DETECTA COLIS�O------////------------------------------------//------------------------------------//
    //------------------------------------////------------------------------------////------------------------------------//------------------------------------//


    //-----------ASSIM QUE A CARTA COLIDIR COM A DROPZONE-----------//
    private void OnCollisionStay2D(Collision2D collision)
    {
        isOverDropZone = true;
        dropZone = collision.gameObject;
    }

    //-----------ASSIM QUE A CARTA SAIR DA DROPZONE-----------//
    private void OnCollisionExit2D(Collision2D collision)
    {
        isOverDropZone = false;
        dropZone = null;
    }


    //------------------------------------////------------------------------------////------------------------------------//------------------------------------//
    //------------------------------------////---M�TODOS VERIFICADORES DA CARTA---////------------------------------------//------------------------------------//
    //------------------------------------////------------------------------------////------------------------------------//------------------------------------//


    //-----------VERIFICAR SE A CARTA EST� SENDO ARRASTADA-----------//
    void VerifyDraggin()
    {
        if (isDragging)
        {
            var screenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z);
            screenPoint.z = 10.0f; //distance of the plane from the camera
            transform.position = Camera.main.ScreenToWorldPoint(screenPoint);

            transform.rotation = new Quaternion(0, 0, 0, 0);
            transform.localScale = new Vector3(0.4166491f, 0.4166491f, 1);
        }
    }


    //-----------VERIFICAR O TIPO DA CARTA PARA DISPARAR EVENTOS-----------//
    void VerifyCard()
    {
        if (GetComponent<Soldier>()) { CardManager.instance.SoldierCards(); }
        if (GetComponent<Equipment>()) { CardManager.instance.EquipmentCards(); }

        if (GetComponent<Temporal>()) { CardManager.instance.EspecialCards(); }
        if (GetComponent<Special>()) { CardManager.instance.EspecialCards(); } //VERIFICAR ISTO AQUI DEPOIS <<<<<<<<<
        if (GetComponent<Support>()) { CardManager.instance.EspecialCards(); }
    }



    //------------------------------------////------------------------------------////------------------------------------//------------------------------------//
    //------------------------------------////-------M�TODOS ARRASTE CARTA--------////------------------------------------//------------------------------------//
    //------------------------------------////------------------------------------////------------------------------------//------------------------------------//


    //-----------ASSIM QUE A CARTA COME�AR A SER ARRASTADA-----------//
    public void StartDrag()
    {
        //Se a carta n�o estiver abaixada.
        if (!lowered) 
        {
            //Guarda as informa��es de transform inicial da carta.
            startPosition = transform.position;
            startRotation = transform.rotation;
            startScale = transform.localScale;
            
            //Confirma que a carta est� sendo arrastada.
            isDragging = true;

            //Verifica o tipo da carta (Equipamento, Soldado, Temporal etc).
            VerifyCard(); 
        }
    }


    //-----------ASSIM QUE ACABAR O ARRASTE DA CARTA-----------//
    public void EndDrag()
    {
        //Se a carta n�o estiver abaixada
        if (!lowered)
        {
            //Confirma que a carta n�o esta mais sendo arrastada.
            isDragging = false;

            //Reabilita as cartas que foram desabilitadas durante o drag.
            VerifyCard(); 


            //------CASO A CARTA ESTEJA SOBRE A DROPZONE E SEJA SOLTA------//
            if (isOverDropZone)
            {
                //Seta a carta como parente do objeto em que foi solta e desabilita o colisor do slot
                transform.SetParent(dropZone.transform, false);
                dropZone.GetComponent<BoxCollider2D>().enabled = false;

                //As informa��es de transform da carta s�o resetadas
                transform.localPosition = new Vector3(0, 0, 0);
                transform.localScale = new Vector3(1, 1, 1);
                

                //Confirma que a carta foi abaixada
                lowered = true;

                //Dispara um evento que envia uma mensagem para o tabuleiro informando que o slot foi ocupado
                this.boardChecker();
                SoundManager.instance.PlaySound(cardClip);
            }


            //------CASO A CARTA N�O ESTEJA SOBRE A DROPZONE E SEJA SOLTA------//
            else
            {
                //A carta retorna as suas propriedades de transform inicias
                transform.position = startPosition;
                transform.rotation = startRotation;
                transform.localScale = startScale;
            }
        }
    }

    void RegisterEvent()
    {
        if (GetComponent<Soldier>()) { boardChecker += checkGameBoard.OnCheckGameBoard; }
        if (GetComponent<Equipment>()) { boardChecker += checkGameBoard.OnCheckGameBoard; }

        if (GetComponent<Temporal>()) { boardChecker += checkGameBoard.OnCheckGameBoard; }
        if (GetComponent<Special>()) { boardChecker += checkGameBoard.OnCheckGameBoard; }
        if (GetComponent<Support>()) { boardChecker += checkGameBoard.OnCheckGameBoard; }
    }
    
}
