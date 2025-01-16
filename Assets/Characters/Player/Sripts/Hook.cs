using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Android.Types;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Timeline;

public class Hook : MonoBehaviour
{

    private LineRenderer lr;
    private HingeJoint2D join2d;
    private Rigidbody2D rb2d;
    //temp values
    private float pivoteDistance, targetDistance, angulePosition;
    private Collider2D temp;
    private Vector3 tempHookPos;
    // control variables
    private Collider2D target;
    private Collider2D tempTarget;
    private bool isConnected;
    private int targetAmount;
    private float selectionGrace;
    private Vector2 distance;
    private bool inGrace;
    private bool traversing;
    private Collider2D[] targets = new Collider2D[5];
    private GameObject SelectionMark;
    // Serialize Values
    [SerializeField]
    private float ropeSpeed;
    [SerializeField]
    private float effectArea;
    [SerializeField]
    private AudioClip HangingSound;
    [SerializeField]
    private float MAXselectionGrace;
    [SerializeField]
    private GameObject SelectionMarkPrefab;
    [Header("Filtro para Targets")]
    //Constants
    [SerializeField]
    private ContactFilter2D focus;
    private void Awake() {
        selectionGrace = 0;
        lr = GetComponent<LineRenderer>();
        rb2d = GetComponent<Rigidbody2D>();
        tempHookPos = transform.position;
        lr.positionCount = 0;
        traversing = false;

    }
    private void Update() {
        
        if(traversing){
            //TODO
            tempHookPos = Vector3.MoveTowards(tempHookPos, target.transform.position, ropeSpeed * Time.deltaTime);
            if(tempHookPos == target.transform.position){
                traversing = false;
                if(target.transform == transform){
                    lr.positionCount = 0;
                    targetAmount = 0;
                }else{
                    SFXManager.instance.PlaySFX(HangingSound, transform.position,1f);
                    if(target.tag == "Pivot"){
                        StartSwing();
                    }else if(target.tag == "Impulse"){
                        StartImpulse();
                    }   
                }
            }
        }
        
        if(lr.positionCount == 2){
            lr.SetPosition(1, transform.position);
            lr.SetPosition(0, tempHookPos);
        }

    }

    public void StartHook(){
        tempHookPos = transform.position;
        target = tempTarget;
        if(targetAmount > 0 ){  
            lr.positionCount = 2;
            traversing = true;
        } 
    }
    public void DestroyHook(){
        if(join2d != null){
            Destroy(join2d);
            rb2d.freezeRotation = true;
        }
        traversing = true;
        target = gameObject.GetComponent<Collider2D>();
        isConnected = false;
    }
    
    bool tempprev = false;
    public void LockTarget(bool Button){
        if(Button && !tempprev){
            inGrace = true;
            selectionGrace = MAXselectionGrace + 1;
        }
        if(Button){
            GraceTimer();
            targetAmount = Physics2D.OverlapCircle(transform.position, effectArea, focus, targets );
            if(targetAmount > 0){
                quickSort(0,targetAmount-1);
            }         
            ChangeSelectionMark();
        }
        else{
            if(SelectionMark != null ){
                if(inGrace){
                    Destroy(SelectionMark);
                    selectionGrace = 0;
                    inGrace = false;
                }else{
                    GraceTimer();
                }
            }
        }
        tempprev = Button;
    }
    private void ChangeSelectionMark(){
        if(inGrace){
            if(targetAmount > 0){
                tempTarget = targets[0];
                if(SelectionMark == null ){
                    SelectionMark = Instantiate(SelectionMarkPrefab, tempTarget.transform);
                }
                else if(tempTarget.transform !=SelectionMark.transform ){
                    SelectionMark.transform.position = tempTarget.transform.position;
                }else{
                    return;
                }
            }else{
                if(SelectionMark != null ){
                    Destroy(SelectionMark);
                }else{
                    return;
                }
            }
            selectionGrace = 0;
            inGrace = false;
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

    private void StartSwing(){
        isConnected = true;
        distance = transform.position-target.transform.position;
        //Debug.Log(transform.position+ " - "+ target.transform.position);
        
        rb2d.freezeRotation = false;
        if(join2d == null){
            join2d = this.AddComponent<HingeJoint2D>();
        }
        
        join2d.autoConfigureConnectedAnchor = false;
        join2d.connectedAnchor = target.transform.position;
        join2d.anchor = new Vector2(0, distance.magnitude);
        join2d.enableCollision = true;
        angulePosition = (float) (Math.Atan(distance.y/distance.x) *180/Math.PI);
        if(distance.x <0){
            angulePosition +=180;
        }
        angulePosition +=90;
        
        
        transform.eulerAngles = new Vector3(transform.eulerAngles.x,transform.eulerAngles.y, angulePosition );
        //Debug.Log(" = " + distance + " theta: " + angulePosition);
        
    }

    public void MovementInHook(float input){
        rb2d.AddForce(input * 10f * transform.right,ForceMode2D.Force);
    }

    public void StartImpulse(){
        rb2d.velocity = Vector2.zero;
        Vector3 direction = target.transform.position - transform.position;
        rb2d.AddForce( 40f*direction.normalized  , ForceMode2D.Impulse);
        DestroyHook();

    }

    public bool GetisConnected(){
        return isConnected;
    }
    public int getTargetAmount(){
        return targetAmount;
    }

}
