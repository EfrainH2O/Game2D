using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HIted : StateMachineBehaviour
{
    private Player player;
    private Transform transform;
    private Rigidbody2D rb2d;
    private Vector3 velocity = Vector3.zero;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player = animator.GetComponent<Player>();
        transform = animator.GetComponent<Transform>();
        rb2d = animator.GetComponent<Rigidbody2D>();
        animator.GetComponent<Hook>().DestroyHook();

        

    }
    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        rb2d.velocity = Vector3.SmoothDamp(rb2d.velocity, new Vector2(0,0), ref velocity, 0.3f);
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {   
        transform.position = player.GetSpawnpt();
        rb2d.angularVelocity = 0;

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
