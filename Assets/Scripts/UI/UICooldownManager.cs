using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UICooldownManager : MonoBehaviour
{
    private static UICooldownManager instance;

    [Header("UI Cooldowns")]
    [SerializeField] private Image dashCooldownImage;
    [SerializeField] private Image parryCooldownImage;
    [SerializeField] private Image jumpCooldownImage;

    [Header("Attack Combo Cooldowns")]
    [SerializeField] private Image attack1CooldownImage;
    [SerializeField] private Image attack2CooldownImage;
    [SerializeField] private Image attack3CooldownImage;

    private float attackCooldown;
    private float dashCooldown;
    private float parryCooldown;

    private bool attack1Ready = true;
    private bool attack2Ready = true;
    private bool attack3Ready = true;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        CheckComponents();
    }

    private void Start()
    {

        //Invoke("ReassignUIElements", 0.1f);

        if (dashCooldownImage == null || parryCooldownImage == null || jumpCooldownImage == null)
        {
            //Debug.Log("help");
            CheckComponents();
        }

        // Ensure cooldowns start invisible
        ResetCooldowns();
    }

    private void Update()
    {
        if (dashCooldownImage == null || parryCooldownImage == null || jumpCooldownImage == null)
        {
            //Debug.Log("help");
            CheckComponents();
        }


    }

    private void ResetCooldowns()
    {
        dashCooldownImage.fillAmount = 0f;
        parryCooldownImage.fillAmount = 0f;
        jumpCooldownImage.fillAmount = 1f; // Jump is available by default

        attack1CooldownImage.enabled = true;
        attack2CooldownImage.enabled = false;
        attack3CooldownImage.enabled = false;
    }


    public void InitializeCooldowns(float attackCD, float dashCD, float parryCD)
    {
        attackCooldown = attackCD;
        dashCooldown = dashCD;
        parryCooldown = parryCD;
    }

    public void StartAttackCooldown(int attackStep)
    {

        if (attack1CooldownImage == null) CheckComponents();
        if (attack2CooldownImage == null) CheckComponents();
        if (attack3CooldownImage == null) CheckComponents();

        switch (attackStep)
        {
            case 1:
                attack1CooldownImage.enabled = false;
                attack2CooldownImage.enabled = true;
                attack3CooldownImage.enabled = false;
                break;
            case 2:
                attack1CooldownImage.enabled = false;
                attack2CooldownImage.enabled = false;
                attack3CooldownImage.enabled = true;
                break;
            case 3:
                attack1CooldownImage.enabled = true;
                attack2CooldownImage.enabled = false;
                attack3CooldownImage.enabled = false;
                break;
        }
    }

    public void ResetAttackImageAfterDelay()
    {
        attack1CooldownImage.enabled = true;
        attack2CooldownImage.enabled = false;
        attack3CooldownImage.enabled = false;
    }

    public void StartDashCooldown(float cooldown)
    {
        dashCooldownImage.fillAmount = 1f;
        StartCoroutine(FillCooldown(dashCooldownImage, cooldown, null));
    }

    public void StartParryCooldown(float cooldown)
    {
        parryCooldownImage.fillAmount = 1f;
        StartCoroutine(FillCooldown(parryCooldownImage, cooldown, null));
    }

    public void UpdateJumpState(bool canJump)
    {
        jumpCooldownImage.fillAmount = canJump ? 1f : 0f;
    }

    private IEnumerator FillCooldown(Image image, float duration, System.Action onComplete)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            image.fillAmount = 1f - (elapsed / duration);
            yield return null;
        }
        image.fillAmount = 0f; // Ensure cooldown resets to zero
        onComplete?.Invoke();
    }


    void CheckComponents()
    {


        if (dashCooldownImage == null) dashCooldownImage = GameObject.Find("DashFill")?.GetComponent<Image>();
        if (parryCooldownImage == null) parryCooldownImage = GameObject.Find("ParryFill")?.GetComponent<Image>();
        if (jumpCooldownImage == null) jumpCooldownImage = GameObject.Find("JumpFill")?.GetComponent<Image>();

        if (attack1CooldownImage == null) attack1CooldownImage = GameObject.Find("AttackFill1")?.GetComponent<Image>();
        if (attack2CooldownImage == null) attack2CooldownImage = GameObject.Find("AttackFill2")?.GetComponent<Image>();
        if (attack3CooldownImage == null) attack3CooldownImage = GameObject.Find("AttackFill3")?.GetComponent<Image>();

        // Reset fill amounts
        if (dashCooldownImage != null) dashCooldownImage.fillAmount = 0f;
        if (parryCooldownImage != null) parryCooldownImage.fillAmount = 0f;
        if (jumpCooldownImage != null) jumpCooldownImage.fillAmount = 1f;
    }



}
