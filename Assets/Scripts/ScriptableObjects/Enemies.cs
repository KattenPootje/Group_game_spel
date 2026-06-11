using UnityEngine;

[CreateAssetMenu(fileName = "Enemies", menuName = "Scriptable Objects/Enemies")]
public class Enemies : ScriptableObject
{
    public string Name;
    public float Health;
    public float MovementSpeed;
    public float AttackRange;
    public float AttackCooldown;
    public float AttackDamage;
}
