using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Click : MonoBehaviour
{
    void OnMouseUp()
    {
        IClicker clicker = transform.GetComponent<IClicker>();
       
        if(clicker == null)
        {
            return;
        }
        else
        {
            clicker.Click();
        }
    }
}
