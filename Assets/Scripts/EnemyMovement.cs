using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    private GameObject player;
    public Enemies enemyStats;
    private Player PlayerScript;
    public float Health;
    public float speed = 3f;

    private float LastAttack = 0f;


    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        PlayerScript = player.GetComponent<Player>();
        Health = enemyStats.Health;
    }

    void Update()
    {
        if (player == null) return;

        transform.position = Vector3.MoveTowards(
            transform.position,
            player.transform.position,
            speed * Time.deltaTime
        );

        if (Time.time-enemyStats.AttackCooldown > LastAttack)
        {
            if ((transform.position-player.transform.position).magnitude < enemyStats.AttackRange)
            {
                PlayerScript.Health -= enemyStats.AttackDamage;
                LastAttack = Time.time;
            }
        }
    }
}