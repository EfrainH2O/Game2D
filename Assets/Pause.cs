using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Pause : MonoBehaviour
{
    public Animator anim;
    private float progress;
    private bool paused;
    private EventSystem eventSystem;
    private void Awake() {
        eventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();
    }
   private void Update() {
    if(Input.GetButtonDown("Cancel")){
        PausePress();
    }
   }

   public void PausePress(){
        progress = anim.GetCurrentAnimatorStateInfo(0).normalizedTime;
        if(progress >= 1 || progress == 0){
            if(!paused){
                eventSystem.enabled = true;
                anim.Play("Enter");
                Player.Instance.GetComponent<CharacterControlls>().enabled = false;
                Time.timeScale = 0;
                SFXManager.instance.PauseSFX();
                paused = true;
            }else{
                eventSystem.enabled = false;
                anim.Play("Exit");
                Player.Instance.GetComponent<CharacterControlls>().enabled = true;
                Time.timeScale = 1;
                SFXManager.instance.UnPauseSFX();
                paused = false;
            }
        } 
   }


}
