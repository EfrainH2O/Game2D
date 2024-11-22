using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class CharactersMovements : MonoBehaviour{
    private bool grounded, canJump, isJumping, rFace;
    private float proportional, jumpCount ;
    [SerializeField]
    private float speed, jumpForce, friction, jumpAfterForce;
    private Rigidbody2D rb2d;
    private Transform groundCheck;
    private LayerMask groundLayer;
    private Vector2 gravity;
    private Vector3 velocity = Vector3.zero;
    private float MAX_TIME = 0.3f; 
   
    private void Awake() {
        rb2d = GetComponent<Rigidbody2D>();    
        groundCheck = gameObject.transform.GetChild(0);
    }

    void Start()
    {
        groundLayer = LayerMask.GetMask("Ground");
        gravity = new Vector2(0, Physics.gravity.y);
        rFace = true;
    }

    public void HorizontalMov(float input){
        
        Vector3 VObjective = new Vector2(input * speed, rb2d.velocity.y);
        rb2d.velocity = Vector3.SmoothDamp(rb2d.velocity, VObjective, ref velocity, friction);
    }

    public void VerticalMov(bool input, bool output){
        if(input){
            Grounded();
        }
        
        if(canJump && input){
            rb2d.velocity = new Vector2(rb2d.velocity.x, jumpForce);
            grounded = false;
            canJump = false;
            isJumping = true;
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
        //plyAnimator.yVel =  rb2d.velocity.y;
    }

    public void Grounded(){
        grounded = Physics2D.OverlapCapsule(groundCheck.position, new Vector2(1.61f,0.2f), CapsuleDirection2D.Horizontal,0 ,groundLayer);
        canJump = grounded? grounded: grounded;
        //plyAnimator.grounded = grounded;
    }
    private void OnCollisionEnter2D(Collision2D other) {
        Grounded();
    }
    public bool getGround(){
        return grounded;
    }
    
    public void animateXDir(float mov){
        if (mov == 0){
            //plyAnimator.walking = false;
        }
        else{
           // plyAnimator.walking = true;
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
}
