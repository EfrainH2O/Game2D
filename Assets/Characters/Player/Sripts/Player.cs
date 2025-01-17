using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public static Player Instance;
    private Vector3 spawnpoint;
    private int level;
    private Vector2 forceDirection;
    private bool hited;
    private float gravity;
    [SerializeField]
    private float mult;

    //Components
    private Rigidbody2D rb2d;
    private PlayerAnimatorController plyAnimController;

    private RigidbodyConstraints2D before;
    // Start is called before the first frame update
    void Awake()
    {
        if (Instance == null)
            Instance = this;

        rb2d = GetComponent<Rigidbody2D>();
        plyAnimController = GetComponent<PlayerAnimatorController>();
    }
    void Start(){
        gravity = rb2d.gravityScale;
        spawnpoint = transform.position;
        level = 0;
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
    public void SetSpawnpt(Vector3 spwn){
        spawnpoint = spwn;
    }
    public void SetLevel(int Next){
        level = Next;
    }

    public int GetLevel(){
        return level;
    }
    public void SetHited(){
        hited = false;
        plyAnimController.Hit = hited;
    }
    public void ReturnGravity(){
        rb2d.gravityScale = gravity;
    }

    public void Freeze(){
        GetComponent<CharacterControlls>().enabled = false;
        before = rb2d.constraints;
        rb2d.constraints = RigidbodyConstraints2D.FreezePosition | RigidbodyConstraints2D.FreezeRotation;
    }

    public void UnFreeze(){
        GetComponent<CharacterControlls>().enabled = true;
        rb2d.constraints = before;
    }
}
