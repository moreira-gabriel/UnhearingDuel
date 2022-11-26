using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 
/// - Script armazena todos os slots das cartas e descobre qual carta está sendo arrastada para o tabuleiro
/// - Dependendo da carta que está sendo arrastada ele desativa os slots que NÃO corresponde a esta carta
/// - Isso para a carta não ser abaixada em algum slot que não é permitido
/// - O script pode ir em qualquer gameObject vazio, de preferência um chamado 'Card & Board Manager'
/// 
/// </summary>

public class CardManager : MonoBehaviour
{
    //---------OBJECTS---------//
    [HideInInspector]
    [Header("HIGHLIGHT PANEL")]
    [SerializeField] private SpriteRenderer highlitghtPanel;
    [SerializeField] private bool itsTutorial;

    //---------VARIÁVEIS---------//
    private bool isChecking = false;
   
    //---------SLOTS---------//
    [Space]
    [Header("SLOTS SOLDIER CARDS")]
    [SerializeField] private SlotsSoldierCards[] slotsSoldierCards;

    [Space]
    [Header("SLOTS ESPECIAL CARDS")]
    [SerializeField] private SlotsTemporalCards[] slotsEspecialCards;

    //--------SINGLETON---------//
    public static CardManager instance;


    //----Void Awake----//
    private void Awake()
    {
        instance = this;
    }

    //--------------------------------------------Método Soldier-----------------------------------------------------------------------------------//

    //---------DESABILITA CARTAS EQUIPAMENTOS E CARTAS TEMPORAIS---------//
    public void SoldierCards() 
    {
        isChecking =! isChecking;

        /// * CASO ESTEJA CHECANDO UMA CARTA DO TIPO SOLDADO
        /// * DESABILITAR TODAS OS ESPAÇOS DE CARTAS QUE NÃO SEJAM
        /// * RESERVADOS A CARTA SOLDADO

        if (isChecking) //Desabilita todos os slots de cartas equipamento, temporal & soldado
        {
            //------MASKS SLOTS------//
            if (itsTutorial)
            {
                highlitghtPanel.color = new Color(0, 0, 0, 0.20f);
                for (int i = 0; i < slotsSoldierCards.Length; i++)
                {
                    slotsSoldierCards[i].slotMasks.SetActive(true);
                }
            }
          

            //------CARTAS EQUIPAMENTOS------//
            for (int i = 0; i < slotsSoldierCards.Length; i++)
            {
                slotsSoldierCards[i].slotEquipmentCard.GetComponent<BoxCollider2D>().enabled = false;
            }

            //------CARTAS TEMPORAIS------//
            for (int i = 0; i < slotsEspecialCards.Length; i++)
            {
                slotsEspecialCards[i].slot.GetComponent<BoxCollider2D>().enabled = false;
            }
            
            //------CARTAS SOLDIER OCUPADAS------//
            for (int i = 0; i < slotsSoldierCards.Length; i++)
            {
                //(Aqui é desabilitado os slots do tipo soldado que estão ocupados)
                if (slotsSoldierCards[i].slotSoldierCard.transform.childCount > 0)
                {
                    slotsSoldierCards[i].slotSoldierCard.GetComponent<BoxCollider2D>().enabled = false;
                }
            }
        }


        /// * CASO NÃO ESTEJA MAIS CHECANDO UMA CARTA DO TIPO SOLDADO
        /// * REABILITAR TODAS OS ESPAÇOS DE CARTAS QUE FORAM DESATIVADOS
        /// * INCLUINDO OS RESERVADOS A CARTA SOLDADO QUE ESTAVAM OCUPADOS
        
        else if (!isChecking)  //Reabilita todos os slots de cartas equipamento, temporal & soldado
        {
            //------MASKS SLOTS------//
            if (itsTutorial)
            {
                highlitghtPanel.color = new Color(0, 0, 0, 0);
                for (int i = 0; i < slotsSoldierCards.Length; i++)
                {
                    slotsSoldierCards[i].slotMasks.SetActive(false);
                }
            }
           
            //------CARTAS EQUIPAMENTOS------//
            for (int i = 0; i < slotsSoldierCards.Length; i++)
            {
                slotsSoldierCards[i].slotEquipmentCard.GetComponent<BoxCollider2D>().enabled = true;
            }

            //------CARTAS TEMPORAIS------//
            for (int i = 0; i < slotsEspecialCards.Length; i++)
            {
                slotsEspecialCards[i].slot.GetComponent<BoxCollider2D>().enabled = true;
            }

            //------CARTAS SOLDIER OCUPADAS------//
            for (int i = 0; i < slotsSoldierCards.Length; i++)
            {
                //Caso o slot de soldado tenha uma carta, desativar seu colisor
                if (slotsSoldierCards[i].slotSoldierCard.transform.childCount > 0)
                {
                    slotsSoldierCards[i].slotSoldierCard.GetComponent<BoxCollider2D>().enabled = false;
                }
                //Caso o slot de soldado não tenha uma carta, ativar seu colisor
                else
                {
                    slotsSoldierCards[i].slotSoldierCard.GetComponent<BoxCollider2D>().enabled = true;
                }
            }
        }
       
    }


