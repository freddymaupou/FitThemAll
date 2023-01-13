using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    #region Singleton
    public static AudioManager Instance;

    private void Awake()
    {
        if(Instance == null) Instance = this;
    }
    #endregion

    private AudioSource audioSource;
    public AudioSource AudioSource => audioSource;

    [Serializable] public struct AudioClipStruct
    {
        public string name;
        public AudioClip audioClip;
    }

    [SerializeField] private List<AudioClipStruct> audioClips;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlaySound(string soundName)
    {
        for(int i = 0; i < audioClips.Count; i++)
        {
            if (audioClips[i].name == soundName)
            {
                audioSource.PlayOneShot(audioClips[i].audioClip);
                return;
            }
        }
    }
}
