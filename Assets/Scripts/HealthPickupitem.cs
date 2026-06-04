using UnityEngine;

public class HealthPickupitem : MonoBehaviour
{
    public int healthAmount = 50;
    private Inventory inventoryScript;
    private Player playerScript;
    private GameObject player;


    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerScript = player.GetComponent<Player>();
    }

    void Update()
    {
        if ((transform.position-player.transform.position).magnitude < 1)
        {
            playerScript.Health += healthAmount;
            Destroy(gameObject);
        }
    }
}
