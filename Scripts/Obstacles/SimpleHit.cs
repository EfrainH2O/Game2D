using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleHit : MonoBehaviour
{
    // Start is called before the first frame update
    private Transform transform;

    private void Awake() {
        transform = GetComponent<Transform>();
    }
    private void OnCollisionEnter2D(Collision2D other) {
        
        if(other.gameObject.CompareTag("Player")){
            other.gameObject.GetComponent<Player>().Hit(new Vector2 (transform.position.x, transform.position.y));
        }
    }
}
