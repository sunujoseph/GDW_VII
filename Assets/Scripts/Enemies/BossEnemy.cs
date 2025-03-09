using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BossEnemy : MonoBehaviour
{
    [Header("Boss Settings")]
    [SerializeField] private float agroRange = 8f;   // The boss room range
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float attackWindUpTime = 1f;
    [SerializeField] private float attackCooldown = 3f;
    [SerializeField] private int attackDamage = 2;
    [SerializeField] private float knockbackForce = 5f;
    [SerializeField] private float movementSpeed = 2f;

    [Header("Boss Stats")]
    [SerializeField] private float toughness = 5f;
    [SerializeField] private float stunDuration = 3f;
    [SerializeField] public float bossHealth = 100f;

    [Header("Components")]
    [SerializeField] private Transform attackHitbox;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Header("Sound")]
    [SerializeField] public AudioClip damageSound;
    [SerializeField] public AudioClip deathSound;
    [SerializeField] public AudioClip attackSound;

    private Rigidbody2D rb;
    private PlayerHealth playerHealth;
    private bool isHostile = false;  // Boss starts passive
    private bool isAttacking = false;
    private bool isStunned = false;
    private bool canAttack = true;
    public bool isFacingRight = true;
    private Transform playerFindTransform;

    


}