    //--------------------------------------------Método Equipment-----------------------------------------------------------------------------------//

    //---------DESABILITA CARTAS SOLDADO E CARTAS TEMPORAIS---------//
    public void EquipmentCards() 
    {
        isChecking = !isChecking;

        /// * CASO ESTEJA CHECANDO UMA CARTA DO TIPO EQUIPAMENTO
        /// * DESABILITAR TODAS OS ESPAÇOS DE CARTAS QUE NÃO SEJAM
        /// * RESERVADOS A CARTA EQUIPAMENTO

        if (isChecking) //Desabilita todos os slots de cartas soldado, temporal & equipamento
        {
            //------CARTAS SOLDADO & EQUIPAMENTO------//
            for (int i = 0; i < slotsSoldierCards.Length; i++)
            {
                //(Aqui é desabilitado os slots do tipo soldado que VAZIOS e os que JÁ POSSUEM carta equipamento)
                if (slotsSoldierCards[i].slotSoldierCard.transform.childCount == 0)
                {
                    slotsSoldierCards[i].slotSoldierCard.GetComponent<BoxCollider2D>().enabled = false;
                    slotsSoldierCards[i].slotEquipmentCard.GetComponent<BoxCollider2D>().enabled = false;

                }
                else if (slotsSoldierCards[i].slotSoldierCard.transform.childCount > 0 && slotsSoldierCards[i].slotEquipmentCard.transform.childCount > 0)
                {
                    slotsSoldierCards[i].slotSoldierCard.GetComponent<BoxCollider2D>().enabled = false;
                    slotsSoldierCards[i].slotEquipmentCard.GetComponent<BoxCollider2D>().enabled = false;

                }
            }
            
            //------CARTAS TEMPORAL------//
            for (int i = 0; i < slotsEspecialCards.Length; i++)
            {
                slotsEspecialCards[i].slot.GetComponent<BoxCollider2D>().enabled = false;
            }
        }


        /// * CASO NÃO ESTEJA MAIS CHECANDO UMA CARTA DO TIPO EQUIPAMENTO
        /// * REABILITAR TODAS OS ESPAÇOS DE CARTAS QUE FORAM DESATIVADOS
        /// * INCLUINDO OS RESERVADOS A EQUIPAMENTO QUE ESTAVAM OCUPADOS 
        /// * E SOLDADO QUE ESTAVAM VAZIOS

        else if (!isChecking) //Reabilita todos os slots de cartas soldado & temporal
        {
            //------CARTAS SOLDADO------//
            for (int i = 0; i < slotsSoldierCards.Length; i++)
            {
                //Caso o slot de soldado tenha uma carta, desativar seu colisor e ativar o colisor do equipamento
                if (slotsSoldierCards[i].slotSoldierCard.transform.childCount > 0)
                {
                    slotsSoldierCards[i].slotSoldierCard.GetComponent<BoxCollider2D>().enabled = false;
                    slotsSoldierCards[i].slotEquipmentCard.GetComponent<BoxCollider2D>().enabled = true;
                }

                //Caso o slot de soldado não tenha uma carta, ativar seu colisor e desativar o colisor do equipamento
                else
                {
                    slotsSoldierCards[i].slotSoldierCard.GetComponent<BoxCollider2D>().enabled = true;
                    slotsSoldierCards[i].slotEquipmentCard.GetComponent<BoxCollider2D>().enabled = false;
                }
      
            }

            //------CARTAS TEMPORAL------//
            for (int i = 0; i < slotsEspecialCards.Length; i++)
            {
                slotsEspecialCards[i].slot.GetComponent<BoxCollider2D>().enabled = true;
            }
        }

    }


