using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiveCheckpoint : MonoBehaviour
{
    [SerializeField]
    private int level;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other) {
        Player temp = other.GetComponent<Player>();
        if( temp != null){
            if(temp.GetLevel() <= level){
                temp.SetSpawnpt( transform.position);
                temp.SetLevel(level);
            }
        }
    }
}
