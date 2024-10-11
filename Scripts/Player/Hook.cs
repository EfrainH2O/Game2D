using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Timeline;

public class Hook : MonoBehaviour
{

    private LineRenderer lr;
    private HingeJoint2D join2d;
    private Rigidbody2D rb2d;

    private float angulePosition;
    private Vector2 distance;
    [SerializeField]
    private float ropeSpeed;
    // Start is called before the first frame update
    private void Awake() {
        lr = GetComponent<LineRenderer>();
        rb2d = GetComponent<Rigidbody2D>();
    }
    private void Update() {
        if(lr.positionCount == 2){
            lr.SetPosition(0, transform.position);
        }
    }

    public void CreateHook(Collider2D target){

        if(target != null){
            lr.positionCount = 2;
            lr.SetPosition(0, transform.position);
            lr.SetPosition(1, target.transform.position);
            if(target.tag == "Pivot"){
                StartSwing(target);

            }else if(target.tag == "Impulse"){
                //Impulse
            }

            
        }
            
            //Action Of Hook depending on layer
    }
    public void DestroyHook(){
        lr.positionCount = 0;
        if(join2d != null){
            Destroy(join2d);
            transform.eulerAngles = new Vector3(0,transform.rotation.eulerAngles.y,0);
            rb2d.freezeRotation = true;

        }
    }

    private void StartSwing(Collider2D target){
        distance = transform.position-target.transform.position;
        Debug.Log(transform.position+ " - "+ target.transform.position);
        
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
        Debug.Log(" = " + distance + " theta: " + angulePosition);
        
        

    }

    public void MovementInHook(float input){
        rb2d.AddForce(input * 10f * transform.right,ForceMode2D.Force);
    }



}
