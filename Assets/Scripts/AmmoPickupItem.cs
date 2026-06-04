using UnityEngine;

public class AmmoPickupItem : MonoBehaviour
{
    public int ammoType = 2;
    public int ammoAmount = 60;
    private Inventory inventoryScript;
    private GameObject player;


    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        inventoryScript = player.GetComponent<Inventory>();
    }

    void Update()
    {
        if ((transform.position-player.transform.position).magnitude < 1)
        {
            inventoryScript.ReserveAmmo[ammoType].Ammo += ammoAmount;
            Destroy(gameObject);
        }
    }
}
