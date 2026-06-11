using UnityEngine;

[System.Serializable]
public class Item
{
    public string name;
    public string weaponType;
    public int ammoType;
    public string fireMode;
    public float fireRate;
    public int magSize;
    public float recoil;
    public int damage;
    public float reloadDuration;
    public Vector3 WeaponOffsetPosition;
    public Quaternion WeaponOffsetRotation;
    public Vector3 SprintWeaponOffsetPosition;
    public Quaternion SprintWeaponOffsetRotation;
}

public class Arsenal : MonoBehaviour
{
    public Item[] Items =
    {
        new Item { name = "mjolnir", weaponType = "melee", ammoType = 0, fireMode = "", fireRate = 150, magSize = 0, recoil = 0, damage = 55, reloadDuration = 0f, WeaponOffsetPosition = new Vector3(0.5f, -0.5f, 1f), WeaponOffsetRotation = Quaternion.Euler(0f, 0f, 0f), SprintWeaponOffsetPosition = new Vector3(0.15f, -0.7f, 1f), SprintWeaponOffsetRotation = Quaternion.Euler(0f, 315f, 30f) },
        new Item { name = "Weapon2", weaponType = "gun", ammoType = 1, fireMode = "semi", fireRate = 500, magSize = 10, recoil = .6f, damage = 60, reloadDuration = 0.5f, WeaponOffsetPosition = new Vector3(0.5f, -0.8f, 1f), WeaponOffsetRotation = Quaternion.Euler(0f, 0f, 0f), SprintWeaponOffsetPosition = new Vector3(0.15f, -0.7f, 1f), SprintWeaponOffsetRotation = Quaternion.Euler(0f, 315f, 30f) },
        new Item { name = "Weapon3", weaponType = "gun", ammoType = 2, fireMode = "full", fireRate = 800, magSize = 30, recoil = .3f, damage = 70, reloadDuration = 0.5f, WeaponOffsetPosition = new Vector3(0.5f, -0.8f, 1f), WeaponOffsetRotation = Quaternion.Euler(0f, 0f, 0f), SprintWeaponOffsetPosition = new Vector3(0.15f, -0.7f, 1f), SprintWeaponOffsetRotation = Quaternion.Euler(0f, 315f, 30f) }
    };
}