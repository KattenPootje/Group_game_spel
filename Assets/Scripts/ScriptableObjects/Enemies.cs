using UnityEngine;

[CreateAssetMenu(fileName = "Enemies", menuName = "Scriptable Objects/Enemies")]
public class Enemies : ScriptableObject
{
    public float Health;
    public float Damage;
    public float Range;
    public float Speed;
    public string Name;
}
