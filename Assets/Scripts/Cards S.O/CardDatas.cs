using System.Collections;
using UnityEngine;

public class CardDatas : ScriptableObject
{
    //Card Name
    public string cardNamePortuguese;
    public string cardNameEnglish;

    //Attack, resistance, defense and health
    public float attackAttribute;
    public float resistanceAttribute;
    public float defenseAttribute;
    public float healthAttribute;

    //Descriptions
    [TextArea] public string descriptionPortuguese;
    [TextArea] public string descriptionEnglish;

    //Another Card Is Needed
    public bool anotherCardIsNeeded;
    public GameObject cardSupport;
    public GameObject cardPrefab;

    //Image Card
    public Texture2D textureField;
    public Texture2D frameField;
}
