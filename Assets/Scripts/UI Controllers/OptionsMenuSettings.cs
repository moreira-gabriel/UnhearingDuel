using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// - Script responsável por controlar todas as opções de configuração do jogo, como: Resolução, idioma, modo janela, volume e outros.
/// </summary>

public class OptionsMenuSettings : MonoBehaviour
{
    //Drop Down Resoluções
    [SerializeField]
    private TMP_Dropdown dropdownResolutions;

    //Drop Down Qualidades
    [SerializeField]
    private TMP_Dropdown dropdownQuality;

    [SerializeField]
    private TMP_Dropdown dropdownLanguage;

    //Toggle Modo Janela
    [SerializeField]
    private Toggle windowMode;

    //Lista Resoluções
    private List<string> resolutionString = new List<string>();

    //Lista Qualidades
    private List<string> qualityString = new List<string>();

    [SerializeField] private TranslationManager[] translationManagers;

    void Start()
    {
        //----------ADICIONA AS RESOLUÇÕES ↓↓↓----------//

        Resolution[] resolutions = Screen.resolutions;
        dropdownResolutions.ClearOptions();

        foreach (var res in resolutions)
        {
            resolutionString.Add(res.width + "x" + res.height);
        }


        //Seta a resolução e a qualidade atual no dropdown
        dropdownQuality.value = QualitySettings.GetQualityLevel();

        //Adiciona todas as resoluções detectadas no foreach dentro do dropdown
        dropdownResolutions.AddOptions(resolutionString);

        //Quando o WindowMode for alterado executar método
        windowMode.onValueChanged.AddListener(delegate { WindowMode(); });

        //Quando o Dropdown de Resolução for alterado executar método
        dropdownResolutions.onValueChanged.AddListener(delegate { ResolutionDropdownValueChanged(dropdownResolutions, resolutions); });

        //Quando o Dropdown de Qualidade for alterado executar método
        dropdownQuality.onValueChanged.AddListener(delegate { QualityDropdownValueChanged(dropdownQuality); });

        //Quando o Dropdown de Idioma for alterado executar método
        dropdownLanguage.onValueChanged.AddListener(delegate {LanguageDropdownValueChanged(dropdownLanguage); });
    }

    //----------QUANDO O DROPDOWN DE RESOLUÇÃO MUDA ↓↓↓----------//
    void ResolutionDropdownValueChanged(TMP_Dropdown change, Resolution[] resolutions)
    {
        Screen.SetResolution(resolutions[change.value].width, resolutions[change.value].height, windowMode.isOn);
    }

    //----------QUANDO O DROPDOWN DE QUALIDADE MUDA ↓↓↓----------//
    void QualityDropdownValueChanged(TMP_Dropdown change)
    {
        if(change.value == 0)
        {
            GraphicLow();
        }else if(change.value == 1)
        {
            GraphicMedium();
        }else if(change.value == 2)
        {
            GraphicHigh();
        }
    }

    //----------QUANDO O DROPDOWN DE IDIOMA MUDA ↓↓↓----------//
    public void LanguageDropdownValueChanged(TMP_Dropdown change)
    {
        if(change.value == 0) //Português
        {
            PlayerPrefs.SetInt("Language", 0);
            for(int i = 0; i < translationManagers.Length; i++)
            {
                translationManagers[i].UpdateTexts();
            }
        }
        if (change.value == 1) //Inglês
        {
            PlayerPrefs.SetInt("Language", 1);
            for (int i = 0; i < translationManagers.Length; i++)
            {
                translationManagers[i].UpdateTexts();
            }
        }
    }

    //----------QUANDO O MODO JANELA MUDA ↓↓↓----------//
    void WindowMode()
    {
        Screen.fullScreen = windowMode.isOn;
    }

    //----------QUALIDADE DOS GRÁFICOS ↓↓↓----------//
    private void GraphicLow()
    {
        QualitySettings.SetQualityLevel(1, true);
    }
    private void GraphicMedium()
    {
        QualitySettings.SetQualityLevel(2, true);
    }
    private void GraphicHigh()
    {
        QualitySettings.SetQualityLevel(3, true);
    }

}
