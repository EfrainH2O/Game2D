using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.InputSystem;
using UnityEngine;

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
    private bool yStart, yEnd, powerInput, powerOutput, powerChange, lockTarget;
    [SerializeField]
    private int state, powerSelected;
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
        powerSelected = 0;
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
                        powerChange = Input.GetKeyUp(KeyCode.Tab);
                        lockTarget = Input.GetKey(KeyCode.LeftControl);

            //Actions depending on the state
            chMov.LockTarget(lockTarget);
                if(state == 0){
                    chMov.HorizontalMov(xMov);
                    chMov.VerticalMov(yStart, yEnd);
                    animateXDir(xMov);
                    
                }
                else if (state == 1){
                    chMov.RotatedMov(xMov, yMov);
                    chMov.Impulse(yStart);
                }
                else if(state == 2){
                    hook.MovementInHook(xMov);
                    animateXBalance(xMov);
                    //Movement in rope
                }


                if(powerSelected == 2){
                    if(powerInput){
                        ChangeState(2);
                    }else if(powerOutput){
                        ChangeState(0);
                    }
                      //Use Hook  
                }
                else if(powerSelected == 1 && powerInput){
                    ChangeStateButton();
                }
                            //Change power selected (gona be changed)
                                if(powerChange){
                                    powerSelected ++;
                                    if(powerSelected > 2 ){
                                        powerSelected = 0;
                                    }
                                }

        }
        

    }
    private void FixedUpdate() {
        if(state == 1){
            chMov.Momentum();
        }
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
                transform.eulerAngles = new Vector3(0f,-180f,transform.eulerAngles.z);
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
    }

    private void SetBalanceState(){
        transform.eulerAngles = new Vector3(0,0,0);
        if(!rFace){
            sp.flipX = true;
        }
        chMov.LockTarget(true);
        hook.CreateHook(chMov.getTarget());
        state = 2;
    }

    private void SetFlyState(){
        if(!chMov.getGround()){
            plyAnimator.flying = true;
            rb2d.freezeRotation = false;
            state = 1;
        }
    }

    private void OnCollisionEnter2D(Collision2D other) {
        var layerMask = other.gameObject.layer;
        if ( (layerMask == 3 || layerMask == 8 )&& state != 0 && state != 2){
            ChangeState(0);
        }
        
    }

}
