using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;
using UnityEngine;

public class MixerManager : MonoBehaviour
{
    // Start is called before the first frame update
    public static MixerManager instance;
    [SerializeField] private AudioMixer audioMixer;
    
    public void Awake(){
        if(instance == null){
            instance = this;
        }else{
            Destroy(gameObject);
        }
    }
    public void SetSFXVolume(float nVol){
        audioMixer.SetFloat("SFXVol", Mathf.Log10(nVol)*20);
    }

    public void SetMasterVolume(float nVol){
        audioMixer.SetFloat("MasterVol", Mathf.Log10(nVol)*20f);
    }

    public void SetMusicVolume(float nVol){
        audioMixer.SetFloat("MusicVol", Mathf.Log10(nVol)*20f);
    }


}
