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
    private float selectionGrace = 0;
    public bool inGrace;
    [SerializeField]
    private float MAXselectionGrace;
    private Quaternion actualPos, desPos;
    private int targetAmount;
    private GameObject SelectionMark;
    private Collider2D[] targets = new Collider2D[5];
    private Collider2D temp;
    float pivoteDistance;
    float targetDistance;
    [Header("Filtro para Targets")]
    //Constants
    [SerializeField]
    private ContactFilter2D focus;
    private Vector3 velocity = Vector3.zero;
    private Collider2D target;
    private Vector2 gravity;
    private float MAX_TIME = 0.3f; 
    private Transform groundCheck;
    private LayerMask groundLayer;
    [SerializeField]
    private GameObject SelectionMarkPrefab;
    [Header("Sonidos")]
    [SerializeField]
    private AudioClip jumpSound;
    [SerializeField]
    private float Jvol;

    [Header("VarExtra")]
    [SerializeField]
    private float speed;
    [SerializeField]
    private float  jumpForce, friction, jumpAfterForce, rotationalSpeed, mult, effectArea;
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

    public void LockTarget(bool Button){
        
        if(Button){
            GraceTimer();
            for(int i = 0; i < 5; i++){
                targets[i] = null;
            }
            targetAmount = Physics2D.OverlapCircle(transform.position, effectArea, focus, targets );
            if(targetAmount != 0){
                quickSort(0,targetAmount-1);
            }
            ChangeSelectionMark();
        }
        else{
            
            if(SelectionMark != null ){
                
                if(inGrace){
                    Destroy(SelectionMark);
                    selectionGrace = 0;
                }else{
                    GraceTimer();
                }
                
            }
        }
    }

    private void ChangeSelectionMark(){
        if(targets[0] != null){
            if(SelectionMark == null){
                SelectionMark = Instantiate(SelectionMarkPrefab, targets[0].transform);
                target = targets[0];
            }
            
            else if(SelectionMark !=  null && targets[0].transform !=SelectionMark.transform && inGrace){
                Destroy(SelectionMark);
                SelectionMark = Instantiate(SelectionMarkPrefab, targets[0].transform);
                target = targets[0];
                selectionGrace = 0;
            }
        }else{
            target = null;
        }
            
            
    }

    private void quickSort(int low, int n){

        int j =  low-1;
        if (n <= low){
            return;
        }
        pivoteDistance = (transform.position - targets[n].GetComponent<Transform>().position).magnitude;
        for (int i = low; i <= n; i++){
            targetDistance = (transform.position - targets[i].GetComponent<Transform>().position).magnitude;

            if(targetDistance <= pivoteDistance){
                j++;
                if(i>j){
                    temp = targets[i];
                    targets[i] = targets[j];
                    targets[j] = temp;
                }
            }
        }

        quickSort(low,j-1);
        quickSort(j+1,n);
        return;
        
    }

    private void GraceTimer(){
        if(selectionGrace < MAXselectionGrace){
            selectionGrace += Time.deltaTime;
            inGrace = false;
        }else{
            inGrace = true;
        }

    }

    public bool getGround(){
        return grounded;
    }

    public Collider2D getTarget(){
        LockTarget(true);
        return target;
    }
    
}
