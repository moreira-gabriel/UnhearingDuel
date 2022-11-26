using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEditor;
using System.Text.RegularExpressions;

[CreateAssetMenu(fileName = "UI Translation", menuName = "Translation/UI Translation")]
public class UITranslationObject : ScriptableObject
{
    public string texto;

    public List<UISentences> uiTexts = new List<UISentences>();

    [Tooltip("Use this id to delete some text according to its order in the UI Texts list")]
    public int uiIdDelete;
}

[System.Serializable]
public class UISentences
{
    public string elemento;
    [HideInInspector] public string holdElemento;

    public UILanguages textos;
}

[System.Serializable]
public class UILanguages
{
    public string _portuguese;
    public string _english;
}

#if UNITY_EDITOR

[CustomEditor(typeof(UITranslationObject))]

public class BuildEditorUI : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        UITranslationObject uTranslation = (UITranslationObject)target;
        UILanguages l = new UILanguages();
        l._portuguese = uTranslation.texto;

        UISentences s = new UISentences();
        //s.actorName = uTranslation.texto;
        s.textos = l;

        if (GUILayout.Button("Create Text Translation"))
        {
            if(uTranslation.texto != "")
            {
                uTranslation.uiTexts.Add(s);
                s.elemento = uTranslation.uiTexts.IndexOf(s) + " - " + uTranslation.texto;
                uTranslation.texto = "";
                uTranslation.uiIdDelete = uTranslation.uiTexts.Count - 1;
            }
        }

        if(GUILayout.Button("Delete Text Translation"))
        {
            if(uTranslation.uiTexts.Count > 0)
            {
                //REMOVER A TRADUÇÃO DA LISTA
                uTranslation.uiTexts.RemoveAt(uTranslation.uiIdDelete);

                //ATUALIZAR O ÍNDICE DOS DIÁLOGOS ASSIM QUE APAGAR ALGUM DIÁLOGO
                for (int i = 0; i < uTranslation.uiTexts.Count; i++)
                {
                    //Guarda o Actor Name
                    s.holdElemento = string.Empty;
                    s.holdElemento = Regex.Replace(uTranslation.uiTexts[i].elemento, @"[\d-]", string.Empty);


                    uTranslation.uiTexts[i].elemento = string.Empty;
                    uTranslation.uiTexts[i].elemento = uTranslation.uiTexts.IndexOf(uTranslation.uiTexts[i]) + "-" + s.holdElemento;
                }


                if (uTranslation.uiTexts.Count == 0)
                {
                    uTranslation.uiIdDelete = 0;
                }
                else
                {
                    uTranslation.uiIdDelete = uTranslation.uiTexts.Count - 1;
                }
            }
        }
    }
}
#endif