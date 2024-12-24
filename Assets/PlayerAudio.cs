using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudio : MonoBehaviour
{
    [SerializeField]
    private List<AudioClip> FootSteps;

    // Start is called before the first frame update
    private void PlayFootSteps(){
        SFXManager.instance.PlaySFX(FootSteps[UnityEngine.Random.Range(0,FootSteps.Count)], transform.position, UnityEngine.Random.Range(6f,8f));
    }
}
