using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private Enemies enemyStats;
    [SerializeField] private Player Players;
    public float speed = 3f;


    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        Players = player.GetComponent<Player>();
    }

    void Update()
    {
        if (player == null) return;

        transform.position = Vector3.MoveTowards(
            transform.position,
            player.transform.position,
            speed * Time.deltaTime
        );
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Something entered trigger");

        if(other.CompareTag("Player"))
        {
            Debug.Log("Player entererd trigger");
            Players.Health -= enemyStats.Damage;
            Debug.Log($"{enemyStats.Name} dealt {enemyStats.Damage} damage! {Players.Health} health remaining.");
        }
    }
}