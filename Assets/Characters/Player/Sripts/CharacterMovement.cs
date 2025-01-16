using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    // "Constants/Variables"
    //Variables
    private bool grounded, canDobleJump, canJump, isJumping, canGo;
    private float proportional, jumpCount, desRotation,  velMagntitude ;

    private Quaternion actualPos, desPos;
    private Vector3 velocity = Vector3.zero;
    private Vector2 gravity;
    private float MAX_TIME = 0.3f; 
    private Transform groundCheck;
    private LayerMask groundLayer;
    
    [Header("Sonidos")]
    [SerializeField]
    private AudioClip jumpSound;
    [SerializeField]
    private float Jvol;

    [Header("VarExtra")]
    [SerializeField]
    private float speed;
    [SerializeField]
    private float  jumpForce, friction, jumpAfterForce, rotationalSpeed, mult;
     //Components
    private Rigidbody2D rb2d;
    private PlayerAnimatorController plyAnimator;



    private void Awake() {
        rb2d = GetComponent<Rigidbody2D>();    
        plyAnimator = GetComponent<PlayerAnimatorController>();
        groundCheck = gameObject.transform.GetChild(0);
    }

    void Start()
    {
        groundLayer = LayerMask.GetMask("Ground");
        gravity = new Vector2(0, Physics.gravity.y);
    }

    private void Update(){
        plyAnimator.grounded = grounded;
    }


    public void HorizontalMov(float input){
        
        Vector3 VObjective = new Vector2(input * speed, rb2d.velocity.y);
        rb2d.velocity = Vector3.SmoothDamp(rb2d.velocity, VObjective, ref velocity, friction);
    }
    public void RotatedMov(float x, float y){
        x = Math.Abs(x);
        desRotation = (float)Math.Atan(y/x); 
        // "y" y "x" son los ejes que estan en el joystick
        desRotation = desRotation*180/(float)Math.PI;
        //desired Rotation
        if(x<0){
          desRotation += 180; 
            //Debilidades de usar tangente
        }
        if(!(x==0 && y == 0)){
            desPos = Quaternion.Euler(0,transform.rotation.eulerAngles.y,desRotation);
            //Transforma la rotaion a la unidad de Quaterniones, y deja la rotacion en y porque la uso 
            //cuando quiero que el personaje gire para ver a la izquierda
            actualPos = transform.rotation;
            //consigo la rotacion actual del objeto en quaterniones
            transform.rotation = Quaternion.Lerp(actualPos, desPos, rotationalSpeed);
            //La funcion de Lerp es similar al PID para poder rotar de manera suave
            //Puede que use una mejor en un futuro (Slerp)
        }
        
        

    }

    public void VerticalMov(bool input, bool output){
        if(input){
            Grounded();
        }
        
        if(canJump && input){
            
            rb2d.velocity = new Vector2(rb2d.velocity.x, jumpForce);
            SFXManager.instance.PlaySFX(jumpSound, transform.position, Jvol);
            grounded = false;
            canJump = false;
            isJumping = true;
            
        }
        else if(canDobleJump && input){
            rb2d.velocity = new Vector2(rb2d.velocity.x, jumpForce);
            SFXManager.instance.PlaySFX(jumpSound, transform.position, Jvol);
            canDobleJump = false;
            isJumping = true;
            plyAnimator.dobleJump = true;
        }

        if(isJumping && !output){
            jumpCount += Time.deltaTime;
            proportional = 1- jumpCount/MAX_TIME;
            if(jumpCount > MAX_TIME){
                jumpCount = 0;
                isJumping = false;
            }
            rb2d.velocity -= gravity * Time.deltaTime* jumpAfterForce * proportional;
        }
        if(output){
            isJumping = false;
            jumpCount = 0;
        }
        plyAnimator.yVel =  rb2d.velocity.y;
    }
    public void Momentum(){
        velMagntitude = Vector3.Cross(rb2d.velocity, transform.right).z*mult;
        if (velMagntitude>50){
            velMagntitude = 50f;
        }else if(velMagntitude < -50f){
            velMagntitude = -50f;
        }
        //Angulo actual que movemos a radianes para la funcion de Sin y Cos
        
        if(transform.rotation.eulerAngles.y == 180){
            rb2d.AddForce( transform.up * -velMagntitude * Math.Abs(velMagntitude) , ForceMode2D.Force);
        }
        else{
            rb2d.AddForce( transform.up * velMagntitude * Math.Abs(velMagntitude) , ForceMode2D.Force);
        }
        //Remplazamos la velocidad actual con la nueva
        //La nueva esta basada por el nuevo angulo y la velocidad actual
    }
    public void Impulse(bool go){
        if(go && canGo){
            rb2d.AddForce(jumpForce*transform.right*5, ForceMode2D.Impulse);
            canGo = false;
        }
    }
    
    public void Grounded(){
        grounded = Physics2D.OverlapCapsule(groundCheck.position, new Vector2(1.6f,0.4f), CapsuleDirection2D.Horizontal,transform.rotation.eulerAngles.z,groundLayer);
        canJump = grounded? grounded: grounded;
        canDobleJump = grounded? grounded: canDobleJump;
        canGo = grounded? grounded: canGo;
        

    }
    private void OnCollisionEnter2D(Collision2D other) {
        Grounded();
    }


    public bool getGround(){
        return grounded;
    }
    
}
