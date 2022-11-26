using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Example : MonoBehaviour
{
    [Randomize(0f, 100f)]
    public float Value;

    [ForceInterface(typeof(IWeapon))]
    public Object weapon;

    [EnumData(typeof(TextStyle))]
    public int[] textSizes = { 12, 6, 8, 14, 18 };

    public void Write()
    {
        Debug.Log("Sizes: " + textSizes.Length);
    }

    public enum TextStyle
    {
        Medium,
        Tiny,
        Small,
        Big,
        Huge,
    }

}
