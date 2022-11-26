using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TranslationManager : MonoBehaviour
{
    public UITranslationObject uiTranslationObject;
    public List<TMP_Text> _uiText = new List<TMP_Text>();
    public List<string> sentences = new List<string>();


    public enum uiIdiom
    {
        pt,
        eng,
    }

    public static TranslationManager instance;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        UpdateTexts();
    }

    public void UpdateTexts()
    {
        if(PlayerPrefs.GetInt("Language") == 0)
        {
            for(int i = 0; i < uiTranslationObject.uiTexts.Count; i++)
            {
                _uiText[i].text = uiTranslationObject.uiTexts[i].textos._portuguese;
            }
        }

        if(PlayerPrefs.GetInt("Language") == 1)
        {
            for(int i = 0; i < uiTranslationObject.uiTexts.Count; i++)
            {
                _uiText[i].text = uiTranslationObject.uiTexts[i].textos._english;
            }
        }
    }
 
}
