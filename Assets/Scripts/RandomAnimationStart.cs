using UnityEngine;
using System.Collections;

public class RandomAnimationStart : MonoBehaviour {

    private Animator animator;

	void Start () {
        animator = GetComponent<Animator>();
        animator.enabled = true;
        animator.StopPlayback();
        animator.Play("Normal", 0, Random.Range(0.0f, 3.0f));

    }
}
