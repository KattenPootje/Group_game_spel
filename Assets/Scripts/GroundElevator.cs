using UnityEngine;

public class GroundElevator : MonoBehaviour
{

    public WaveSystem waveSystem;
    public float ElevatorSpeed = 2f;
    public bool isAtNewLevel = true;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(waveSystem.levelEnding == true)
        {
            transform.Translate(Vector3.forward * Time.deltaTime * ElevatorSpeed);
        }
        if(transform.localPosition.y >= 30f*waveSystem.CurrentLevel)
        {
            waveSystem.levelEnding = false;
            transform.localPosition = new Vector3(transform.localPosition.x, 30f*waveSystem.CurrentLevel, transform.localPosition.z);
            isAtNewLevel = true;
        }
    }
}
