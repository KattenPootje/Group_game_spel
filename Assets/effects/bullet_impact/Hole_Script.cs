using UnityEngine;

public class Hole_Script : MonoBehaviour
{
    public float lifeTime = 10;
    private float startTime;
    void Start()
    {
        startTime = Time.time;
    }

    void Update()
    {
        if (Time.time - lifeTime > startTime)
        {
            Destroy(gameObject);
        }
    }
}
