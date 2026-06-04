using Unity.Mathematics;
using UnityEngine;

public class ItemSpin : MonoBehaviour
{
    private float rotateSpeed = 4;
    private float verticalMoveSpeed = 4;
    private float verticalMoveDistance = 0.25f;

    void Start()
    {
        
    }

    void Update()
    {
        transform.localPosition = new Vector3(0,Mathf.Sin(Time.time*verticalMoveSpeed)*verticalMoveDistance,0);
        transform.rotation = quaternion.Euler(0,Time.time*rotateSpeed,0);
    }
}
