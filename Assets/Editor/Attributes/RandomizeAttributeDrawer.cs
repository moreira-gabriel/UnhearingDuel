using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(RandomizeAttribute))]
public class RandomizeAttributeDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return 32f;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if(property.propertyType == SerializedPropertyType.Float)
        {
            EditorGUI.BeginProperty(position, label, property);
            Rect labelPosition = new Rect(position.x, position.y, position.width, 16f);
            Rect buttonPosition = new Rect(position.x, position.y + labelPosition.height, position.width, 16f);

            EditorGUI.LabelField(labelPosition, label, new GUIContent(property.floatValue.ToString()));
            if (GUI.Button(buttonPosition, "Randomize!"))
            {
                RandomizeAttribute randomizeAttribute = (RandomizeAttribute)attribute;
                property.floatValue = Random.Range(randomizeAttribute.minValue, randomizeAttribute.maxValue);
            }
            EditorGUI.EndProperty();
        }
        else
        {
            EditorGUI.LabelField(position, "Use Randomize Attribute with floats");
        }

     
    }
}
