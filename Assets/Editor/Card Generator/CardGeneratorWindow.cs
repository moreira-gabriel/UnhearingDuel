using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

public class CardGeneratorWindow : EditorWindow
{
    #region Scroll
    Vector2 scrollDescription;
    #endregion


    #region GUISkin
    GUISkin skin;
    #endregion guiskin  


    #region ToolBars
    //-----------TOOLBARS-----------//

    //Desenhar as abas toolBars
    //???????? Esses nomes devem ser os nomes das primeiras pastas onde as cartas se encontram (EX: Resources > Soldiers Cards) ????????
    string[] toolBarsStrings = { "Soldier Cards", "Equipment Cards", "Temporal Cards", "Special Cards", "Support Cards" }; //Esses nomes devem ser os nomes das primeiras pastas onde as cartas se encontram (EX: Resources > Soldiers Cards)
    string[] subFoldersToolBar;

    int toolBarSelected = 0;
    int subToolBarSelected = 0;
    int previousToolBarSelected = -1;

    List<Texture2D> textures = new List<Texture2D>();
    public object[] myTextures;

    Vector2 scrollViewTypes; //Vector 2 do scrollview dos tipos de cartas
    Vector2 scrollViewCards; //Vector 2 do scrollview das cartas

    //-----------TOOLBARS-----------//
    #endregion toolBars


    #region Cards
    //-----------CARDS-----------//
    string[] cardRaritiesString = new string[6] { "1 - Common", "2 - Uncommon", "3 - Rare", "4 - Super Rare", "5 - Heroic", "6 - Legendary" }; //Nome deve ser igual ao nome das pastas dentro da Unity
    Texture2D[] cardRaritiesTexture = new Texture2D[6];
    GUIContent[] cardRarities = new GUIContent[6];

    //Salvar as descrições em PT e EN
    string descriptionPortuguese;
    string descriptionEnglish;

    //Salvar o nome da carta em PT e EN
    string cardNamePortuguese;
    string cardNameEnglish;

    //Atributos Básicos da Carta
    float defenseAttribute;
    float attackAttribute;
    float resistanceAttribute;
    float healthAttribute;

    //Valor dos toolBars
    int cardDescriptionSelected;
    int cardRaritySelected;
    int cardNameLanguage;

    //Atributos especiais da carta
    bool anotherCardIsNeeded;
    GameObject cardSupport;
    GameObject cardPrefab;

    enum enumCardTypes
    {
        Soldier,
        Equipment,
        Temporal,
        Special,
        Support
    }

    enumCardTypes cardTypes;

    Texture2D textureField;
    Texture2D frameField;

    Rect imageCardRect;
    Rect frameCardRect;

    Vector2 attributesScroll;

    //Scriptable Objects que serviram de base para criar as cartas
    static SoldierData soldierCard;
    static EquipmentData equipmentCard;
    static TemporalData temporalCard;
    static SpecialData specialCard;
    static SupportData supportCard;

    public static SoldierData SoldierInfo {get{ return soldierCard; }}
    public static EquipmentData EquipmentInfo { get { return equipmentCard; } }
    public static TemporalData TemporalInfo { get { return temporalCard; } }
    public static SpecialData SpecialInfo { get { return specialCard; } }
    public static SupportData SupportInfo { get { return supportCard; } }

    //-----------CARDS-----------//
    #endregion cards


    #region Header
    //-----------HEADER-----------//

    bool cardCreate;
    bool editCard;
    bool clearData;

    //-----------HEADER-----------//
    #endregion header


    #region Sections
    //-----------SECTIONS-----------//

    //Espessura da borda
    Vector4 edgeThickness = new Vector4(0, 0, 0.6f, 0.6f);
    //Arredondamento da borda
    float edgeRounding = 0.1f;

    //------------Header Section------------//
    Texture2D headerSectionTexture; //Textura do cabeçalho
    Texture2D cardCreateTexture; //Textura escrito 'Card Create' que fica na região do cabeçalho
    
    Color headerSectionColor = new Color(56 / 255f, 56 / 255f, 56 / 255f, 0.99f);
    Color cardCreateColor = new Color(42f / 255f, 42f / 255f, 42f / 255, 0.99f);
    Rect headerSection;


    //------------Cards Section------------//
    Texture2D cardsSectionTexture;
    Color cardsSectionColor = new Color(56 / 255f, 56 / 255f, 56 / 255f, 0.99f);
    Rect cardTypesSection;


    //------------Cards List Section------------//
    Texture2D cardListSectionTexture;
    Color cardListSectionColor = new Color(56 / 255f, 56 / 255f, 56 / 255f, 0.99f);
    Rect cardListSection;


    //------------Image Card Section------------//
    Texture2D imageCardSectionTexture;
    Color imageCardSectionColor = new Color(56 / 255f, 56 / 255f, 56 / 255f, 0.99f);
    Rect imageCardSection;


    //------------Attribute Section------------//
    Texture2D attributeSectionTexture;
    Color attributeSectionColor = new Color(56 / 255f, 56 / 255f, 56 / 255f, 0.99f);
    Rect attributeSection;

    float spaceHorizontalAttributeContent; //Espaço Horizontal do conteúdo da seção de atributos.
    float spaceVerticalAttributeContent; //Espaço Vertical de conteúdo da seção de atributos.
                                         //-----------SECTIONS-----------//
    #endregion sections


    #region Rarity Colors
    GUIStyle common = new GUIStyle();
    GUIStyle uncommon = new GUIStyle();
    GUIStyle rare = new GUIStyle();
    GUIStyle superRare = new GUIStyle();
    GUIStyle heroic = new GUIStyle();
    GUIStyle legendary = new GUIStyle();

    GUIStyle[] rarityStyles = new GUIStyle[6];

    string[] rarityStrings = new string[6] { "Cinza", "Verde", "Azul", "Roxo", "Rosa", "Amarelo" };
    #endregion rarity colors


    #region Verify Attributes

    int numberOfAttributes;

    #endregion verify attributes


    #region Edit Card

    CardDatas editCardData;
    Texture2D imageCardData;

    string imagePath;
    string editImage;
    string oldPrefabPath;
    #endregion edit card



    [MenuItem("U.D Project/Card Generator %g")]
    /// <summary>
    /// Método responsável por criar o painel
    /// </summary>
    static void CreateWindow()
    {
        CardGeneratorWindow window = (CardGeneratorWindow)EditorWindow.GetWindow(typeof(CardGeneratorWindow));

        window.titleContent.text = "U.D Card Generator";
        window.Focus();

        window.ShowAsDropDown(new Rect(Screen.width/2,Screen.height/4,0,0),new Vector2(700,500));
    }




    /// <summary>
    /// Inicia as texturas assim que o painel for aberto
    /// </summary>
    private void OnEnable()
    {
        InitData();
        InitTextures();
        skin = Resources.Load<GUISkin>("Card Generator Window/GuiSkin/Card Generator Window Skin");
    }




    /// <summary>
    /// Inicia todos os scriptable objects bases
    /// </summary>
    public static void InitData()
    {
        soldierCard = (SoldierData)ScriptableObject.CreateInstance(typeof(SoldierData));
        equipmentCard = (EquipmentData)ScriptableObject.CreateInstance(typeof(EquipmentData));
        temporalCard = (TemporalData)ScriptableObject.CreateInstance(typeof(TemporalData));
        specialCard = (SpecialData)ScriptableObject.CreateInstance(typeof(SpecialData));
        supportCard = (SupportData)ScriptableObject.CreateInstance(typeof(SupportData));
    }




    /// <summary>
    /// Inicializa todas as texturas
    /// </summary>
    void InitTextures()
    {
        //--------Header Texture--------//
        headerSectionTexture = new Texture2D(1, 1);
        headerSectionTexture.SetPixel(0, 0, headerSectionColor);
        headerSectionTexture.Apply();

        cardCreateTexture = new Texture2D(1, 1);
        cardCreateTexture.SetPixel(0, 0, cardCreateColor);
        cardCreateTexture.Apply();

        //--------Card Section Texture--------//

        cardsSectionTexture = new Texture2D(1, 1);
        cardsSectionTexture.SetPixel(0, 0, cardsSectionColor);
        cardsSectionTexture.Apply();


        //--------Card List Section Texture--------//

        cardListSectionTexture = new Texture2D(1, 1);
        cardListSectionTexture.SetPixel(0, 0, cardListSectionColor);
        cardListSectionTexture.Apply();


        //--------Image Card Section Texture--------//

        imageCardSectionTexture = new Texture2D(1, 1);
        imageCardSectionTexture.SetPixel(0, 0, imageCardSectionColor);
        imageCardSectionTexture.Apply();


        //--------Attribute Section Texture--------//

        attributeSectionTexture = new Texture2D(1, 1);
        attributeSectionTexture.SetPixel(0, 0, attributeSectionColor);
        attributeSectionTexture.Apply();
    }




    /// <summary>
    /// Chama todos os métodos que vão desenhar algo na tela
    /// </summary>
    private void OnGUI()
    {
        GUI.skin.font = skin.GetStyle("Panel").font;
        GUI.backgroundColor = new Color(0, 0, 0, 0.2f);
        DrawBackground(); //Desenha a imagem de background do painel 
        DrawLayouts(); //Desenha as seções
        

        DrawHeader(); //Desenha o cabeçalho
        DrawToolBars(); //Desenha os botões 
        DrawImageCards(); //Desenha o campo field onde aparece a imagem da carta
        if (cardCreate) { DrawAttributes(); DrawPreviewAttributes(); } //Desenha tudo que estiver na seção Atributo
        if (editCard) { DrawEditAttributes(); DrawPreviewAttributes(); } //Desenha a região onde podem ser editados os atributos de uma carta


        GUI.backgroundColor = new Color(250, 250, 250, 1f);
        if (cardCreate) { DrawBars(); } //Desenha as barras de status (Attack, Defense, Resistance, Health)

    }



