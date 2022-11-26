using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEditor;
using UnityEngine.UI;
using System.Text.RegularExpressions;

[CreateAssetMenu(fileName = "Dialogue Object", menuName = "Translation/Dialogue Object")]
public class DialogueObject : ScriptableObject
{
    //SCRIPTABLE OBJECT CRIADO PARA F�CIL MUDAN�A DO IDIOMA NAS CONFIGURA��ES

    [Header("Text")]
    public string sentence;
    public List<Sentences> dialogues = new List<Sentences>();

    public int dialogueIdDelete; 
}

[System.Serializable]
public class Sentences
{
    public string actorName;
    [HideInInspector] public string holdActorName;

    public Image actorSprite;
    public Languages sentence;
}

[System.Serializable]
public class Languages
{
    [TextArea] public string portuguese;
    [TextArea] public string english;
}

#if UNITY_EDITOR
[CustomEditor(typeof(DialogueObject))]
public class DialogueEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        DialogueObject tObj = (DialogueObject)target;
        Languages l = new Languages();
        l.portuguese = tObj.sentence;

        Sentences s = new Sentences();
       
        s.sentence = l;

        if (GUILayout.Button("Create Dialogue"))
        {
            if (tObj.sentence != "")
            {
                tObj.dialogues.Add(s);

                s.actorName = tObj.dialogues.IndexOf(s) + " - " + tObj.sentence;

                tObj.sentence = "";
            }
        }

        if(GUILayout.Button("Delete Dialogue"))
        {
            if(tObj.dialogues.Count > 0)
            {
                //REMOVER O DI�LOGO DA LISTA
                tObj.dialogues.RemoveAt(tObj.dialogueIdDelete);
                
                //ATUALIZAR O �NDICE DOS DI�LOGOS ASSIM QUE APAGAR ALGUM DI�LOGO
                for(int i = 0; i < tObj.dialogues.Count; i++)
                {
                    //Guarda o Actor Name
                    s.holdActorName = string.Empty;
                    s.holdActorName = Regex.Replace(tObj.dialogues[i].actorName, @"[\d-]", string.Empty);


                    tObj.dialogues[i].actorName = string.Empty;
                    tObj.dialogues[i].actorName = tObj.dialogues.IndexOf(tObj.dialogues[i]) + "-" + s.holdActorName;
                }

                //CASO O DI�LOGO SEJA ZERO O �NDICE DE DELETE VAI RECEBER ZERO
                if (tObj.dialogues.Count == 0)
                {
                    tObj.dialogueIdDelete = 0;
                }
                else //CASO CONTR�RIO SER� A QUANTIDADE DE DI�LOGOS -1
                {
                    tObj.dialogueIdDelete = tObj.dialogues.Count - 1;
                }
            }
        }
    }
}

#endif
