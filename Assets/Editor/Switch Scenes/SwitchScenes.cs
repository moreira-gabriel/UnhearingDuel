using UnityEngine;

public class SwitchScenes : MonoBehaviour
{
    int toolbarInt = 0;
    string[] toolbarStrings = { "Toolbar1", "Toolbar2", "Toolbar3" };

    void OnGUI()
    {
        toolbarInt = GUILayout.Toolbar(toolbarInt, toolbarStrings);
    }
}


