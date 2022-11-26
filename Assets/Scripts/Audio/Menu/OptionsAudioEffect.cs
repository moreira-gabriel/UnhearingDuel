using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionsAudioEffect : MonoBehaviour
{
    [SerializeField] private AudioClip optionEffect;

    public void OptionEffect()
    {
        SoundManager.instance.PlaySound(optionEffect);
    }
}
