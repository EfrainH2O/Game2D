using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{

    private Vector3 spawnpoint;
    private Vector2 forceDirection;
    private bool hited;
    private float gravity;
    [SerializeField]
    private float mult;

    //Components
    private Rigidbody2D rb2d;
    private PlayerAnimatorController plyAnimController;
    // Start is called before the first frame update
    void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
        plyAnimController = GetComponent<PlayerAnimatorController>();
    }
    void Start(){
        gravity = rb2d.gravityScale;
        spawnpoint = transform.position;
    }
    public void Hit(Vector2 direction){
        if(!hited){
            hited = true;
            rb2d.gravityScale = 0;
            forceDirection = new Vector2(transform.position.x-direction.x, transform.position.y-direction.y).normalized;
            rb2d.AddForce(forceDirection*mult,ForceMode2D.Impulse );
            plyAnimController.Hit = hited;

        }
    }
    public bool GetHited(){
        return hited;
    }
    public Vector3 GetSpawnpt(){
        return spawnpoint;
    }

    public void SetHited(){
        hited = false;
        plyAnimController.Hit = hited;
    }
    public void ReturnGravity(){
        rb2d.gravityScale = gravity;
    }
}
