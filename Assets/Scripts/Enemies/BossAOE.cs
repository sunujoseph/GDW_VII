using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAOE : MonoBehaviour
{
    [SerializeField]
    private GameObject AOE1
        , AOE2, AOE3;
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
}
