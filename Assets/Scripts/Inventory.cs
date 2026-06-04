using UnityEngine;

[System.Serializable]
public class InventoryItem
{
    public int itemNumber;
    public int Ammo;
}
public class ReserveAmmo
{
    public string ammoType;
    public int Ammo;
}

public class Inventory : MonoBehaviour
{
    public InventoryItem[] Items =
    {
        new InventoryItem { itemNumber = 0, Ammo = 0},
        new InventoryItem { itemNumber = 1, Ammo = 10},
        new InventoryItem { itemNumber = 2, Ammo = 30},
    };

        public ReserveAmmo[] ReserveAmmo =
    {
        new ReserveAmmo { ammoType = "melee", Ammo = 0 },
        new ReserveAmmo { ammoType = "pistol", Ammo = 30 },
        new ReserveAmmo { ammoType = "rifle", Ammo = 90 },
    };
}