    /// <summary>
    /// A imagem de background
    /// </summary>
    void DrawBackground()
    {
        Texture2D backgroundTexture = Resources.Load<Texture2D>("Card Generator Window/Layout Textures/UNHEARING_DUEL_LOGO_BRANCO");
        Color backgroundColor = new Color(50, 50, 50, 255f);
        
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), backgroundTexture,ScaleMode.ScaleAndCrop,true,0, backgroundColor,0,0);
    }




    /// <summary>
    /// Desenha todos so layouts
    /// </summary>
    void DrawLayouts()
    {
        //----------------------------HEADER SECTION----------------------------//

        //Posições do cabeçalho
        headerSection.x = 0;
        headerSection.y = 0;

        //Largura e altura do cabeçalho
        headerSection.width = Screen.width / 3.5f;
        headerSection.height = 100;

        GUI.DrawTexture(headerSection, headerSectionTexture);
        GUI.DrawTexture(headerSection, headerSectionTexture,ScaleMode.ScaleAndCrop, true, 1, headerSectionColor, edgeThickness, edgeRounding);

        
        //----------------------------CARD TYPES SECTION----------------------------//

        //Posições da area onde ficam os tipos de cartas
        cardTypesSection.x = 0;
        cardTypesSection.y = 100;

        //Largura e altura da area onde ficam os tipos de cartas
        //cardTypesSection.width = 200;
        cardTypesSection.width = Screen.width / 3.5f;
        cardTypesSection.height = 70;

        GUI.DrawTexture(cardTypesSection, cardsSectionTexture);
        GUI.DrawTexture(cardTypesSection, cardsSectionTexture, ScaleMode.ScaleAndCrop, true, 1, cardsSectionColor, edgeThickness, edgeRounding);


        //----------------------------LIST OF CARDS SECTION----------------------------//

        //Posições da area das cartas
        cardListSection.x = 0;
        cardListSection.y = 170;

        //Largura e altura da area das cartas
        //cardListSection.width = 200;
        cardListSection.width = Screen.width / 3.5f;
        cardListSection.height = Screen.height - 170;

        GUI.DrawTexture(cardListSection, cardListSectionTexture);
        GUI.DrawTexture(cardListSection, cardListSectionTexture, ScaleMode.ScaleAndCrop, true, 1, cardListSectionColor, edgeThickness, edgeRounding);


        //----------------------------IMAGE CARD SECTION----------------------------//

        //Posições da area que aparece a image das cartas
        imageCardSection.x = Screen.width / 3.5f; 
        imageCardSection.y = 0;

        //Largura e altura da area que aparece a imagem das cartas
        imageCardSection.width = Screen.width - Screen.width / 3.5f; 
        imageCardSection.height = Screen.height / 2;

        GUI.DrawTexture(imageCardSection, imageCardSectionTexture);
        GUI.DrawTexture(imageCardSection, imageCardSectionTexture, ScaleMode.ScaleAndCrop, true, 1, imageCardSectionColor, new Vector4(0,0,0,edgeThickness.w), edgeRounding);


        //----------------------------ATTRIBUTE SECTION----------------------------//

        //Posições da area que aparecem os atributos da carta
        attributeSection.x = Screen.width / 3.5f; 
        attributeSection.y = Screen.height - Screen.height/2;

        //Largura e altura da area que aparecem os atributos da carta
        attributeSection.width = Screen.width - Screen.width / 3.5f; 
        attributeSection.height = Screen.height / 2;

        GUI.DrawTexture(attributeSection, attributeSectionTexture);
        GUI.DrawTexture(attributeSection, attributeSectionTexture, ScaleMode.ScaleAndCrop, true, 1, attributeSectionColor, new Vector4(0, 0, 0, edgeThickness.w), edgeRounding);        
    }




    /// <summary>
    /// Controla o conteúdo da região do cabeçalho
    /// </summary>
    void DrawHeader()
    {
        #region Header Attribute Style
        var headerButtonStyle = new GUIStyle(EditorStyles.miniButtonMid);
        headerButtonStyle.alignment = TextAnchor.MiddleCenter;

        headerButtonStyle.fixedHeight = 50;

        headerButtonStyle.font = skin.GetStyle("Header & Create").font;
        headerButtonStyle.fontSize = 25;
        headerButtonStyle.fontStyle = FontStyle.Normal;
        headerButtonStyle.richText = true;
        #endregion

        #region BeginArea, BeginVertical, Guilayout.Space
        GUILayout.BeginArea(headerSection);

        GUILayout.BeginVertical();
        GUILayout.Space(headerSection.height/4);
        #endregion

        //Botão toggle para desenhar o painel de criação de cartas
        cardCreate = GUILayout.Toggle(cardCreate, "Card Create", headerButtonStyle, GUILayout.ExpandWidth(true) /*headerSection.height / 2*/);
        
        #region Verifications
        //Se cardCreate for verdadeiro, edit card é falso e clear data verdadeiro
        if (cardCreate) 
        { 
            editCard = false;
            clearData = true;
        }
        //Se clearData for verdadeiro chama o método ClearData() e depois fica falso novamente
        if (!clearData)
        {
            ClearData();
            clearData = true;
        }
        #endregion

        #region EndVertical, EndArea
        GUILayout.EndVertical();
        GUILayout.EndArea();
        #endregion

    }




    /// <summary>
    /// Controla o conteúdo da região das barras de ferramentas (Soldier, Equipment e Temporal) 
    /// </summary>
    void DrawToolBars()
    {
        float buttonHorizontalSpace = (cardTypesSection.width - 175) / 2;

        GUILayout.BeginArea(cardTypesSection);
        GUILayout.BeginVertical(GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
        scrollViewTypes = GUILayout.BeginScrollView(scrollViewTypes,GUILayout.Width(cardTypesSection.width),GUILayout.Height(cardTypesSection.height - 1));
        
        toolBarSelected = GUILayout.SelectionGrid(toolBarSelected, toolBarsStrings, 1);

        GUILayout.EndScrollView();
        GUILayout.EndVertical();
        GUILayout.EndArea();


        switch (toolBarSelected)
        {
            case 0:
                LoadPanel("Assets/Resources/" + toolBarsStrings[0]);
                break;
            case 1:
                LoadPanel("Assets/Resources/" + toolBarsStrings[1]);
                break;
            case 2:
                LoadPanel("Assets/Resources/" + toolBarsStrings[2]);
                break;
            case 3:
                LoadPanel("Assets/Resources/" + toolBarsStrings[3]);
                break;
            case 4:
                LoadPanel("Assets/Resources/" + toolBarsStrings[4]);
                break;

        }

        //////---------------------------------------LOAD PANEL---------------------------------------//////     

        void LoadPanel(string folderName)
    {
        if(toolBarSelected != previousToolBarSelected)
        {
            string[] folderNames = Directory.GetDirectories(folderName);
            List<string> subToolBars = new List<string>();

            if (folderNames != null)
            {
                foreach (string Folder in folderNames)
                {
                    string foldername = Path.GetFileName(Folder);
                    subToolBars.Add(foldername);
                }
            }
            subFoldersToolBar = subToolBars.ToArray();
            previousToolBarSelected = toolBarSelected;
            subToolBarSelected = 0;

            
        }

            GUILayout.BeginArea(cardListSection);
            EditorGUILayout.BeginVertical();
            scrollViewCards = EditorGUILayout.BeginScrollView(scrollViewCards, GUILayout.Width(Screen.width / 3.5f), GUILayout.Height(cardListSection.height));

            if (subFoldersToolBar.Length > 0)
            {
                subToolBarSelected = EditorGUILayout.Popup(subToolBarSelected, subFoldersToolBar, GUILayout.ExpandWidth(true));

                MakeWindow(toolBarsStrings[toolBarSelected] + "/" + subFoldersToolBar[subToolBarSelected]);
            }
            else
            {
                MakeWindow(toolBarsStrings[toolBarSelected]);
            }

            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
            GUILayout.EndArea();
        }
    
        void MakeWindow(string TextureFolder)
        {
            textures.Clear();
            
            if (textures.Count == 0)
            {
                myTextures = Resources.LoadAll(TextureFolder, typeof(Texture2D));

                for (int i = 0; i < myTextures.Length; i++)
                {
                    textures.Add((Texture2D)myTextures[i]);
                }

                switch (toolBarSelected)
                {
                    case 0:
                        SoldierCardData();
                        break;
                    case 1:
                        EquipmentData();
                        break;
                    case 2:
                        TemporalData();
                        break;
                    case 3:
                        SpecialData();
                        break;
                    case 4:
                        SupportData();
                        break;
                }

                void SoldierCardData()
                {
                    for (int i = 0; i < textures.Count; i++)
                    {
                        SoldierData[] descriptionData = Resources.LoadAll<SoldierData>(TextureFolder);

                        GUILayout.BeginVertical(GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));

                        DescriptionDetails();
                        GUILayout.BeginHorizontal();
                        GUILayout.Space(buttonHorizontalSpace);
                        if (GUILayout.Button(textures[i], GUILayout.Width(175), GUILayout.Height(175)))
                        {
                            cardCreate = false;                            
                            editCard = true;

                            oldPrefabPath = "Assets/Prefabs/Cards/" + toolBarsStrings[toolBarSelected] + "/" + textures[i].name + ".prefab";
                            editImage = "Assets/Resources/" + toolBarsStrings[toolBarSelected] + "/" + subFoldersToolBar[subToolBarSelected] + "/" + textures[i].name + ".png";   
                            editCardData = (SoldierData)Resources.Load<CardDatas>(TextureFolder + "/" + textures[i].name);
                            imageCardData = Resources.Load<Texture2D>(TextureFolder + "/" + textures[i].name);

                            DrawEditAttributes(editCardData, enumCardTypes.Soldier,subToolBarSelected);
                        }
                        GUILayout.EndHorizontal();
                        GUILayout.EndVertical();

                        void DescriptionDetails()
                        {
                            GUILayout.Box(new GUIContent(textures[i].name,
                                  "Attack: " + descriptionData[i].attackAttribute + " " + "Defense: " + descriptionData[i].defenseAttribute + " "
                                  + "Resistance: " + descriptionData[i].resistanceAttribute + " " + "Health: " + descriptionData[i].healthAttribute + " "
                                  + "Description:" + " " + descriptionData[i].descriptionPortuguese), GUILayout.ExpandWidth(true),GUILayout.Height(25));
                        }
                    }
                }

                void EquipmentData()
                {
                    for (int i = 0; i < textures.Count; i++)
                    {
                        EquipmentData[] descriptionData = Resources.LoadAll<EquipmentData>(TextureFolder);

                        GUILayout.BeginVertical("box", GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));

                        DescriptionDetails();
                        GUILayout.BeginHorizontal();
                        GUILayout.Space(buttonHorizontalSpace);
                        if (GUILayout.Button(textures[i], GUILayout.Width(175), GUILayout.Height(175)))
                        {
                            cardCreate = false;
                            editCard = true;

                            oldPrefabPath = "Assets/Prefabs/Cards/" + toolBarsStrings[toolBarSelected] + "/" + textures[i].name + ".prefab";
                            editImage = "Assets/Resources/" + toolBarsStrings[toolBarSelected] + "/" + subFoldersToolBar[subToolBarSelected] + "/" + textures[i].name + ".png";
                            editCardData = (EquipmentData)Resources.Load<CardDatas>(TextureFolder + "/" + textures[i].name);
                            imageCardData = Resources.Load<Texture2D>(TextureFolder + "/" + textures[i].name);

                            DrawEditAttributes(editCardData, enumCardTypes.Equipment, subToolBarSelected);
                        }
                        GUILayout.EndHorizontal();
                        GUILayout.EndVertical();

                        void DescriptionDetails()
                        {
                            GUILayout.Box(new GUIContent(textures[i].name,"Description:" + " " + descriptionData[i].descriptionPortuguese), GUILayout.ExpandWidth(true), GUILayout.Height(25));
                        }
                    }
                }

                void TemporalData()
                {
                    for (int i = 0; i < textures.Count; i++)
                    {
                        TemporalData[] descriptionData = Resources.LoadAll<TemporalData>(TextureFolder);

                        GUILayout.BeginVertical("box", GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));

                        DescriptionDetails();
                        GUILayout.BeginHorizontal();
                        GUILayout.Space(buttonHorizontalSpace);
                        if (GUILayout.Button(textures[i], GUILayout.Width(175), GUILayout.Height(175)))
                        {
                            cardCreate = false;
                            editCard = true;

                            oldPrefabPath = "Assets/Prefabs/Cards/" + toolBarsStrings[toolBarSelected] + "/" + textures[i].name + ".prefab";
                            editImage = "Assets/Resources/" + toolBarsStrings[toolBarSelected] + "/" + subFoldersToolBar[subToolBarSelected] + "/" + textures[i].name + ".png";
                            editCardData = (TemporalData)Resources.Load<CardDatas>(TextureFolder + "/" + textures[i].name);
                            imageCardData = Resources.Load<Texture2D>(TextureFolder + "/" + textures[i].name);

                            DrawEditAttributes(editCardData, enumCardTypes.Temporal, subToolBarSelected);
                        }
                        GUILayout.EndHorizontal();
                        GUILayout.EndVertical();

                        void DescriptionDetails()
                        {
                            GUILayout.Box(new GUIContent(textures[i].name, "Description:" + " " + descriptionData[i].descriptionPortuguese), GUILayout.ExpandWidth(true), GUILayout.Height(25));
                        }
                    }
                }

                void SpecialData()
                {
                    for (int i = 0; i < textures.Count; i++)
                    {
                        SpecialData[] descriptionData = Resources.LoadAll<SpecialData>(TextureFolder);

                        GUILayout.BeginVertical("box", GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));

                        DescriptionDetails();
                        GUILayout.BeginHorizontal();
                        GUILayout.Space(buttonHorizontalSpace);
                        if (GUILayout.Button(textures[i], GUILayout.Width(175), GUILayout.Height(175)))
                        {
                            cardCreate = false;
                            editCard = true;

                            oldPrefabPath = "Assets/Prefabs/Cards/" + toolBarsStrings[toolBarSelected] + "/" + textures[i].name + ".prefab";
                            editImage = "Assets/Resources/" + toolBarsStrings[toolBarSelected] + "/" + subFoldersToolBar[subToolBarSelected] + "/" + textures[i].name + ".png";
                            editCardData = (SpecialData)Resources.Load<CardDatas>(TextureFolder + "/" + textures[i].name);
                            imageCardData = Resources.Load<Texture2D>(TextureFolder + "/" + textures[i].name);

                            DrawEditAttributes(editCardData, enumCardTypes.Special, subToolBarSelected);
                        }
                        GUILayout.EndHorizontal();
                        GUILayout.EndVertical();

                        void DescriptionDetails()
                        {
                            GUILayout.Box(new GUIContent(textures[i].name, "Description:" + " " + descriptionData[i].descriptionPortuguese), GUILayout.ExpandWidth(true), GUILayout.Height(25));
                        }
                    }
                }

                void SupportData()
                {
                    for (int i = 0; i < textures.Count; i++)
                    {
                        SupportData[] descriptionData = Resources.LoadAll<SupportData>(TextureFolder);

                        GUILayout.BeginVertical("box", GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));

                        DescriptionDetails();
                        GUILayout.BeginHorizontal();
                        GUILayout.Space(buttonHorizontalSpace);
                        if (GUILayout.Button(textures[i], GUILayout.Width(175), GUILayout.Height(175)))
                        {
                            cardCreate = false;
                            editCard = true;

                            oldPrefabPath = "Assets/Prefabs/Cards/" + toolBarsStrings[toolBarSelected] + "/" + textures[i].name + ".prefab";
                            editImage = "Assets/Resources/" + toolBarsStrings[toolBarSelected] + "/" + subFoldersToolBar[subToolBarSelected] + "/" + textures[i].name + ".png";
                            editCardData = (SupportData)Resources.Load<CardDatas>(TextureFolder + "/" + textures[i].name);
                            imageCardData = Resources.Load<Texture2D>(TextureFolder + "/" + textures[i].name);

                            DrawEditAttributes(editCardData, enumCardTypes.Support, subToolBarSelected);
                        }
                        GUILayout.EndHorizontal();
                        GUILayout.EndVertical();

                        void DescriptionDetails()
                        {
                            GUILayout.Box(new GUIContent(textures[i].name, "Description:" + " " + descriptionData[i].descriptionPortuguese), GUILayout.ExpandWidth(true), GUILayout.Height(25));
                        }
                    }
                }
            }
        }
   
    }




    /// <summary>
    /// Controla o conteúdo da região onde aparece a imagem da carta 
    /// </summary>
    void DrawImageCards()
    {
        #region Frame Card Rect
        frameCardRect.x = (Screen.width - Screen.width / 3.5f) / 4;
        frameCardRect.y = 0;

        frameCardRect.width = imageCardSection.width / 2;
        frameCardRect.height = imageCardSection.height;
        #endregion


        #region Image Card Rect
        imageCardRect.x = (Screen.width - Screen.width / 3.5f) / 4;
        imageCardRect.y = imageCardSection.height / 5.5f;

        imageCardRect.width = imageCardSection.width / 2;
        imageCardRect.height = imageCardSection.height/1.8f;
        #endregion


        GUILayout.BeginArea(imageCardSection);

        //Caso esteja criando uma carta, mostrar a imagem do textureField
        if (cardCreate)
        {
            if (textureField)
            {
                EditorGUI.DrawPreviewTexture(imageCardRect, textureField, null, ScaleMode.ScaleToFit);
            }
            if (frameField)
            {
                Color guiColor = GUI.color; // Save the current GUI color
                GUI.color = Color.clear;
                
                EditorGUI.DrawTextureTransparent(frameCardRect, frameField, ScaleMode.ScaleToFit);

                GUI.color = guiColor; // Get back to previous GUI color
            }
        }
        
        //Caso esteja editando uma carta, mostrar também a imagem do textureField
        else if (editCard)
        {
            if (textureField)
            {
                EditorGUI.DrawPreviewTexture(imageCardRect, textureField, null, ScaleMode.ScaleToFit);                
            }
            if (frameField)
            {
                Color guiColor = GUI.color; // Save the current GUI color
                GUI.color = Color.clear;

                EditorGUI.DrawTextureTransparent(frameCardRect, frameField, ScaleMode.ScaleToFit);

                GUI.color = guiColor; // Get back to previous GUI color
            }
        }
     
        GUILayout.EndArea();
    }



    /// <summary>
    /// Desenha o Preview das informações do gerador 
    /// </summary>
    void DrawPreviewAttributes()
    {
        #region GUI STYLE        
        //Name STYLE
        var nameGUISTYLE = new GUIStyle(EditorStyles.label);
        nameGUISTYLE.fontSize = skin.GetStyle("Name Preview").fontSize;
        nameGUISTYLE.richText = true;
        nameGUISTYLE.alignment = TextAnchor.MiddleCenter;
        nameGUISTYLE.normal.textColor = Color.black;
        nameGUISTYLE.font = skin.GetStyle("Name Preview").font;

        //Description STYLE
        var descriptionGUIStyle = new GUIStyle(EditorStyles.label);
        descriptionGUIStyle.fontSize = skin.GetStyle("Description Preview").fontSize;
        descriptionGUIStyle.richText = true;
        descriptionGUIStyle.alignment = TextAnchor.UpperLeft;
        descriptionGUIStyle.normal.textColor = Color.black;
        descriptionGUIStyle.font = skin.GetStyle("Description Preview").font;
        #endregion

        float space = ((Screen.width - Screen.width / 3.5f)) / 3 + Screen.width / 3.5f;

        //Card Name Preview
        EditorGUI.LabelField(new Rect(space + 10, -45, 147, 150), cardNamePortuguese, nameGUISTYLE);

        //Card Description Preview     
        EditorGUI.LabelField(new Rect(space + 10, (imageCardSection.height / 3) + 105, 147, 50), descriptionPortuguese, descriptionGUIStyle);

    }



    /// <summary>
    /// Controla o conteúdo da região onde ficam os atributos
    /// </summary>
    void DrawAttributes()
    {
        spaceHorizontalAttributeContent = ((Screen.width - Screen.width / 3.5f) - 390) / 2;
        spaceVerticalAttributeContent = 20;

        #region Attributes Styles
        //Attributes Style
        var attributeHeaderStyle = new GUIStyle(EditorStyles.label);
        attributeHeaderStyle.fontSize = 18;
        attributeHeaderStyle.fontStyle = FontStyle.Bold;
        attributeHeaderStyle.richText = true;
        attributeHeaderStyle.alignment = TextAnchor.UpperCenter;

        //Attributes Text Style
        var attributeTextStyle = new GUIStyle(EditorStyles.label);
        attributeTextStyle.fontStyle = FontStyle.Normal;
        attributeTextStyle.alignment = TextAnchor.MiddleLeft;
       
        //Attributes TextArea Style
        var attributeTextAreaStyle = new GUIStyle(EditorStyles.textArea);
        attributeTextAreaStyle.fixedWidth = 385;

        //Attributes TextField Style
        var attributeTextFieldStyle = new GUIStyle(EditorStyles.textField);
        attributeTextFieldStyle.alignment = TextAnchor.MiddleLeft;
        attributeTextFieldStyle.fixedWidth = 200;

        //Attributes Enum Style
        var attributeEnumStyle = new GUIStyle(EditorStyles.popup);
        attributeEnumStyle.alignment = TextAnchor.MiddleCenter;

        //Attributes ObjectField Style
        var attributeObjectFieldStyle = new GUIStyle(EditorStyles.objectField);
        attributeObjectFieldStyle.alignment = TextAnchor.MiddleLeft;

        //Attributes Label Style
        var attributeLabelStyle = new GUIStyle(EditorStyles.label);
        attributeLabelStyle.alignment = TextAnchor.MiddleLeft;
        attributeLabelStyle.richText = false;
        attributeLabelStyle.fontStyle = FontStyle.Normal;
        attributeLabelStyle.fontSize = 13;
       

        //Attributes Button Style
        var attributeButtonStyle = new GUIStyle(EditorStyles.miniButton);
        attributeButtonStyle.alignment = TextAnchor.MiddleCenter;

        attributeButtonStyle.fixedHeight = 60;
        attributeButtonStyle.fixedWidth = 300;

        attributeButtonStyle.normal.textColor = new Color(255f, 255f, 255f);
        attributeButtonStyle.hover.textColor = new Color(100, 100, 100);

        attributeButtonStyle.font = skin.GetStyle("Header & Create").font;
        attributeButtonStyle.richText = true;
        attributeButtonStyle.fontSize = 30;
        attributeButtonStyle.fontStyle = FontStyle.Normal;

        //Attributes ToolBar Style
        var attributeToolBarStyle = new GUIStyle();
        attributeToolBarStyle.alignment = TextAnchor.MiddleLeft;
        attributeToolBarStyle.fontStyle = FontStyle.Normal;
        attributeToolBarStyle.richText = true;
        attributeToolBarStyle.fixedHeight = 25;


        //Color of Rarities
        rarityStyles[0] = common;
        rarityStyles[1] = uncommon;
        rarityStyles[2] = rare;
        rarityStyles[3] = superRare;
        rarityStyles[4] = heroic;
        rarityStyles[5] = legendary;

        common.normal.textColor = new Color(155,164,178,1);
        uncommon.normal.textColor = new Color(152,222,90,1);
        rare.normal.textColor = new Color(89,226,232,1);
        superRare.normal.textColor = new Color(114, 64, 170, 1);
        heroic.normal.textColor = new Color(203,71,85,1);
        legendary.normal.textColor = new Color(247,251,47,1);
        #endregion attributes styles

        #region Begin Area, Begin Vertical, Begin ScrollView
        
        GUILayout.BeginArea(attributeSection);
        GUILayout.BeginVertical();

        attributesScroll = EditorGUILayout.BeginScrollView(attributesScroll, GUILayout.Width(attributeSection.width), GUILayout.Height(attributeSection.height - 20));
        #endregion

        #region Attributes        
        
        //Attributes Text
        EditorGUILayout.LabelField("ATTRIBUTES", attributeHeaderStyle);
        GUILayout.Space(spaceVerticalAttributeContent);


        //Card Type
        GUILayout.BeginHorizontal();
        GUILayout.Space(spaceHorizontalAttributeContent);
        EditorGUILayout.LabelField("Card Type:", attributeLabelStyle, GUILayout.MaxWidth(85));
        cardTypes = (enumCardTypes)EditorGUILayout.EnumPopup(cardTypes, attributeEnumStyle, GUILayout.MaxWidth(100));
        GUILayout.EndHorizontal();

       
        //Card Image
        GUILayout.BeginHorizontal();
        GUILayout.Space(spaceHorizontalAttributeContent);
        EditorGUILayout.LabelField("Card Image:", attributeLabelStyle, GUILayout.Width(85));
        textureField = (Texture2D)EditorGUILayout.ObjectField(textureField, typeof(Texture2D), false, GUILayout.MaxWidth(305));
        GUILayout.EndHorizontal();


        //Card Frame (Moldura da carta)
        GUILayout.BeginHorizontal();
        GUILayout.Space(spaceHorizontalAttributeContent);
        EditorGUILayout.LabelField("Card Frame:", attributeLabelStyle, GUILayout.Width(85));
        frameField = (Texture2D)EditorGUILayout.ObjectField(frameField, typeof(Texture2D), false, GUILayout.MaxWidth(305));
        GUILayout.EndHorizontal();


        //Card Name
        GUILayout.BeginHorizontal();
        GUILayout.Space(spaceHorizontalAttributeContent);
        GUILayout.Label("Card Name:", attributeLabelStyle, GUILayout.Width(85));
        cardNameLanguage = EditorGUILayout.Popup(cardNameLanguage, new string[] { "Portuguese", "English" }, attributeEnumStyle, GUILayout.MaxWidth(100));
        CardNameAttribute(attributeTextFieldStyle);
        GUILayout.EndHorizontal();
        

        switch (cardTypes)
        {
            case enumCardTypes.Soldier:
                //Attack
                GUILayout.BeginHorizontal();
                GUILayout.Space(spaceHorizontalAttributeContent);
                GUILayout.Label("Attack:", attributeLabelStyle, GUILayout.Width(85));
                attackAttribute = EditorGUILayout.IntSlider(Mathf.RoundToInt(attackAttribute), 0, 10, GUILayout.Width(300));
                GUILayout.EndHorizontal();


                //Defense
                GUILayout.BeginHorizontal();
                GUILayout.Space(spaceHorizontalAttributeContent);
                GUILayout.Label("Defense:", attributeLabelStyle, GUILayout.Width(85));
                defenseAttribute = EditorGUILayout.IntSlider(Mathf.RoundToInt(defenseAttribute), 0, 10, GUILayout.Width(300));
                GUILayout.EndHorizontal();


                //Resistance
                GUILayout.BeginHorizontal();
                GUILayout.Space(spaceHorizontalAttributeContent);
                GUILayout.Label("Resistance:", attributeLabelStyle, GUILayout.Width(85));
                resistanceAttribute = EditorGUILayout.IntSlider(Mathf.RoundToInt(resistanceAttribute), 0, 10, GUILayout.Width(300));
                GUILayout.EndHorizontal();

                //Health
                GUILayout.BeginHorizontal();
                GUILayout.Space(spaceHorizontalAttributeContent);
                GUILayout.Label("Health:", attributeLabelStyle, GUILayout.Width(85));
                healthAttribute = EditorGUILayout.IntSlider(Mathf.RoundToInt(healthAttribute), 0, 10, GUILayout.Width(300));
                GUILayout.EndHorizontal();
                break;
        }
        

        GUILayout.Space(spaceVerticalAttributeContent);
        #endregion attributes

        #region Description
        //Card Description
        EditorGUILayout.LabelField("CARD DESCRIPTION", attributeHeaderStyle);
        GUILayout.Space(spaceVerticalAttributeContent);

        GUILayout.BeginVertical();
        GUILayout.BeginHorizontal();
        GUILayout.Space(spaceHorizontalAttributeContent);

        //Card Description
        EditorGUILayout.LabelField("Card Description:", attributeLabelStyle, GUILayout.Width(115));
        
        cardDescriptionSelected = EditorGUILayout.Popup(cardDescriptionSelected, new string[] { "Portuguese", "English" }, attributeEnumStyle, GUILayout.MaxWidth(150));
        
        GUILayout.EndHorizontal();
        GUILayout.EndVertical();

        //Card Description Text Area
        GUILayout.BeginHorizontal();
        GUILayout.Space(spaceHorizontalAttributeContent);
        DescriptionAttribute(attributeTextAreaStyle);
        GUILayout.EndHorizontal();

        GUILayout.Space(spaceVerticalAttributeContent);    
        #endregion description

        #region SpecialAttributes
        //Especial Attributes
        EditorGUILayout.LabelField("SPECIAL ATTRIBUTES", attributeHeaderStyle);
        GUILayout.Space(spaceVerticalAttributeContent);



        //Escolha o prefab base para criar a carta
        GUILayout.BeginHorizontal();
        GUILayout.Space(spaceHorizontalAttributeContent);

        GUILayout.Label(new GUIContent("Choose the base prefab for creating the card:"), attributeLabelStyle, GUILayout.Width(270));
        cardPrefab = (GameObject)EditorGUILayout.ObjectField(cardPrefab, typeof(GameObject), false, GUILayout.MaxWidth(80));
        
        GUILayout.EndHorizontal();



        //Essa carta necessita de outra para ser abaixada ?
        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(spaceHorizontalAttributeContent);

        GUILayout.Label(new GUIContent("Does this card need another card on the field to play?"), attributeLabelStyle, GUILayout.Width(315));
        anotherCardIsNeeded = GUILayout.Toggle(anotherCardIsNeeded,"");
        
        EditorGUILayout.EndHorizontal();
        SpecialAttributes(attributeLabelStyle);



        //Escolha a raridade da carta:
        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(spaceHorizontalAttributeContent);

        GUILayout.Label(new GUIContent("Choose the rarity of the card:"), attributeLabelStyle, GUILayout.Width(200));

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(spaceHorizontalAttributeContent);

        cardRaritySelected = GUILayout.SelectionGrid(cardRaritySelected, cardRarities, 3,GUILayout.MaxWidth(370),GUILayout.MaxHeight(50));

        for (int i = 0; i < cardRarities.Length; i++)
        {
            cardRarities[i] = new GUIContent(cardRaritiesString[i], cardRaritiesTexture[i], rarityStrings[i]);
        }   

        EditorGUILayout.EndHorizontal();

        GUILayout.Space(spaceVerticalAttributeContent * 3);
        #endregion specialAttributes

        #region Create Button
        GUILayout.BeginHorizontal();
        GUILayout.Space((attributeSection.width - 300) / 2);

        if(GUILayout.Button("Create Card", attributeButtonStyle, GUILayout.Width(300), GUILayout.Height(60)))
        {
            //Se atributos estiverem faltando e o nome estiver preenchido e cardPrefab estiverem preenchidos.
            if (VerifyAttributes() == true && !string.IsNullOrEmpty(cardNamePortuguese) && cardPrefab != null)
            {
                if(EditorUtility.DisplayDialog("Create the card ?", numberOfAttributes +" attributes have not yet been filled. Do you really want to continue? ", "Create Anyway", "Cancel"))
                {
                    SaveData();
                }
            }
            //Se atributos estiverem faltando e o nome NÃO estiver preenchido.
            else if (VerifyAttributes() == true && string.IsNullOrEmpty(cardNamePortuguese))
            {
                ShowNotification(new GUIContent("Please add a name to the card"),1.5f);
            }
            //Se atributos estiverem faltando o campo cardPrefab NÃO estiver preenchido.
            else if (VerifyAttributes() == true && cardPrefab == null)
            {
                ShowNotification(new GUIContent("Please add a base prefab to create the card"), 1.5f);
            }
            //Se não estiver faltando atributos.
            else if (VerifyAttributes() == false)
            {
                if(EditorUtility.DisplayDialog("Create the card ?", "Are you sure you want to create the card ?", "Yes", "No"))
                {
                    SaveData();
                }
            }
        }

        GUILayout.EndHorizontal();
        #endregion

        #region End Area, End Vertical, End ScrollView

        EditorGUILayout.Space(spaceHorizontalAttributeContent);
        
        EditorGUILayout.EndScrollView();

        GUILayout.EndVertical();
        GUILayout.EndArea();

        #endregion
    }




    /// <summary>
    /// Controla o conteúdo de edição dos atributos de cartas já existentes
    /// </summary>
    void DrawEditAttributes()
    {
        spaceHorizontalAttributeContent = ((Screen.width - Screen.width / 3.5f) - 390) / 2;
        spaceVerticalAttributeContent = 20;

        #region Attributes Styles
        //Attributes Style
        var attributeHeaderStyle = new GUIStyle(EditorStyles.label);
        attributeHeaderStyle.fontSize = 18;
        attributeHeaderStyle.fontStyle = FontStyle.Bold;
        attributeHeaderStyle.richText = true;
        attributeHeaderStyle.alignment = TextAnchor.UpperCenter;

        //Attributes Text Style
        var attributeTextStyle = new GUIStyle(EditorStyles.label);
        attributeTextStyle.fontStyle = FontStyle.Normal;
        attributeTextStyle.alignment = TextAnchor.MiddleLeft;

        //Attributes TextArea Style
        var attributeTextAreaStyle = new GUIStyle(EditorStyles.textArea);
        attributeTextAreaStyle.fixedWidth = 385;

        //Attributes TextField Style
        var attributeTextFieldStyle = new GUIStyle(EditorStyles.textField);
        attributeTextFieldStyle.alignment = TextAnchor.MiddleLeft;
        attributeTextFieldStyle.fixedWidth = 200;

        //Attributes Enum Style
        var attributeEnumStyle = new GUIStyle(EditorStyles.popup);
        attributeEnumStyle.alignment = TextAnchor.MiddleCenter;

        //Attributes ObjectField Style
        var attributeObjectFieldStyle = new GUIStyle(EditorStyles.objectField);
        attributeObjectFieldStyle.alignment = TextAnchor.MiddleLeft;

        //Attributes Label Style
        var attributeLabelStyle = new GUIStyle(EditorStyles.label);
        attributeLabelStyle.alignment = TextAnchor.MiddleLeft;
        attributeLabelStyle.richText = false;
        attributeLabelStyle.fontStyle = FontStyle.Normal;
        attributeLabelStyle.fontSize = 13;


        //Attributes Button Style
        var attributeButtonStyle = new GUIStyle(EditorStyles.miniButton);
        attributeButtonStyle.alignment = TextAnchor.MiddleCenter;

        attributeButtonStyle.fixedHeight = 60;
        attributeButtonStyle.fixedWidth = 300;

        attributeButtonStyle.normal.textColor = new Color(255f, 255f, 255f);
        attributeButtonStyle.hover.textColor = new Color(100, 100, 100);

        attributeButtonStyle.font = skin.GetStyle("Header & Create").font;
        attributeButtonStyle.richText = true;
        attributeButtonStyle.fontSize = 30;
        attributeButtonStyle.fontStyle = FontStyle.Normal;

        //Attributes ToolBar Style
        var attributeToolBarStyle = new GUIStyle();
        attributeToolBarStyle.alignment = TextAnchor.MiddleLeft;
        attributeToolBarStyle.fontStyle = FontStyle.Normal;
        attributeToolBarStyle.richText = true;
        attributeToolBarStyle.fixedHeight = 25;


        //Color of Rarities
        rarityStyles[0] = common;
        rarityStyles[1] = uncommon;
        rarityStyles[2] = rare;
        rarityStyles[3] = superRare;
        rarityStyles[4] = heroic;
        rarityStyles[5] = legendary;

        common.normal.textColor = new Color(155, 164, 178, 1);
        uncommon.normal.textColor = new Color(152, 222, 90, 1);
        rare.normal.textColor = new Color(89, 226, 232, 1);
        superRare.normal.textColor = new Color(114, 64, 170, 1);
        heroic.normal.textColor = new Color(203, 71, 85, 1);
        legendary.normal.textColor = new Color(247, 251, 47, 1);
        #endregion attributes styles

        #region Begin Area, Begin Vertical, Begin ScrollView

        GUILayout.BeginArea(attributeSection);
        GUILayout.BeginVertical();

        attributesScroll = EditorGUILayout.BeginScrollView(attributesScroll, GUILayout.Width(attributeSection.width), GUILayout.Height(attributeSection.height - 20));
        #endregion

        #region Attributes

        //Attributes Text
        EditorGUILayout.LabelField("EDIT ATTRIBUTES", attributeHeaderStyle);
        GUILayout.Space(spaceVerticalAttributeContent);


        //Card Type
        GUILayout.BeginHorizontal();
        GUILayout.Space(spaceHorizontalAttributeContent);
        EditorGUILayout.LabelField("Card Type:", attributeLabelStyle, GUILayout.MaxWidth(85));
        cardTypes = (enumCardTypes)EditorGUILayout.EnumPopup(cardTypes, attributeEnumStyle, GUILayout.MaxWidth(100));
        GUILayout.EndHorizontal();

        //Card Image
        GUILayout.BeginHorizontal();
        GUILayout.Space(spaceHorizontalAttributeContent);
        EditorGUILayout.LabelField("Card Image:", attributeLabelStyle, GUILayout.Width(85));
        textureField = (Texture2D)EditorGUILayout.ObjectField(textureField, typeof(Texture2D), false, GUILayout.MaxWidth(305));
        GUILayout.EndHorizontal();

        //Card Frame (Moldura da carta)
        GUILayout.BeginHorizontal();
        GUILayout.Space(spaceHorizontalAttributeContent);
        EditorGUILayout.LabelField("Card Frame:", attributeLabelStyle, GUILayout.Width(85));
        frameField = (Texture2D)EditorGUILayout.ObjectField(frameField, typeof(Texture2D), false, GUILayout.MaxWidth(305));
        GUILayout.EndHorizontal();

        //Card Name
        GUILayout.BeginHorizontal();
        GUILayout.Space(spaceHorizontalAttributeContent);
        GUILayout.Label("Card Name:", attributeLabelStyle, GUILayout.Width(85));
        cardNameLanguage = EditorGUILayout.Popup(cardNameLanguage, new string[] { "Portuguese", "English" }, attributeEnumStyle, GUILayout.MaxWidth(100));
        CardNameAttribute(attributeTextFieldStyle);
        GUILayout.EndHorizontal();


        switch (cardTypes)
        {
            case enumCardTypes.Soldier:
                //Attack
                GUILayout.BeginHorizontal();
                GUILayout.Space(spaceHorizontalAttributeContent);
                GUILayout.Label("Attack:", attributeLabelStyle, GUILayout.Width(85));
                attackAttribute = EditorGUILayout.IntSlider(Mathf.RoundToInt(attackAttribute), 0, 10, GUILayout.Width(300));
                GUILayout.EndHorizontal();


                //Defense
                GUILayout.BeginHorizontal();
                GUILayout.Space(spaceHorizontalAttributeContent);
                GUILayout.Label("Defense:", attributeLabelStyle, GUILayout.Width(85));
                defenseAttribute = EditorGUILayout.IntSlider(Mathf.RoundToInt(defenseAttribute), 0, 10, GUILayout.Width(300));
                GUILayout.EndHorizontal();


                //Resistance
                GUILayout.BeginHorizontal();
                GUILayout.Space(spaceHorizontalAttributeContent);
                GUILayout.Label("Resistance:", attributeLabelStyle, GUILayout.Width(85));
                resistanceAttribute = EditorGUILayout.IntSlider(Mathf.RoundToInt(resistanceAttribute), 0, 10, GUILayout.Width(300));
                GUILayout.EndHorizontal();

                //Health
                GUILayout.BeginHorizontal();
                GUILayout.Space(spaceHorizontalAttributeContent);
                GUILayout.Label("Health:", attributeLabelStyle, GUILayout.Width(85));
                healthAttribute = EditorGUILayout.IntSlider(Mathf.RoundToInt(healthAttribute), 0, 10, GUILayout.Width(300));
                GUILayout.EndHorizontal();
                break;
        }


        GUILayout.Space(spaceVerticalAttributeContent);
        #endregion attributes

        #region Description
        //Card Description
        EditorGUILayout.LabelField("EDIT DESCRIPTION", attributeHeaderStyle);
        GUILayout.Space(spaceVerticalAttributeContent);

        GUILayout.BeginVertical();
        GUILayout.BeginHorizontal();
        GUILayout.Space(spaceHorizontalAttributeContent);

        //Card Description
        EditorGUILayout.LabelField("Card Description:", attributeLabelStyle, GUILayout.Width(115));

        cardDescriptionSelected = EditorGUILayout.Popup(cardDescriptionSelected, new string[] { "Portuguese", "English" }, attributeEnumStyle, GUILayout.MaxWidth(150));

        GUILayout.EndHorizontal();
        GUILayout.EndVertical();

        //Card Description Text Area
        GUILayout.BeginHorizontal();
        GUILayout.Space(spaceHorizontalAttributeContent);
        DescriptionAttribute(attributeTextAreaStyle);
        GUILayout.EndHorizontal();

        GUILayout.Space(spaceVerticalAttributeContent);
        #endregion description

        #region SpecialAttributes
        //Especial Attributes
        EditorGUILayout.LabelField("EDIT SPECIAL ATTRIBUTES", attributeHeaderStyle);
        GUILayout.Space(spaceVerticalAttributeContent);



        //Escolha o prefab base para criar a carta
        GUILayout.BeginHorizontal();
        GUILayout.Space(spaceHorizontalAttributeContent);

        GUILayout.Label(new GUIContent("Choose the base prefab for creating the card:"), attributeLabelStyle, GUILayout.Width(270));
        cardPrefab = (GameObject)EditorGUILayout.ObjectField(cardPrefab, typeof(GameObject), false, GUILayout.MaxWidth(80));

        GUILayout.EndHorizontal();



        //Essa carta necessita de outra para ser abaixada ?
        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(spaceHorizontalAttributeContent);

        GUILayout.Label(new GUIContent("Does this card need another card on the field to play?"), attributeLabelStyle, GUILayout.Width(315));
        anotherCardIsNeeded = GUILayout.Toggle(anotherCardIsNeeded, "");

        EditorGUILayout.EndHorizontal();
        SpecialAttributes(attributeLabelStyle);



        //Escolha a raridade da carta:
        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(spaceHorizontalAttributeContent);

        GUILayout.Label(new GUIContent("Choose the rarity of the card:"), attributeLabelStyle, GUILayout.Width(200));

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(spaceHorizontalAttributeContent);

        cardRaritySelected = GUILayout.SelectionGrid(cardRaritySelected, cardRarities, 3, GUILayout.MaxWidth(370), GUILayout.MaxHeight(50));

        for (int i = 0; i < cardRarities.Length; i++)
        {
            cardRarities[i] = new GUIContent(cardRaritiesString[i], cardRaritiesTexture[i], rarityStrings[i]);
        }

        EditorGUILayout.EndHorizontal();

        GUILayout.Space(spaceVerticalAttributeContent * 3);
        #endregion specialAttributes

        #region Edit Button
        GUILayout.BeginHorizontal();
        GUILayout.Space((attributeSection.width - 300) / 2);

        if (GUILayout.Button("Edit Card", attributeButtonStyle, GUILayout.Width(300), GUILayout.Height(60)))
        {
            //Se atributos estiverem faltando e o nome estiver preenchido e cardPrefab estiverem preenchidos.
            if (VerifyAttributes() == true && !string.IsNullOrEmpty(cardNamePortuguese) && cardPrefab != null)
            {
                if (EditorUtility.DisplayDialog("Create the card ?", numberOfAttributes + " attributes have not yet been filled. Do you really want to continue? ", "Continue Anyway", "Cancel"))
                {
                    CardEditAttributes(editCardData, cardTypes, subToolBarSelected, imageCardData,oldPrefabPath);
                }
            }
            //Se atributos estiverem faltando e o nome NÃO estiver preenchido.
            else if (VerifyAttributes() == true && string.IsNullOrEmpty(cardNamePortuguese))
            {
                ShowNotification(new GUIContent("Please add a name to the card"), 1.5f);
            }
            //Se atributos estiverem faltando o campo cardPrefab NÃO estiver preenchido.
            else if (VerifyAttributes() == true && cardPrefab == null)
            {
                ShowNotification(new GUIContent("Please add a base prefab to edit the card"), 1.5f);
            }
            //Se não estiver faltando atributos.
            else if (VerifyAttributes() == false)
            {
                if (EditorUtility.DisplayDialog("Create the card ?", "Are you sure you want to edit the card ?", "Yes", "No"))
                {
                    CardEditAttributes(editCardData, cardTypes, subToolBarSelected,imageCardData, oldPrefabPath);
                }
            }
        }

        GUILayout.EndHorizontal();
        #endregion

        #region Delete Button
        GUILayout.BeginHorizontal();
        GUILayout.Space((attributeSection.width - 300) / 2);

        GUI.contentColor = Color.red;
        if (GUILayout.Button("Delete Card", attributeButtonStyle, GUILayout.Width(300), GUILayout.Height(60)))
        {
            if (EditorUtility.DisplayDialog("Delete card ?", "Are you sure you want to DELETE the card?", "Yes", "Cancel"))
            {
                DeleteCard();
            }
            
        }

        GUILayout.EndHorizontal();
        #endregion

        #region End Area, End Vertical, End ScrollView

        EditorGUILayout.Space(spaceHorizontalAttributeContent);

        EditorGUILayout.EndScrollView();

        GUILayout.EndVertical();
        GUILayout.EndArea();

        #endregion
    }




    /// <summary>
    /// Adiciona os atributos da carta selecionada ao painel de edição
    /// </summary>
    void DrawEditAttributes(CardDatas cardData, enumCardTypes type, int rarity)
    {
        textureField = cardData.textureField;
        frameField = cardData.frameField;

        cardSupport = cardData.cardSupport;
        cardPrefab = cardData.cardPrefab;

        anotherCardIsNeeded = cardData.anotherCardIsNeeded;

        attackAttribute = cardData.attackAttribute;
        resistanceAttribute = cardData.resistanceAttribute;
        defenseAttribute = cardData.defenseAttribute;
        healthAttribute = cardData.healthAttribute;

        cardNamePortuguese = cardData.cardNamePortuguese;
        cardNameEnglish = cardData.cardNameEnglish;
        descriptionPortuguese = cardData.descriptionPortuguese;
        descriptionEnglish = cardData.descriptionEnglish;

        cardTypes = type;     
        cardRaritySelected = rarity;
    }




    /// <summary>
    /// Edita os atributos da carta selecionada para edição
    /// </summary>
    void CardEditAttributes(CardDatas cardData, enumCardTypes type, int rarity, Texture2D imageData, string oldPrefab)
    {
        //Pega o caminho do scriptable object que esta sendo editado //Pega o novo caminho para onde o scriptable object será enviado
        //DATA
        string oldDataPath = AssetDatabase.GetAssetPath(cardData);
        string newDataPath = "Assets/Resources/";

        if (imageData != null)
        {
            imagePath = AssetDatabase.GetAssetPath(imageData);
        }

        //IMAGE
        string editImagePath = AssetDatabase.GetAssetPath(textureField);
        string newImagePath = "Assets/Resources/";

        //PREFAB        
        string prefabPath; //Caminho do prefab base da carta
        string newPrefabPath = "Assets/Prefabs/Cards/"; //Caminho onde salvar o novo prefab
        

        switch (type)
        {
            #region Soldier
            case enumCardTypes.Soldier: //Caso Soldado

                #region Attributes
                cardData.attackAttribute = attackAttribute;
                cardData.resistanceAttribute = resistanceAttribute;
                cardData.defenseAttribute = defenseAttribute;
                cardData.healthAttribute = healthAttribute;
                cardData.textureField = textureField;
                cardData.frameField = frameField;
                cardData.descriptionPortuguese = descriptionPortuguese;
                cardData.cardNamePortuguese = cardNamePortuguese;
                #endregion

                #region Prefab
                //DATA
                newDataPath += "Soldier Cards" + "/" + cardRaritiesString[cardRaritySelected] + "/" + cardNamePortuguese + ".asset";
                newImagePath += "Soldier Cards" + "/" + cardRaritiesString[cardRaritySelected] + "/" + cardNamePortuguese + ".png";                

                AssetDatabase.CopyAsset(editImagePath, newImagePath); //Cria uma nova imagem de acordo com o objeto na pasta correspondente
                AssetDatabase.DeleteAsset(oldDataPath); //Deleta o scriptable Object antigo
                

                //DATA INSTANCE
                SoldierData soldierEditCard = Instantiate(SoldierInfo); //Instancia um novo scriptable object
                AssetDatabase.CreateAsset(soldierEditCard, newDataPath); //Cria o novo scriptable object

                if (imageData != null && imagePath != string.Empty)
                {
                    AssetDatabase.DeleteAsset(imagePath); //Deleta a imagem do objeto editado
                }


                //PREFAB
                prefabPath = AssetDatabase.GetAssetPath(cardData.cardPrefab); //Pega o caminho do prefab base                
                newPrefabPath += "Soldier Cards/" + cardNamePortuguese + ".prefab"; //Pega o caminho do novo prefab
          
                AssetDatabase.DeleteAsset(oldPrefab); /*Deleta o Prefab antigo*/
                AssetDatabase.CopyAsset(prefabPath, newPrefabPath); /*Copia o Prefab base*/

                GameObject soldierPrefab = (GameObject)AssetDatabase.LoadAssetAtPath(newPrefabPath, typeof(GameObject));
                if (!soldierPrefab.GetComponent<Soldier>()) { soldierPrefab.AddComponent(typeof(Soldier)); }
                soldierPrefab.GetComponent<Soldier>().soldierData = soldierEditCard;

                soldierPrefab.transform.GetChild(0).GetComponent<RawImage>().texture = cardData.textureField; //Adiciona a Image da Carta
                soldierPrefab.transform.GetChild(1).GetComponent<RawImage>().texture = cardData.frameField; //Adiciona a Moldura da Carta

                soldierPrefab.transform.GetChild(2).GetComponent<TMP_Text>().text = cardData.cardNamePortuguese;
                soldierPrefab.transform.GetChild(3).GetComponent<TMP_Text>().text = cardData.descriptionPortuguese;

                //Adiciona Ataque, Defesa, Resistência e Vida ao Prefab  //* ESSAS OPÇÕES SÃO APENAS PARA A CARTA SOLDADO *//
                soldierPrefab.transform.GetChild(4).GetComponent<TMP_Text>().text = cardData.attackAttribute.ToString("00");
                soldierPrefab.transform.GetChild(5).GetComponent<TMP_Text>().text = cardData.defenseAttribute.ToString("00");
                soldierPrefab.transform.GetChild(6).GetComponent<TMP_Text>().text = cardData.resistanceAttribute.ToString("00");
                soldierPrefab.transform.GetChild(7).GetComponent<TMP_Text>().text = cardData.healthAttribute.ToString("00");
                
                //SAVE AND REFRESH
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                #endregion
                EditAttributes(soldierEditCard);

                break;
            #endregion

            #region Equipment
            case enumCardTypes.Equipment: //Caso Equipamento
                #region Attributes
                cardTypes = enumCardTypes.Equipment;
                cardData.attackAttribute = 0;
                cardData.resistanceAttribute = 0;
                cardData.defenseAttribute = 0;
                cardData.healthAttribute = 0;
                cardData.textureField = textureField;
                cardData.frameField = frameField;
                cardData.descriptionPortuguese = descriptionPortuguese;
                cardData.cardNamePortuguese = cardNamePortuguese;
                #endregion

                #region Prefab
                //DATA
                newDataPath += "Equipment Cards" + "/" + cardRaritiesString[cardRaritySelected] + "/" + cardNamePortuguese + ".asset";
                newImagePath += "Equipment Cards" + "/" + cardRaritiesString[cardRaritySelected] + "/" + cardNamePortuguese + ".png";

                AssetDatabase.CopyAsset(editImagePath, newImagePath); //Cria uma nova imagem de acordo com o objeto na pasta correspondente
                AssetDatabase.DeleteAsset(oldDataPath); //Deleta o scriptable Object antigo


                //DATA INSTANCE
                EquipmentData equipmentEditCard = Instantiate(EquipmentInfo); //Instancia um novo scriptable object
                AssetDatabase.CreateAsset(equipmentEditCard, newDataPath); //Cria o novo scriptable object

                if (imageData != null && imagePath != string.Empty)
                {
                    AssetDatabase.DeleteAsset(imagePath); //Deleta a imagem do objeto editado
                }

                //PREFAB
                prefabPath = AssetDatabase.GetAssetPath(cardData.cardPrefab); //Pega o caminho do prefab base                
                newPrefabPath += "Equipment Cards/" + cardNamePortuguese + ".prefab"; //Pega o caminho do novo prefab

                AssetDatabase.DeleteAsset(oldPrefab); /*Deleta o Prefab antigo*/
                AssetDatabase.CopyAsset(prefabPath, newPrefabPath); /*Copia o Prefab base*/

                GameObject equipmentPrefab = (GameObject)AssetDatabase.LoadAssetAtPath(newPrefabPath, typeof(GameObject));
                if (!equipmentPrefab.GetComponent<Equipment>()) { equipmentPrefab.AddComponent(typeof(Equipment)); }
                equipmentPrefab.GetComponent<Equipment>().equipmentData = equipmentEditCard;

                //equipmentPrefab.GetComponent<RawImage>().texture = cardData.textureField;

                equipmentPrefab.transform.GetChild(0).GetComponent<RawImage>().texture = cardData.textureField; //Adiciona a Image da Carta
                equipmentPrefab.transform.GetChild(1).GetComponent<RawImage>().texture = cardData.frameField; //Adiciona a Moldura da Carta

                equipmentPrefab.transform.GetChild(2).GetComponent<TMP_Text>().text = cardData.cardNamePortuguese;
                equipmentPrefab.transform.GetChild(3).GetComponent<TMP_Text>().text = cardData.descriptionPortuguese;

                //SAVE AND REFRESH
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                #endregion
                EditAttributes(equipmentEditCard);

                break;
            #endregion

            #region Temporal
            case enumCardTypes.Temporal: //Caso Temporal
                #region Attributes
                cardTypes = enumCardTypes.Equipment;
                cardData.attackAttribute = 0;
                cardData.resistanceAttribute = 0;
                cardData.defenseAttribute = 0;
                cardData.healthAttribute = 0;
                cardData.textureField = textureField;
                cardData.frameField = frameField;
                cardData.descriptionPortuguese = descriptionPortuguese;
                cardData.cardNamePortuguese = cardNamePortuguese;
                #endregion

                #region Prefab
                //DATA
                newDataPath += "Temporal Cards" + "/" + cardRaritiesString[cardRaritySelected] + "/" + cardNamePortuguese + ".asset";
                newImagePath += "Temporal Cards" + "/" + cardRaritiesString[cardRaritySelected] + "/" + cardNamePortuguese + ".png";

                AssetDatabase.CopyAsset(editImagePath, newImagePath); //Cria uma nova imagem de acordo com o objeto na pasta correspondente
                AssetDatabase.DeleteAsset(oldDataPath); //Deleta o scriptable Object antigo


                //DATA INSTANCE
                TemporalData temporalEditCard = Instantiate(TemporalInfo); //Instancia um novo scriptable object
                AssetDatabase.CreateAsset(temporalEditCard, newDataPath); //Cria o novo scriptable object

                if (imageData != null && imagePath != string.Empty)
                {
                    AssetDatabase.DeleteAsset(imagePath); //Deleta a imagem do objeto editado
                }

                //PREFAB
                prefabPath = AssetDatabase.GetAssetPath(cardData.cardPrefab); //Pega o caminho do prefab base                
                newPrefabPath += "Temporal Cards/" + cardNamePortuguese + ".prefab"; //Pega o caminho do novo prefab

                AssetDatabase.DeleteAsset(oldPrefab); /*Deleta o Prefab antigo*/
                AssetDatabase.CopyAsset(prefabPath, newPrefabPath); /*Copia o Prefab base*/

                GameObject temporalPrefab = (GameObject)AssetDatabase.LoadAssetAtPath(newPrefabPath, typeof(GameObject));
                if (!temporalPrefab.GetComponent<Temporal>()) { temporalPrefab.AddComponent(typeof(Temporal)); }
                temporalPrefab.GetComponent<Temporal>().temporalData = temporalEditCard;

                //temporalPrefab.GetComponent<RawImage>().texture = cardData.textureField;
                temporalPrefab.transform.GetChild(0).GetComponent<RawImage>().texture = cardData.textureField; //Adiciona a Image da Carta
                temporalPrefab.transform.GetChild(1).GetComponent<RawImage>().texture = cardData.frameField; //Adiciona a Moldura da Carta

                temporalPrefab.transform.GetChild(2).GetComponent<TMP_Text>().text = cardData.cardNamePortuguese;
                temporalPrefab.transform.GetChild(3).GetComponent<TMP_Text>().text = cardData.descriptionPortuguese;

                //SAVE AND REFRESH
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                #endregion
                EditAttributes(temporalEditCard);
                break;
            #endregion

            #region Special
            case enumCardTypes.Special: //Caso Special

                #region Attributes
                cardTypes = enumCardTypes.Equipment;
                cardData.attackAttribute = 0;
                cardData.resistanceAttribute = 0;
                cardData.defenseAttribute = 0;
                cardData.healthAttribute = 0;
                cardData.textureField = textureField;
                cardData.frameField = frameField;
                cardData.descriptionPortuguese = descriptionPortuguese;
                cardData.cardNamePortuguese = cardNamePortuguese;
                #endregion

                #region Prefab
                //DATA
                newDataPath += "Special Cards" + "/" + cardRaritiesString[cardRaritySelected] + "/" + cardNamePortuguese + ".asset";
                newImagePath += "Special Cards" + "/" + cardRaritiesString[cardRaritySelected] + "/" + cardNamePortuguese + ".png";

                AssetDatabase.CopyAsset(editImagePath, newImagePath); //Cria uma nova imagem de acordo com o objeto na pasta correspondente
                AssetDatabase.DeleteAsset(oldDataPath); //Deleta o scriptable Object antigo


                //DATA INSTANCE
                SpecialData specialEditCard = Instantiate(SpecialInfo); //Instancia um novo scriptable object
                AssetDatabase.CreateAsset(specialEditCard, newDataPath); //Cria o novo scriptable object

                if (imageData != null && imagePath != string.Empty)
                {
                    AssetDatabase.DeleteAsset(imagePath); //Deleta a imagem do objeto editado
                }

                //PREFAB
                prefabPath = AssetDatabase.GetAssetPath(cardData.cardPrefab); //Pega o caminho do prefab base                
                newPrefabPath += "Special Cards/" + cardNamePortuguese + ".prefab"; //Pega o caminho do novo prefab
      
                AssetDatabase.DeleteAsset(oldPrefab); /*Deleta o Prefab antigo*/
                AssetDatabase.CopyAsset(prefabPath, newPrefabPath); /*Copia o Prefab base*/

                GameObject specialPrefab = (GameObject)AssetDatabase.LoadAssetAtPath(newPrefabPath, typeof(GameObject));
                if (!specialPrefab.GetComponent<Special>()) { specialPrefab.AddComponent(typeof(Special)); }
                specialPrefab.GetComponent<Special>().specialData = specialEditCard;

                //specialPrefab.GetComponent<RawImage>().texture = cardData.textureField;
                specialPrefab.transform.GetChild(0).GetComponent<RawImage>().texture = cardData.textureField; //Adiciona a Image da Carta
                specialPrefab.transform.GetChild(1).GetComponent<RawImage>().texture = cardData.frameField; //Adiciona a Moldura da Carta
                             
                specialPrefab.transform.GetChild(2).GetComponent<TMP_Text>().text = cardData.cardNamePortuguese;
                specialPrefab.transform.GetChild(3).GetComponent<TMP_Text>().text = cardData.descriptionPortuguese;

                //SAVE AND REFRESH
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                #endregion
                EditAttributes(specialEditCard);

                break;
            #endregion

            #region Support
            case enumCardTypes.Support: //Caso Support
                #region Attributes
                cardTypes = enumCardTypes.Equipment;
                cardData.attackAttribute = 0;
                cardData.resistanceAttribute = 0;
                cardData.defenseAttribute = 0;
                cardData.healthAttribute = 0;
                cardData.textureField = textureField;
                cardData.frameField = frameField;
                cardData.descriptionPortuguese = descriptionPortuguese;
                cardData.cardNamePortuguese = cardNamePortuguese;
                #endregion

                #region Prefab
                //DATA
                newDataPath += "Support Cards" + "/" + cardRaritiesString[cardRaritySelected] + "/" + cardNamePortuguese + ".asset";
                newImagePath += "Support Cards" + "/" + cardRaritiesString[cardRaritySelected] + "/" + cardNamePortuguese + ".png";

                AssetDatabase.CopyAsset(editImagePath, newImagePath); //Cria uma nova imagem de acordo com o objeto na pasta correspondente
                AssetDatabase.DeleteAsset(oldDataPath); //Deleta o scriptable Object antigo


                //DATA INSTANCE
                SupportData supportEditCard = Instantiate(SupportInfo); //Instancia um novo scriptable object
                AssetDatabase.CreateAsset(supportEditCard, newDataPath); //Cria o novo scriptable object

                if (imageData != null && imagePath != string.Empty)
                {
                    AssetDatabase.DeleteAsset(imagePath); //Deleta a imagem do objeto editado
                }

                //PREFAB
                prefabPath = AssetDatabase.GetAssetPath(cardData.cardPrefab); //Pega o caminho do prefab base                
                newPrefabPath += "Support Cards/" + cardNamePortuguese + ".prefab"; //Pega o caminho do novo prefab
          
                AssetDatabase.DeleteAsset(oldPrefab); /*Deleta o Prefab antigo*/
                AssetDatabase.CopyAsset(prefabPath, newPrefabPath); /*Copia o Prefab base*/

                GameObject supportPrefab = (GameObject)AssetDatabase.LoadAssetAtPath(newPrefabPath, typeof(GameObject));
                if (!supportPrefab.GetComponent<Support>()) { supportPrefab.AddComponent(typeof(Support)); }
                supportPrefab.GetComponent<Support>().supportData = supportEditCard;

                //supportPrefab.GetComponent<RawImage>().texture = cardData.textureField;

                supportPrefab.transform.GetChild(0).GetComponent<RawImage>().texture = cardData.textureField; //Adiciona a Image da Carta
                supportPrefab.transform.GetChild(1).GetComponent<RawImage>().texture = cardData.frameField; //Adiciona a Moldura da Carta

                supportPrefab.transform.GetChild(2).GetComponent<TMP_Text>().text = cardData.cardNamePortuguese;
                supportPrefab.transform.GetChild(3).GetComponent<TMP_Text>().text = cardData.descriptionPortuguese;

                //SAVE AND REFRESH
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                #endregion
                EditAttributes(supportEditCard);


                break;
                #endregion
        }

        void EditAttributes(CardDatas cardDatas)
        {
            cardDatas.attackAttribute = attackAttribute;
            cardDatas.defenseAttribute = defenseAttribute;
            cardDatas.resistanceAttribute = resistanceAttribute;
            cardDatas.healthAttribute = healthAttribute;

            cardDatas.textureField = textureField;
            cardDatas.frameField = frameField;

            cardDatas.cardSupport = cardSupport;
            cardDatas.cardPrefab = cardPrefab;

            cardDatas.anotherCardIsNeeded = anotherCardIsNeeded;

            cardDatas.cardNamePortuguese = cardNamePortuguese;
            cardDatas.cardNameEnglish = cardNameEnglish;

            cardDatas.descriptionPortuguese = descriptionPortuguese;
            cardDatas.descriptionEnglish = descriptionEnglish;

            editCard = false;
           
        }
        ClearData();
    }




    /// <summary>
    /// Método responsável por deletar cartas no painel de edição
    /// </summary>
    void DeleteCard()
    {
        //Deleta o prefab e a imagem 
        AssetDatabase.DeleteAsset(oldPrefabPath);
        AssetDatabase.DeleteAsset(editImage);
        
        //Deleta o scriptable object
        string editData = AssetDatabase.GetAssetPath(editCardData);
        AssetDatabase.DeleteAsset(editData);

        AssetDatabase.Refresh();
        AssetDatabase.SaveAssets();

        editCard = false;
        ClearData();
    }




    /// <summary>
    /// Desenha as barras de status (Attack, Defense, Resistance, Health)
    /// </summary>
    void DrawBars()
    {
        float spaceBars = ((Screen.width - Screen.width / 3.5f) - 390) / 2;

        switch (cardTypes)
        {
            case enumCardTypes.Soldier:
                //ATTACK BAR (Esquerda)
                GUILayout.BeginArea(imageCardSection);
                GUILayout.BeginHorizontal();

                EditorGUI.ProgressBar(new Rect(spaceBars - 15, (imageCardSection.height / 3) + 50, 100, 20), attackAttribute / 10, "Attack " + "(" + attackAttribute + "/" + "10" + ")");

                GUILayout.EndHorizontal();
                GUILayout.EndArea();

                //DEFENSE BAR (Esquerda)
                GUILayout.BeginArea(imageCardSection);
                GUILayout.BeginHorizontal();

                EditorGUI.ProgressBar(new Rect(spaceBars - 15, (imageCardSection.height / 3) + 100, 100, 20), defenseAttribute / 10, "Defense " + "(" + defenseAttribute + "/" + "10" + ")");

                GUILayout.EndHorizontal();
                GUILayout.EndArea();


                //RESISTANCE BAR (Direita)
                GUILayout.BeginArea(imageCardSection);
                GUILayout.BeginHorizontal();

                EditorGUI.ProgressBar(new Rect(spaceBars + 390 - 85 , (imageCardSection.height / 3) + 50, 100, 20), resistanceAttribute / 10, "Resistance " + "(" + resistanceAttribute + "/" + "10" + ")");

                GUILayout.EndHorizontal();
                GUILayout.EndArea();

                //HEALTH BAR (Direita)                                                                                 
                GUILayout.BeginArea(imageCardSection);
                GUILayout.BeginHorizontal();

                EditorGUI.ProgressBar(new Rect(spaceBars + 390 - 85 , (imageCardSection.height / 3) + 100, 100, 20), healthAttribute / 10, "Health " + "(" + healthAttribute + "/" + "10" + ")");

                GUILayout.EndHorizontal();
                GUILayout.EndArea();
                break;
        }

    }




    /// <summary>
    /// Controla o card name em português e em inglês
    /// </summary>
    void CardNameAttribute(GUIStyle attributeTextFieldStyle)
    {
        switch (cardNameLanguage)
        {
            case 0:
                cardNamePortuguese = EditorGUILayout.TextField(cardNamePortuguese, attributeTextFieldStyle);
                break;
            case 1:
                cardNameEnglish = EditorGUILayout.TextField(cardNameEnglish, attributeTextFieldStyle);
                break;
        }
    }




    /// <summary>
    /// Controla a descrição em português e em inglês
    /// </summary>
    void DescriptionAttribute(GUIStyle attributeTextAreaStyle)
    {
        switch (cardDescriptionSelected)
        {
            case 0:
                scrollDescription = EditorGUILayout.BeginScrollView(scrollDescription, false, false, GUILayout.Height(75), GUILayout.MaxWidth(385));           
                descriptionPortuguese = EditorGUILayout.TextArea(descriptionPortuguese, attributeTextAreaStyle, GUILayout.ExpandHeight(true));
                EditorGUILayout.EndScrollView();
                break;
            case 1:
                scrollDescription = EditorGUILayout.BeginScrollView(scrollDescription, false, false, GUILayout.Height(75), GUILayout.MaxWidth(385));
                descriptionEnglish = EditorGUILayout.TextArea(descriptionEnglish, attributeTextAreaStyle, GUILayout.ExpandHeight(true));
                EditorGUILayout.EndScrollView();
                break;
        }
    }




    /// <summary>
    /// Exibe novos campos de preenchimento caso determinada condição seja verdadeira
    /// </summary>
    void SpecialAttributes(GUIStyle attributeLabelStyle)
    {
        //Caso Sim
        if (anotherCardIsNeeded)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(spaceHorizontalAttributeContent);

            GUILayout.Label("Which Card?", attributeLabelStyle, GUILayout.Width(80));
            cardSupport = (GameObject)EditorGUILayout.ObjectField(cardSupport, typeof(GameObject), false,GUILayout.MaxWidth(200));
            GUILayout.EndHorizontal();

            
            if(cardSupport == null)
            {
                GUILayout.BeginHorizontal(GUILayout.MaxWidth(435));
                GUILayout.Space(spaceHorizontalAttributeContent);

                EditorStyles.helpBox.alignment = TextAnchor.MiddleCenter;
                EditorStyles.helpBox.fontSize = 14;
                EditorStyles.helpBox.fontStyle = FontStyle.Bold;
               
                EditorGUILayout.HelpBox("Please choose a card.", MessageType.Warning);

                GUILayout.EndHorizontal();
            }
        }
    }




    /// <summary>
    /// Verifica se todos os atributos foram preenchidos assim que se clica no botão Create Card.
    /// Caso todos os atributos estiverem preenchidos retornará false, caso haja algum faltando retornará true.
    /// Dependendo do valor aparecerá uma mensagem diferente no inspector da Unity.
    /// </summary>
    bool VerifyAttributes()
    {
        numberOfAttributes = 0;

        #region Verificação dos Atributos
        //Verificação dos atributos DEFENSE, ATTACK, RESISTANCE E HEALTH.
        switch (cardTypes)
        {
            case enumCardTypes.Soldier:
                if (defenseAttribute == 0)
                {
                    numberOfAttributes++;
                }
                if (attackAttribute == 0)
                {
                    numberOfAttributes++;
                }
                if (resistanceAttribute == 0)
                {
                    numberOfAttributes++;
                }
                if (healthAttribute == 0)
                {
                    numberOfAttributes++;
                }
                break;
        }
       

        //Vertificação dos campos de DESCRIÇÕES da carta e do NOME.
        if(string.IsNullOrEmpty(descriptionPortuguese))
        {
            numberOfAttributes++;
        }
        if(string.IsNullOrEmpty(descriptionEnglish))
        {
            numberOfAttributes++;
        }
        if(string.IsNullOrEmpty(cardNamePortuguese))
        {
            numberOfAttributes++;
        }
        if (string.IsNullOrEmpty(cardNameEnglish))
        {
            numberOfAttributes++;
        }


        //Verificação da carta SUPORTE e IMAGEM da carta.
        switch (anotherCardIsNeeded)
        {
            case true:
                if(cardSupport == null)
                {
                    numberOfAttributes++;
                }
                break;
        }

        if(cardPrefab == null)
        {
            numberOfAttributes++;
        }

        if (textureField == null)
        {
            numberOfAttributes++;
        }
        #endregion verificação dos atributos
       
        #region Verificação Final
        //Verificação final
        if(numberOfAttributes > 0)
        {
            //Caso esteja faltando atributos para serem preenchidos retornar true
            return true;
        }
        else
        {
            //Caso todos os atributos estiverem preenchidos retornar false
            return false;
        }

        #endregion verificação final
    }




    /// <summary>
    /// Atribui todas as variáveis do painel para o scriptable object da carta
    /// </summary>
    void AssignData()
    {
        switch (cardTypes)
        {
            #region Soldier Type
            case enumCardTypes.Soldier:
                //Card Name
                CardGeneratorWindow.SoldierInfo.cardNamePortuguese = cardNamePortuguese;
                CardGeneratorWindow.SoldierInfo.cardNameEnglish = cardNameEnglish;

                //Attack, resistance, defense, health
                CardGeneratorWindow.SoldierInfo.attackAttribute = attackAttribute;
                CardGeneratorWindow.SoldierInfo.resistanceAttribute = resistanceAttribute;
                CardGeneratorWindow.SoldierInfo.defenseAttribute = defenseAttribute;
                CardGeneratorWindow.SoldierInfo.healthAttribute = healthAttribute;

                //Description portuguese and english
                CardGeneratorWindow.SoldierInfo.descriptionPortuguese = descriptionPortuguese;
                CardGeneratorWindow.SoldierInfo.descriptionEnglish = descriptionEnglish;

                //Another Card is Needed, cardSupport, textureField, frameField && cardPrefab
                CardGeneratorWindow.SoldierInfo.anotherCardIsNeeded = anotherCardIsNeeded;
                CardGeneratorWindow.SoldierInfo.cardSupport = cardSupport;

                CardGeneratorWindow.SoldierInfo.textureField = textureField;
                CardGeneratorWindow.SoldierInfo.frameField = frameField;

                CardGeneratorWindow.SoldierInfo.cardPrefab = cardPrefab;
                CardGeneratorWindow.SoldierInfo.cardType = Types.CardTypes.SOLDIER;

                break;
            #endregion soldier type
            
            #region Equipment Type
            case enumCardTypes.Equipment:
                //Card Name
                CardGeneratorWindow.EquipmentInfo.cardNamePortuguese = cardNamePortuguese;
                CardGeneratorWindow.EquipmentInfo.cardNameEnglish = cardNameEnglish;

                //Description portuguese and english
                CardGeneratorWindow.EquipmentInfo.descriptionPortuguese = descriptionPortuguese;
                CardGeneratorWindow.EquipmentInfo.descriptionEnglish = descriptionEnglish;

                //Another Card is Needed, cardSupport, textureField && cardPrefab
                CardGeneratorWindow.EquipmentInfo.anotherCardIsNeeded = anotherCardIsNeeded;
                CardGeneratorWindow.EquipmentInfo.cardSupport = cardSupport;

                CardGeneratorWindow.EquipmentInfo.textureField = textureField;
                CardGeneratorWindow.EquipmentInfo.frameField = frameField;

                CardGeneratorWindow.EquipmentInfo.cardPrefab = cardPrefab;
                CardGeneratorWindow.EquipmentInfo.cardType = Types.CardTypes.EQUIPMENT;
                break;
            #endregion equipment type

            #region Temporal Type
            case enumCardTypes.Temporal:
                //Card Name
                CardGeneratorWindow.TemporalInfo.cardNamePortuguese = cardNamePortuguese;
                CardGeneratorWindow.TemporalInfo.cardNameEnglish = cardNameEnglish;

                //Description portuguese and english
                CardGeneratorWindow.TemporalInfo.descriptionPortuguese = descriptionPortuguese;
                CardGeneratorWindow.TemporalInfo.descriptionEnglish = descriptionEnglish;

                //Another Card is Needed, cardSupport, textureField && cardPrefab
                CardGeneratorWindow.TemporalInfo.anotherCardIsNeeded = anotherCardIsNeeded;
                CardGeneratorWindow.TemporalInfo.cardSupport = cardSupport;

                CardGeneratorWindow.TemporalInfo.textureField = textureField;
                CardGeneratorWindow.TemporalInfo.frameField = frameField;

                CardGeneratorWindow.TemporalInfo.cardPrefab = cardPrefab;
                CardGeneratorWindow.TemporalInfo.cardType = Types.CardTypes.TEMPORAL;
                break;
            #endregion temporal type

            #region Special Type
            case enumCardTypes.Special:
                //Card Name
                CardGeneratorWindow.SpecialInfo.cardNamePortuguese = cardNamePortuguese;
                CardGeneratorWindow.SpecialInfo.cardNameEnglish = cardNameEnglish;

                //Description portuguese and english
                CardGeneratorWindow.SpecialInfo.descriptionPortuguese = descriptionPortuguese;
                CardGeneratorWindow.SpecialInfo.descriptionEnglish = descriptionEnglish;

                //Another Card is Needed, cardSupport, textureField && cardPrefab
                CardGeneratorWindow.SpecialInfo.anotherCardIsNeeded = anotherCardIsNeeded;
                CardGeneratorWindow.SpecialInfo.cardSupport = cardSupport;

                CardGeneratorWindow.SpecialInfo.textureField = textureField;
                CardGeneratorWindow.SpecialInfo.frameField = frameField;

                CardGeneratorWindow.SpecialInfo.cardPrefab = cardPrefab;
                CardGeneratorWindow.SpecialInfo.cardType = Types.CardTypes.SPECIAL;
                break;
            #endregion special type

            #region Support Type
            case enumCardTypes.Support:
                //Card Name
                CardGeneratorWindow.SupportInfo.cardNamePortuguese = cardNamePortuguese;
                CardGeneratorWindow.SupportInfo.cardNameEnglish = cardNameEnglish;

                //Description portuguese and english
                CardGeneratorWindow.SupportInfo.descriptionPortuguese = descriptionPortuguese;
                CardGeneratorWindow.SupportInfo.descriptionEnglish = descriptionEnglish;

                //Another Card is Needed, cardSupport, textureField && cardPrefab
                CardGeneratorWindow.SupportInfo.anotherCardIsNeeded = anotherCardIsNeeded;
                CardGeneratorWindow.SupportInfo.cardSupport = cardSupport;

                CardGeneratorWindow.SupportInfo.textureField = textureField;
                CardGeneratorWindow.SupportInfo.frameField = frameField;

                CardGeneratorWindow.SupportInfo.cardPrefab = cardPrefab;
                CardGeneratorWindow.SupportInfo.cardType = Types.CardTypes.SUPPORT;
                break;
                #endregion support type
        }
    }




    /// <summary>
    /// Cria o scriptable object e o prefab, ambos já configurados.
    /// </summary>
    void SaveData()
    {
        string prefabPath; //Caminho do prefab base da carta
        string newPrefabPath = "Assets/Prefabs/Cards/"; //Caminho onde salvar o novo prefab
        string dataPath = "Assets/Resources/";

        string previewPath;
        string newPreviewPath = "Assets/Resources/";

        AssignData();

        switch (cardTypes)
        {
            #region Soldier Type
            case enumCardTypes.Soldier:
                        
                dataPath += "Soldier Cards/" + cardRaritiesString[cardRaritySelected] + "/" + CardGeneratorWindow.SoldierInfo.cardNamePortuguese + ".asset";

                SoldierData soldierCard = Instantiate(SoldierInfo);
                AssetDatabase.CreateAsset(soldierCard, dataPath);

                newPrefabPath += "Soldier Cards/" + CardGeneratorWindow.SoldierInfo.cardNamePortuguese + ".prefab";
                newPreviewPath += "Soldier Cards/" + cardRaritiesString[cardRaritySelected] + "/" + CardGeneratorWindow.SoldierInfo.cardNamePortuguese + ".png";

                prefabPath = AssetDatabase.GetAssetPath(CardGeneratorWindow.SoldierInfo.cardPrefab);
                previewPath = AssetDatabase.GetAssetPath(CardGeneratorWindow.SoldierInfo.textureField);

                AssetDatabase.CopyAsset(prefabPath, newPrefabPath); /*Copia o Prefab*/
                AssetDatabase.CopyAsset(previewPath, newPreviewPath);/*Copia a Imagem*/
                
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                GameObject soldierPrefab = (GameObject)AssetDatabase.LoadAssetAtPath(newPrefabPath, typeof(GameObject));
                if (!soldierPrefab.GetComponent<Soldier>()) { soldierPrefab.AddComponent(typeof(Soldier)); }
                soldierPrefab.GetComponent<Soldier>().soldierData = soldierCard;            
                

                //Adiciona a Imagem e a Moldura ao Prefab
                soldierPrefab.transform.GetChild(0).GetComponent<RawImage>().texture = CardGeneratorWindow.SoldierInfo.textureField; //Adiciona a Imagem da Carta
                soldierPrefab.transform.GetChild(1).GetComponent<RawImage>().texture = CardGeneratorWindow.SoldierInfo.frameField; //Adiciona a Moldura da Carta

                //Adiciona a descrição e o Nome ao Prefab
                soldierPrefab.transform.GetChild(2).GetComponent<TMP_Text>().text = CardGeneratorWindow.SoldierInfo.cardNamePortuguese;
                soldierPrefab.transform.GetChild(3).GetComponent<TMP_Text>().text = CardGeneratorWindow.SoldierInfo.descriptionPortuguese;

                //Adiciona Ataque, Defesa, Resistência e Vida ao Prefab  //* ESSAS OPÇÕES SÃO APENAS PARA A CARTA SOLDADO *//
                soldierPrefab.transform.GetChild(4).GetComponent<TMP_Text>().text = CardGeneratorWindow.SoldierInfo.attackAttribute.ToString("00");
                soldierPrefab.transform.GetChild(5).GetComponent<TMP_Text>().text = CardGeneratorWindow.SoldierInfo.defenseAttribute.ToString("00");
                soldierPrefab.transform.GetChild(6).GetComponent<TMP_Text>().text = CardGeneratorWindow.SoldierInfo.resistanceAttribute.ToString("00");
                soldierPrefab.transform.GetChild(7).GetComponent<TMP_Text>().text = CardGeneratorWindow.SoldierInfo.healthAttribute.ToString("00");
                break;
            #endregion soldier type

            #region Equipment Type
            case enumCardTypes.Equipment:
                        
                dataPath += "Equipment Cards/" + cardRaritiesString[cardRaritySelected] + "/" + CardGeneratorWindow.EquipmentInfo.cardNamePortuguese + ".asset";

                EquipmentData equipmentCard = Instantiate(EquipmentInfo);
                AssetDatabase.CreateAsset(equipmentCard, dataPath);

                newPrefabPath += "Equipment Cards/" + CardGeneratorWindow.EquipmentInfo.cardNamePortuguese + ".prefab";
                newPreviewPath += "Equipment Cards/" + cardRaritiesString[cardRaritySelected] + "/" + CardGeneratorWindow.EquipmentInfo.cardNamePortuguese + ".png";

                prefabPath = AssetDatabase.GetAssetPath(CardGeneratorWindow.EquipmentInfo.cardPrefab);
                previewPath = AssetDatabase.GetAssetPath(CardGeneratorWindow.EquipmentInfo.textureField);

                AssetDatabase.CopyAsset(prefabPath, newPrefabPath); /*Copia o Prefab*/
                AssetDatabase.CopyAsset(previewPath, newPreviewPath);/*Copia a Imagem*/

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                GameObject equipmentPrefab = (GameObject)AssetDatabase.LoadAssetAtPath(newPrefabPath, typeof(GameObject));
                if (!equipmentPrefab.GetComponent<Equipment>()) { equipmentPrefab.AddComponent(typeof(Equipment)); }
                equipmentPrefab.GetComponent<Equipment>().equipmentData = equipmentCard;


                equipmentPrefab.transform.GetChild(0).GetComponent<RawImage>().texture = CardGeneratorWindow.EquipmentInfo.textureField; //Adiciona a Imagem da Carta
                equipmentPrefab.transform.GetChild(1).GetComponent<RawImage>().texture = CardGeneratorWindow.EquipmentInfo.frameField; //Adiciona a Moldura da Carta

                equipmentPrefab.transform.GetChild(2).GetComponent<TMP_Text>().text = CardGeneratorWindow.EquipmentInfo.cardNamePortuguese;
                equipmentPrefab.transform.GetChild(3).GetComponent<TMP_Text>().text = CardGeneratorWindow.EquipmentInfo.descriptionPortuguese;

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                break;
            #endregion equipment type

            #region Temporal Type
            case enumCardTypes.Temporal:
                       
                dataPath += "Temporal Cards/" + cardRaritiesString[cardRaritySelected] + "/" + CardGeneratorWindow.TemporalInfo.cardNamePortuguese + ".asset";

                TemporalData temporalCard = Instantiate(TemporalInfo);
                AssetDatabase.CreateAsset(temporalCard, dataPath);

                newPrefabPath += "Temporal Cards/" + CardGeneratorWindow.TemporalInfo.cardNamePortuguese + ".prefab";
                newPreviewPath += "Temporal Cards/" + cardRaritiesString[cardRaritySelected] + "/" + CardGeneratorWindow.TemporalInfo.cardNamePortuguese + ".png";

                prefabPath = AssetDatabase.GetAssetPath(CardGeneratorWindow.TemporalInfo.cardPrefab);
                previewPath = AssetDatabase.GetAssetPath(CardGeneratorWindow.TemporalInfo.textureField);

                AssetDatabase.CopyAsset(prefabPath, newPrefabPath); /*Copia o Prefab*/
                AssetDatabase.CopyAsset(previewPath, newPreviewPath);/*Copia a Imagem*/


                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                GameObject temporalPrefab = (GameObject)AssetDatabase.LoadAssetAtPath(newPrefabPath, typeof(GameObject));
                if (!temporalPrefab.GetComponent<Temporal>()) { temporalPrefab.AddComponent(typeof(Temporal)); }
                temporalPrefab.GetComponent<Temporal>().temporalData = temporalCard;


                temporalPrefab.transform.GetChild(0).GetComponent<RawImage>().texture = CardGeneratorWindow.TemporalInfo.textureField; //Adiciona a Imagem da Carta
                temporalPrefab.transform.GetChild(1).GetComponent<RawImage>().texture = CardGeneratorWindow.TemporalInfo.frameField; //Adiciona a Moldura da Carta

                temporalPrefab.transform.GetChild(2).GetComponent<TMP_Text>().text = CardGeneratorWindow.TemporalInfo.cardNamePortuguese;
                temporalPrefab.transform.GetChild(3).GetComponent<TMP_Text>().text = CardGeneratorWindow.TemporalInfo.descriptionPortuguese;

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                break;
            #endregion tempora type

            #region Special Type
            case enumCardTypes.Special:
                            
                dataPath += "Special Cards/" + cardRaritiesString[cardRaritySelected] + "/" + CardGeneratorWindow.SpecialInfo.cardNamePortuguese + ".asset";

                SpecialData specialCard = Instantiate(SpecialInfo);
                AssetDatabase.CreateAsset(specialCard, dataPath);

                newPrefabPath += "Special Cards/" + CardGeneratorWindow.SpecialInfo.cardNamePortuguese + ".prefab";
                newPreviewPath += "Special Cards/" + cardRaritiesString[cardRaritySelected] + "/" + CardGeneratorWindow.SpecialInfo.cardNamePortuguese + ".png";

                prefabPath = AssetDatabase.GetAssetPath(CardGeneratorWindow.SpecialInfo.cardPrefab);
                previewPath = AssetDatabase.GetAssetPath(CardGeneratorWindow.SpecialInfo.textureField);

                AssetDatabase.CopyAsset(prefabPath, newPrefabPath); /*Copia o Prefab*/
                AssetDatabase.CopyAsset(previewPath, newPreviewPath);/*Copia a Imagem*/


                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                GameObject specialPrefab = (GameObject)AssetDatabase.LoadAssetAtPath(newPrefabPath, typeof(GameObject));
                if (!specialPrefab.GetComponent<Special>()) { specialPrefab.AddComponent(typeof(Special)); }
                specialPrefab.GetComponent<Special>().specialData = specialCard;


                specialPrefab.transform.GetChild(0).GetComponent<RawImage>().texture = CardGeneratorWindow.SpecialInfo.textureField; //Adiciona a Imagem da Carta
                specialPrefab.transform.GetChild(1).GetComponent<RawImage>().texture = CardGeneratorWindow.SpecialInfo.frameField; //Adiciona a Moldura da Carta

                specialPrefab.transform.GetChild(2).GetComponent<TMP_Text>().text = CardGeneratorWindow.SpecialInfo.cardNamePortuguese;
                specialPrefab.transform.GetChild(3).GetComponent<TMP_Text>().text = CardGeneratorWindow.SpecialInfo.descriptionPortuguese;

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                break;
            #endregion special type

            #region Support Type
            case enumCardTypes.Support:
                        
                dataPath += "Support Cards/" + cardRaritiesString[cardRaritySelected] + "/" + CardGeneratorWindow.SupportInfo.cardNamePortuguese + ".asset";

                SupportData supportCard = Instantiate(SupportInfo);
                AssetDatabase.CreateAsset(supportCard, dataPath);

                newPrefabPath += "Support Cards/" + CardGeneratorWindow.SupportInfo.cardNamePortuguese + ".prefab";
                newPreviewPath += "Support Cards/" + cardRaritiesString[cardRaritySelected] + "/" + CardGeneratorWindow.SupportInfo.cardNamePortuguese + ".png";

                prefabPath = AssetDatabase.GetAssetPath(CardGeneratorWindow.SupportInfo.cardPrefab);
                previewPath = AssetDatabase.GetAssetPath(CardGeneratorWindow.SupportInfo.textureField);

                AssetDatabase.CopyAsset(prefabPath, newPrefabPath); /*Copia o Prefab*/
                AssetDatabase.CopyAsset(previewPath, newPreviewPath);/*Copia a Imagem*/


                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                GameObject supportPrefab = (GameObject)AssetDatabase.LoadAssetAtPath(newPrefabPath, typeof(GameObject));
                if (!supportPrefab.GetComponent<Support>()) { supportPrefab.AddComponent(typeof(Support)); }
                supportPrefab.GetComponent<Support>().supportData = supportCard;


                supportPrefab.transform.GetChild(0).GetComponent<RawImage>().texture = CardGeneratorWindow.SupportInfo.textureField; //Adiciona a Imagem da Carta
                supportPrefab.transform.GetChild(1).GetComponent<RawImage>().texture = CardGeneratorWindow.SupportInfo.frameField; //Adiciona a Moldura da Carta

                supportPrefab.transform.GetChild(2).GetComponent<TMP_Text>().text = CardGeneratorWindow.SupportInfo.cardNamePortuguese;
                supportPrefab.transform.GetChild(3).GetComponent<TMP_Text>().text = CardGeneratorWindow.SupportInfo.descriptionPortuguese;

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                break;
                #endregion support type
        }

        ClearData();
    }




    /// <summary>
    /// Limpa todos os dados do painel após a criação da carta
    /// </summary>
    void ClearData()
    {
        //Image && Card Type
        //cardTypes = 0; <<< Não é necessário
        textureField = null;
        frameField = null;

        //Names
        cardNameLanguage = 0;
        cardNamePortuguese = "";
        cardNameEnglish = "";

        //Attributes
        attackAttribute = 0;
        defenseAttribute = 0;
        resistanceAttribute = 0;
        healthAttribute = 0;

        //Descriptions
        cardDescriptionSelected = 0;
        descriptionPortuguese = "";
        descriptionEnglish = "";

        //Special Attributes
        cardPrefab = null;
        anotherCardIsNeeded = false;
        cardSupport = null;
        cardRaritySelected = 0;
    }
}