    //--------------------------------------------Método Especial-----------------------------------------------------------------------------------//

    //---------DESABILITA CARTAS EQUIPAMENTOS E CARTAS SOLDADO---------// 
    public void EspecialCards()
    {
        isChecking = !isChecking;

        /// * CASO ESTEJA CHECANDO UMA CARTA DO TIPO SOLDADO
        /// * DESABILITAR TODAS OS ESPAÇOS DE CARTAS QUE NÃO SEJAM
        /// * RESERVADOS A CARTA SOLDADO


        if (isChecking) //Desabilita todos os slots de cartas soldado & equipamento
        {
            //------CARTAS SOLDADO------//
            for (int i = 0; i < slotsSoldierCards.Length; i++)
            {
                slotsSoldierCards[i].slotSoldierCard.GetComponent<BoxCollider2D>().enabled = false;
                slotsSoldierCards[i].slotEquipmentCard.GetComponent<BoxCollider2D>().enabled = false;
            }

            //------CARTAS EQUIPAMENTOS------//
            for (int i = 0; i < slotsSoldierCards.Length; i++)
            {
                slotsSoldierCards[i].slotEquipmentCard.GetComponent<BoxCollider2D>().enabled = false;
            }

            //------CARTAS TEMPORAL OCUPADAS------//
            for (int i = 0; i < slotsEspecialCards.Length; i++)
            {
                if (slotsEspecialCards[i].slot.transform.childCount > 0)
                {
                    slotsEspecialCards[i].slot.GetComponent<BoxCollider2D>().enabled = false;
                }
            }
        }

        else if (!isChecking) //Reabilita todos os slots de cartas soldado e equipamento
        {
            //------CARTAS SOLDADO------//
            for (int i = 0; i < slotsSoldierCards.Length; i++)
            {
                slotsSoldierCards[i].slotSoldierCard.GetComponent<BoxCollider2D>().enabled = true;
            }

            //------CARTAS EQUIPAMENTOS------//
            for (int i = 0; i < slotsSoldierCards.Length; i++)
            {
                slotsSoldierCards[i].slotEquipmentCard.GetComponent<BoxCollider2D>().enabled = true;
            }

            //------CARTAS TEMPORAL OCUPADAS------//
            for (int i = 0; i < slotsEspecialCards.Length; i++)
            {
                slotsEspecialCards[i].slot.GetComponent<BoxCollider2D>().enabled = true;
            }
        }
    }

}


//--------------------------------------------CLASSES DOS SLOTS DAS CARTAS-----------------------------------------------------------------------------------//

//---------ARRAY DOS SLOTS DAS CARTAS SOLDADO---------//
[System.Serializable]
public class SlotsSoldierCards
{
    [SerializeField] private string slotName;

    public GameObject slotEquipmentCard;
    public GameObject slotSoldierCard;
    public GameObject slotMasks;

}

//---------ARRAY DOS SLOTS DAS CARTAS TEMPORAIS---------//
[System.Serializable]
public class SlotsTemporalCards
{
    [SerializeField] private string slotName;

    public GameObject slot;
    public GameObject slotMasks;

}