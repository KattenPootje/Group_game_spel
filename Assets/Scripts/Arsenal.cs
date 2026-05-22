using UnityEngine;

[System.Serializable]
public class Item
{
    public string name;
    public string weaponType;
    public string fireMode;
    public float fireRate;
    public float recoil;
    public int damage;
    public Vector3 WeaponOffsetPosition;
    public Quaternion WeaponOffsetRotation;
    public Vector3 SprintWeaponOffsetPosition;
    public Quaternion SprintWeaponOffsetRotation;
}

public class Arsenal : MonoBehaviour
{
    public Item[] Items =
    {
        new Item { name = "mjolnir", weaponType = "melee", fireMode = "", fireRate = 150, recoil = 0, damage = 55, WeaponOffsetPosition = new Vector3(0.5f, -0.5f, 1f), WeaponOffsetRotation = Quaternion.Euler(0f, 0f, 0f), SprintWeaponOffsetPosition = new Vector3(0.15f, -0.7f, 1f), SprintWeaponOffsetRotation = Quaternion.Euler(0f, 315f, 30f) },
        new Item { name = "Weapon2", weaponType = "gun", fireMode = "semi", fireRate = 500, recoil = .6f, damage = 60, WeaponOffsetPosition = new Vector3(0.5f, -0.8f, 1f), WeaponOffsetRotation = Quaternion.Euler(0f, 0f, 0f), SprintWeaponOffsetPosition = new Vector3(0.15f, -0.7f, 1f), SprintWeaponOffsetRotation = Quaternion.Euler(0f, 315f, 30f) },
        new Item { name = "Weapon3", weaponType = "gun", fireMode = "full", fireRate = 800, recoil = .3f, damage = 70, WeaponOffsetPosition = new Vector3(0.5f, -0.8f, 1f), WeaponOffsetRotation = Quaternion.Euler(0f, 0f, 0f), SprintWeaponOffsetPosition = new Vector3(0.15f, -0.7f, 1f), SprintWeaponOffsetRotation = Quaternion.Euler(0f, 315f, 30f) }
    };
}