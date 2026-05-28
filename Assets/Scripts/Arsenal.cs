using UnityEngine;

[System.Serializable]
public class Item
{
    public string name;
    public string type;
    public int damage;
    public Vector3 DefaultWeaponOffsetPosition;
    public Quaternion DefaultWeaponOffsetRotation;
    public Vector3 SprintWeaponOffsetPosition;
    public Quaternion SprintWeaponOffsetRotation;
}

public class Arsenal : MonoBehaviour
{
    public Item[] Items =
    {
        new Item { name = "mjolnir", type = "melee", damage = 55, DefaultWeaponOffsetPosition = new Vector3(0.5f, -0.5f, 1f), DefaultWeaponOffsetRotation = Quaternion.Euler(0f, 0f, 0f), SprintWeaponOffsetPosition = new Vector3(0.5f, -0.8f, 1f), SprintWeaponOffsetRotation = Quaternion.Euler(0f, 45f, 35f) },
        new Item { name = "Weapon2", type = "gun", damage = 60, DefaultWeaponOffsetPosition = new Vector3(0.5f, -0.8f, 1f), DefaultWeaponOffsetRotation = Quaternion.Euler(0f, 0f, 0f), SprintWeaponOffsetPosition = new Vector3(0.5f, -0.8f, 1f), SprintWeaponOffsetRotation = Quaternion.Euler(0f, 45f, 35f) },
        new Item { name = "Weapon3", type = "gun", damage = 70, DefaultWeaponOffsetPosition = new Vector3(0.5f, -0.8f, 1f), DefaultWeaponOffsetRotation = Quaternion.Euler(0f, 0f, 0f), SprintWeaponOffsetPosition = new Vector3(0.5f, -0.8f, 1f), SprintWeaponOffsetRotation = Quaternion.Euler(0f, 45f, 35f) }
    };
}