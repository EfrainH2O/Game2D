using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class SFXManager : MonoBehaviour
{

    public static SFXManager instance;
    // Start is called before the first frame update
    [SerializeField] 
    private AudioSource prefab;
    private Dictionary<AudioClip, AudioSource> active = new Dictionary<AudioClip, AudioSource>();
    void Awake()
    {
        if(instance ==  null){
            instance = this;
        }else{
            Destroy(gameObject);
        }
    }

    public void PlaySFX(AudioClip clip, Vector2 pos, float volum){
        AudioSource temp = Instantiate(prefab, pos, Quaternion.identity);
        temp.clip = clip;
        temp.volume = volum;
        temp.Play();
        Destroy(temp.gameObject, clip.length);
    }

    public AudioSource StartPlayingSFX(AudioClip clip, Vector2 pos, float volum){
        AudioSource temp = Instantiate(prefab, pos, Quaternion.identity);
        temp.clip = clip;
        temp.volume = volum;
        temp.loop = true;
        try{
            active.Add(clip,temp);
        }catch(ArgumentException){
            Debug.LogWarning("The Sound is alredy Playing");
            Destroy(temp.gameObject);
            return temp;
        }
        temp.Play();
        return temp;
    }

    public void EndPlaySFX(AudioClip clip){
        AudioSource temp;
        try{
            temp = active[clip];
        }catch (KeyNotFoundException){
            Debug.LogWarning("The Sound is not being played");
            return;
        }
        Destroy(temp.gameObject);
        active.Remove(clip);
    }
    
    
}
