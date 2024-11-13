using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimatorController : MonoBehaviour
{
    //States
    public bool walking, dobleJump, Hit, grounded, flying;
    public float yVel;
    //Animation
    private string WALKING, IDLE, JUMPING, FALLING, HIT, DOBLEJUMP, FLYING;
    private string currentAnimation;
    //Components
    private Animator animator;
    // Start is called before the first frame update
    

    // Update is called once per frame
    private void Awake() {
        animator = GetComponent<Animator>();
        WALKING = "Walking";
        IDLE = "Idle";
        HIT = "Hit";
        JUMPING = "Upwards";
        FALLING = "DownWards";
        DOBLEJUMP = "DobleJ";
        FLYING = "Flying";
    }
    void Update()
    {
        if(Hit == true){
            ChangeAnimation(HIT);
        }
        else if(!grounded){
            if(flying){
                ChangeAnimation(FLYING);
            }
            else if(dobleJump){
                ChangeAnimation(DOBLEJUMP);
            }
            else if(yVel > 0.2f ){
                ChangeAnimation(JUMPING);
            }
            else if(yVel<0f){
                ChangeAnimation(FALLING);
            }
        }
        else if(walking){
            ChangeAnimation(WALKING);
        }
        else{
            ChangeAnimation(IDLE);
        }


        
    }
    void ChangeAnimation(string desAnimation){
        if(desAnimation != currentAnimation){
            currentAnimation = desAnimation;
            animator.Play(desAnimation);
        }
    }
}
