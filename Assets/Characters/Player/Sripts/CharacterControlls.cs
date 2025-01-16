using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.InputSystem;
using UnityEngine;
using UnityEditor.Rendering;
using System.Runtime.CompilerServices;
public enum States{
    Normal,
    Fly,
    Chained
}
public class CharacterControlls : MonoBehaviour
{
    // Components
    private CharacterMovement chMov;
    private Player player;
    private PlayerAnimatorController plyAnimator;
    private Hook hook;
    private Rigidbody2D rb2d;
    private SpriteRenderer sp;
    private LineRenderer lr;
    // Variables
    private float xMov, yMov;
    private bool yStart, yEnd, powerInput, powerOutput, planePow, lockTarget;
    [SerializeField]
    private States state, prevState;
    private bool rFace;
    
    private void Awake() {
        chMov = GetComponent<CharacterMovement>();
        player = GetComponent<Player>();
        plyAnimator = GetComponent<PlayerAnimatorController>();
        hook = GetComponent<Hook>();
        sp = GetComponent<SpriteRenderer>();
        lr = GetComponent<LineRenderer>();
        rb2d = GetComponent<Rigidbody2D>();

    }
    void Start()
    {
        rFace = true;
        state = States.Normal;
        prevState = state;
    }

    // Update is called once per frame
    void Update()
    {
        
        
        if (player.GetHited())
        {
            chMov.HorizontalMov(0);
            chMov.VerticalMov(false, false);
            hook.LockTarget(false);
    
        }
        else
        {
            //Inputs
                        xMov = Input.GetAxisRaw("Horizontal");
                        yMov = Input.GetAxisRaw("Vertical");
                        yStart = Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.JoystickButton0);
                        yEnd = Input.GetKeyUp(KeyCode.Space) || Input.GetKeyUp(KeyCode.JoystickButton0);
                        powerInput = Input.GetButtonDown("Power") ;
                        powerOutput = Input.GetButtonUp("Power") || !Input.GetButton("Power");
                        planePow = Input.GetKeyDown(KeyCode.E);
                        lockTarget = Input.GetKey(KeyCode.LeftControl);

            //Actions depending on the state
            
                if(state == States.Normal){
                    chMov.HorizontalMov(xMov);
                    chMov.VerticalMov(yStart, yEnd);
                    animateXDir(xMov);
                    //Normal movement
                    
                }
                else if (state == States.Fly){
                    chMov.RotatedMov(xMov, yMov);
                    chMov.Impulse(yStart);
                    //Movement in plane
                }
                else if(state == States.Chained && hook.GetisConnected()){
                    hook.MovementInHook(xMov);
                    animateXBalance(xMov);
                    //Movement in rope
                }

//TODO
                if(powerInput){
                    hook.LockTarget(true);
                    hook.StartHook();
                }else if(powerOutput && lr.positionCount > 1){
                    hook.DestroyHook();
                }


                if(state != States.Chained && hook.GetisConnected()){      
                    ChangeState(States.Chained);   

                }
                else if(state == States.Chained  &&! hook.GetisConnected()){  //Hook
                    ChangeState(prevState);
                }

                if(planePow){
                    ChangeStateButton(); // plane
                }

        }
        

    }
    private void FixedUpdate() {
        if(state == States.Fly ){
            chMov.Momentum();
        }
        hook.LockTarget(lockTarget);
        
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

    public void ChangeState(States desState){
        prevState = state;
        if(desState == States.Fly){
            SetFlyState();
            
        }else if(desState == States.Normal){
            SetGroundState();
        }
        else if(desState == States.Chained){
            SetBalanceState();
        }
    }

    private void ChangeStateButton(){
        if(state == States.Fly){
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
            state = States.Normal;
    }

    private void SetBalanceState(){
        if(!rFace){
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, 0, transform.eulerAngles.z);
            sp.flipX = true;
        }
        state = States.Chained;
       
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
            state = States.Fly;
        }else{
            SetGroundState();
        }
    }

    private void OnCollisionEnter2D(Collision2D other) {
        var layerMask = other.gameObject.layer;
        if ( layerMask == 3 || layerMask == 8 ){
            if(state == States.Fly || (state == States.Chained && other.relativeVelocity.magnitude > 20)){
                ChangeState(States.Normal);
            }
        }
        
    }
    public void OnDisable(){
        plyAnimator.walking = false;
    }
}
