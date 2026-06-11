
//using System.Numerics;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    private GameObject player;
    public Enemies enemyStats;
    private Player playerScript;
    public GameObject imagePlane;
    public float health;

    public GameObject[] itemDrops;
    public float itemDropChance = 0.25f;


    private float LastAttack = 0f;


    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerScript = player.GetComponent<Player>();
        health = enemyStats.Health;
    }

    void Update()
    {
        if (player == null) return;

        transform.position = Vector3.MoveTowards(
            transform.position,
            player.transform.position,
            enemyStats.MovementSpeed * Time.deltaTime
        );

        if (Time.time-enemyStats.AttackCooldown > LastAttack)
        {
            if ((transform.position-player.transform.position).magnitude < enemyStats.AttackRange)
            {
                playerScript.Health -= enemyStats.AttackDamage;
                LastAttack = Time.time;
            }
        }


        //rotate towards player
        Vector3 dist = player.transform.position-transform.position;
        dist = new Vector3(dist.x,0,dist.z);
        transform.forward = dist;
    }
}