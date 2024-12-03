using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.InputSystem;
using UnityEngine;
using UnityEditor.Rendering;

public class CharacterControlls : MonoBehaviour
{
    // Components
    private CharacterMovement chMov;
    private Player player;
    private PlayerAnimatorController plyAnimator;
    private Hook hook;
    private Rigidbody2D rb2d;
    private SpriteRenderer sp;
    // Variables
    private float xMov, yMov;
    private bool yStart, yEnd, powerInput, powerOutput, planePow, lockTarget;
    [SerializeField]
    private int state, prevState;
    private bool rFace;
    
    private void Awake() {
        chMov = GetComponent<CharacterMovement>();
        player = GetComponent<Player>();
        plyAnimator = GetComponent<PlayerAnimatorController>();
        hook = GetComponent<Hook>();
        sp = GetComponent<SpriteRenderer>();
        rb2d = GetComponent<Rigidbody2D>();

    }
    void Start()
    {
        rFace = true;
        state = 0;
        prevState = state;
    }

    // Update is called once per frame
    void Update()
    {
        
        
        if (player.GetHited())
        {
            chMov.HorizontalMov(0);
            chMov.VerticalMov(false, false);
            chMov.LockTarget(false);
    
        }
        else
        {
            //Inputs
                        xMov = Input.GetAxisRaw("Horizontal");
                        yMov = Input.GetAxisRaw("Vertical");
                        yStart = Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.JoystickButton0);
                        yEnd = Input.GetKeyUp(KeyCode.Space) || Input.GetKeyUp(KeyCode.JoystickButton0);
                        powerInput = Input.GetButtonDown("Power") ;
                        powerOutput = Input.GetButtonUp("Power");
                        planePow = Input.GetKeyDown(KeyCode.E);
                        lockTarget = Input.GetKey(KeyCode.LeftControl);

            //Actions depending on the state
            
                if(state == 0){
                    chMov.HorizontalMov(xMov);
                    chMov.VerticalMov(yStart, yEnd);
                    animateXDir(xMov);
                    //Normal movement
                    
                }
                else if (state == 1){
                    chMov.RotatedMov(xMov, yMov);
                    chMov.Impulse(yStart);
                    //Movement in plane
                }
                else if(state == 2){
                    hook.MovementInHook(xMov);
                    animateXBalance(xMov);
                    //Movement in rope
                }


                if(powerInput){         //
                    ChangeState(2);     //

                }else if(state == 2 &&(powerOutput|| chMov.getTarget() == null)){  //Hook
                    ChangeState(prevState);
                }
                if(planePow){
                    ChangeStateButton(); // plane
                }

        }
        

    }
    private void FixedUpdate() {
        if(state == 1){
            chMov.Momentum();
        }
        chMov.LockTarget(lockTarget);
    }

    private void animateXDir(float mov){
        if (mov == 0){
            plyAnimator.walking = false;
        }
        else{
            plyAnimator.walking = true;
            if(mov > 0 && !rFace){
                transform.eulerAngles = new Vector3(0f,0,transform.eulerAngles.z);
                rFace = true;
            }
            else if(mov < 0 && rFace){
                transform.eulerAngles = new Vector3(0f,-180f,transform.eulerAngles.z);
                rFace = false;
            }
        }

    }
    private void animateXBalance(float mov){
        if (mov == 0){
            plyAnimator.walking = false;
        }
        else{
            plyAnimator.walking = true;
            if(mov > 0 && !rFace){
                sp.flipX = false;
                rFace = true;
            }
            else if(mov < 0 && rFace){
                sp.flipX = true;
                rFace = false;
            }
        }
    }

    public void ChangeState(int desState){
        if(desState == 1){
            SetFlyState();
            
        }else if(desState == 0){
            SetGroundState();
        }
        else if(desState == 2){
            SetBalanceState();
        }
    }

    private void ChangeStateButton(){
        if(state == 1){
            SetGroundState();
        }
        else{
            SetFlyState();
        }
    }


    private void SetGroundState(){
            plyAnimator.flying = false;
            if(sp.flipX == true){
                transform.eulerAngles = new Vector3(0f,-180f,0);
                sp.flipX = false;
            }
            transform.eulerAngles = new Vector3(0,transform.rotation.eulerAngles.y,0);
            if(player.GetHited()){
                transform.eulerAngles = new Vector3(0,0,0);
                rFace = true;
            }
            hook.DestroyHook();
            rb2d.freezeRotation = true;
            state = 0;
            prevState = state == 2? prevState : state;
    }

    private void SetBalanceState(){
        prevState = state == 2? prevState : state;
        Collider2D temp = chMov.getTarget();
        if(temp != null){
            transform.eulerAngles = new Vector3(0,0,0);
            if(!rFace){
                sp.flipX = true;
            }
            hook.CreateHook(temp);
        }
        state = 2;
       
    }

    private void SetFlyState(){
        if(!chMov.getGround()){
            hook.DestroyHook();
            plyAnimator.flying = true;
            if(sp.flipX == true){
                transform.eulerAngles = new Vector3(0f,180f, 360f-transform.eulerAngles.z);
                sp.flipX = false;
            }
            rb2d.freezeRotation = false;
            state = 1;
        }else{
            SetGroundState();
        }
    }

    private void OnCollisionEnter2D(Collision2D other) {
        var layerMask = other.gameObject.layer;
        if ( layerMask == 3 || layerMask == 8 ){
            if(state == 1 || (state == 2 && other.relativeVelocity.magnitude > 20)){
                ChangeState(0);
                Debug.Log(state);
                Debug.Log(other.relativeVelocity.magnitude);
            }
        }
        
    }

}
