using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    float smotth;
    private float refVel = 0f;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float x =  Mathf.SmoothDamp(transform.position.x, Player.Instance.transform.position.x, ref refVel, smotth);
        transform.position = new Vector2(x, transform.position.y);
    }
}
