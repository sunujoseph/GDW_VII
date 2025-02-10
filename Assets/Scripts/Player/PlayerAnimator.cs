using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    protected Animator animator;

    [Header("Animation State Variables")]
    public bool wasHit;
    public bool isAlive;
    public Vector2 movement;
    public bool dash;
    public bool inAir;
    public bool parry;
    public bool reflect;
    public bool attacking;
    public int attackNumber;
    public float direction;
    public bool hitBelow;

    public virtual void Init()
    {
        animator = GetComponent<Animator>();
        wasHit = false;
        hitBelow = false;
        isAlive = true;
        dash = false;
        inAir = true;
        parry = false;
        attacking = false;
        reflect = false;
        attackNumber = 0;


        HandleAnimations();
    }

    private void HandleAnimations()
    {
        animator.SetFloat("xSpeed", Mathf.Abs(movement.x));
        animator.SetFloat("ySpeed", Mathf.Abs(movement.y));
        animator.SetFloat("Direction", direction);
        animator.SetBool("inAir", inAir);
        animator.SetBool("wasHit", wasHit);
        animator.SetBool("isAlive", isAlive);
        animator.SetBool("isDashing", dash);
        animator.SetBool("isParry", parry);
        animator.SetBool("isAttacking", attacking);
        animator.SetInteger("attackNumber", attackNumber);
        animator.SetBool("hitBelow", hitBelow);
        animator.SetBool("Reflect", reflect);
    }
}
