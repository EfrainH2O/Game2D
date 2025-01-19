using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FlyingSound : StateMachineBehaviour
{
    [SerializeField] private AudioClip ToActivate;
    private AudioSource temp;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        float vol = animator.GetComponent<Rigidbody2D>().velocity.magnitude / 50f;
        vol = vol < 0.15f ? 0.15f : vol;
        temp = SFXManager.instance.StartPlayingSFX(ToActivate, animator.transform.position, 0f);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        float vol = animator.GetComponent<Rigidbody2D>().velocity.magnitude / 50f;
        vol = vol < 0.1f ? 0.1f : vol;
        temp.volume = vol ;
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        SFXManager.instance.EndPlaySFX(ToActivate);
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
