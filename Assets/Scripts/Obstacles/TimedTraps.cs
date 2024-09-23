using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedTraps : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    private Material material;
    private BoxCollider2D boxColl2D;
    [SerializeField]
    private float timeBetweenTicks, timesToActivate, timeActive, percentage;
    public Color Deactivate;
    public Color White;
    public Color Base;
    private bool transition;
    private bool State0_1;
    
    private void Awake() {
        boxColl2D = GetComponent<BoxCollider2D>();
    }
    void Start()
    {
        StartCoroutine(Transition());
    }

    // Update is called once per frame
    void Update()
    {
        if(transition){
            if(percentage > 1f){
                percentage = 0;
            }
            percentage += Time.deltaTime / timeBetweenTicks;
            if(State0_1){
                material.color = Color.Lerp(White, Base, percentage);
            }
            else{
                material.color = Color.Lerp(Base, White, percentage);
            }
            
        }
        
        
    }
    private IEnumerator Transition(){
        //Active
        material.color = Base;
        boxColl2D.enabled = true;
        transition = false;
        percentage = 0;
        yield return new WaitForSeconds(timeActive);
            //Transition
        State0_1 = false;
        transition = true;
        yield return new WaitForSeconds(timesToActivate*timeBetweenTicks);
        //Deactive
        boxColl2D.enabled = false;
        transition = false;
        material.color = Deactivate;
        percentage = 0;
        yield return new WaitForSeconds(timeActive);
            //Transition
        State0_1 = true;
        transition = true;
        yield return new WaitForSeconds(timesToActivate*timeBetweenTicks);
        //Repeat      
        StartCoroutine(Transition());
    }


}
