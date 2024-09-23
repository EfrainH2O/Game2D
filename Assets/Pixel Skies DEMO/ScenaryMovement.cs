using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScenaryMovement : MonoBehaviour
{
    private Material material;
    [SerializeField]
    private float velocity;

    // Start is called before the first frame update
    private void Awake() {
        material = GetComponent<SpriteRenderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        material.mainTextureOffset += new Vector2(velocity * Time.deltaTime, 0);
    }
}
