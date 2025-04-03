using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAOE : MonoBehaviour
{
    [SerializeField]
    private GameObject[] AOE_List;
    [SerializeField] private int AOE_Damage = 1; // Damage bullet deals
    public GameObject Boss;
    BossEnemy bossEnemy;
    PlayerHealth playerHealth;



    // Start is called before the first frame update
    void Start()
    {
        playerHealth = FindObjectOfType<PlayerHealth>();
        bossEnemy = Boss.GetComponent<BossEnemy>();
    }

    // Update is called once per frame
    void Update()
    {
        TrackBossLocation();
    }

    void TrackBossLocation()
    {
        this.transform.position = Boss.transform.position;
    }

    public void TriggerAOEAttack()
    {
        ChooseRandomAttack();
    }

    public void ChooseRandomAttack()
    {
        int num = Random.Range(0, AOE_List.Length);
        GameObject chosenAOE = AOE_List[num];

        StartCoroutine(HandleAOEAttack(chosenAOE));

    }

    private IEnumerator HandleAOEAttack(GameObject aoe)
    {
        aoe.SetActive(true); // Activate the AOE object first

        Debug.Log("HERE2");

        // Step 1: Fade in all children visuals
        foreach (Transform child in aoe.transform)
        {
            SpriteRenderer sr = child.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                Color fadedColor = sr.color;
                fadedColor.a = 0.3f; // Faded warning
                sr.color = fadedColor;
                sr.enabled = true;
            }
        }

        yield return new WaitForSeconds(1f); // Wait before activating laser

        // Step 2: Activate laser damage
        foreach (Transform child in aoe.transform)
        {
            SpriteRenderer sr = child.GetComponent<SpriteRenderer>();
            BoxCollider2D col = child.GetComponent<BoxCollider2D>();
            AOEDamage damageScript = child.GetComponent<AOEDamage>();


            if (sr != null)
            {
                Color fullColor = sr.color;
                fullColor.a = 1f; // Fully visible
                sr.color = fullColor;
            }

            if (col != null)
            {
                col.enabled = true;
            }

            if (damageScript != null)
            {
                damageScript.EnableDamage();
            }
        }

        yield return new WaitForSeconds(1f); // Laser stays active

        // Step 3: Deactivate all visuals and colliders
        foreach (Transform child in aoe.transform)
        {
            SpriteRenderer sr = child.GetComponent<SpriteRenderer>();
            BoxCollider2D col = child.GetComponent<BoxCollider2D>();
            AOEDamage damageScript = child.GetComponent<AOEDamage>();

            if (damageScript != null)
            {
                damageScript.DisableDamage();
            }

            if (sr != null) sr.enabled = false;
            if (col != null) col.enabled = false;
        }

        aoe.SetActive(false); // Deactivate the AOE after use
    }

}
