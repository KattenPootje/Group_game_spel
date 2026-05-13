using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class Player : MonoBehaviour
{
    private Rigidbody rb;
    public Camera Camera;
    private float xRotation = 0f;
    private float yRotation = 0f;
    private bool isGrounded = false;
    private float CameraFollowHeight;
    private float walkWobbleTime = 0f;
    private float smoothMovementIntensity = 0f;
    private Vector3 camShake = Vector3.zero;
    private Vector3 smoothCamShake = Vector3.zero;
    private float inputDistance = 0f;


    public float Sensitivity = 2f;
    public float WalkSpeed = 5f;
    public float JumpPower = 15f;
    public float Friction = 0.5f;
    public float JumpMovementBoost = 1.25f;
    public float CameraFollowSpeed = 0.75f;
    public float walkWobbleSpeed = 25f;
    public float walkWobbleIntensity = 5f;
    public float camShakeSpeed = 0.2f;

    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
        CameraFollowHeight = Camera.transform.position.y;
    }

    void Update()
    {

        inputDistance = math.sqrt(Input.GetAxis("Vertical") * Input.GetAxis("Vertical") + Input.GetAxis("Horizontal") * Input.GetAxis("Horizontal"));
        smoothMovementIntensity = Mathf.Lerp(smoothMovementIntensity, Mathf.Clamp(inputDistance, 0f, 1f), Mathf.Clamp(0.15f*(Time.deltaTime*60), 0f, 1f));
        xRotation += Input.GetAxis("Mouse X")*Sensitivity;
        yRotation = math.clamp(yRotation-Input.GetAxis("Mouse Y")*Sensitivity, -75f, 75f);
        transform.rotation = Quaternion.Euler(0, xRotation, 0f);

        CameraFollowHeight = CameraFollowHeight - ( CameraFollowHeight - (transform.position.y + 0.6f) ) / CameraFollowSpeed*(Time.deltaTime*60);
        if (Physics.SphereCast(transform.position, 0.2f, -Vector3.up, out RaycastHit hit, (float)(rb.GetComponent<Collider>().bounds.extents.y + 0.1)))
        {
            if (isGrounded == false)
            {
                camShake += new Vector3(5,0,0);
            }
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
        
        if (isGrounded)
        {
            walkWobbleTime += smoothMovementIntensity*(Time.deltaTime*60);
            if (Input.GetKeyDown(KeyCode.Space))
            {
                camShake += new Vector3(-5,0,0);
                rb.linearVelocity = new Vector3(rb.linearVelocity.x*JumpMovementBoost, rb.linearVelocity.y+JumpPower, rb.linearVelocity.z*JumpMovementBoost);
            }
        }
        float walkWobbleX = Mathf.Sin(walkWobbleTime * walkWobbleSpeed) * smoothMovementIntensity;
        float walkWobbleY =  Mathf.Sin(walkWobbleTime * walkWobbleSpeed*2) * smoothMovementIntensity;

        camShake = Vector3.Lerp(camShake, Vector3.zero, Mathf.Clamp(camShakeSpeed*(Time.deltaTime*60), 0f, 1f));
        smoothCamShake = Vector3.Lerp(smoothCamShake, camShake, Mathf.Clamp(camShakeSpeed*(Time.deltaTime*60), 0f, 1f));

        Camera.transform.position = new Vector3(Camera.transform.position.x, CameraFollowHeight, Camera.transform.position.z);
        Camera.transform.localRotation = Quaternion.Euler(yRotation + (walkWobbleY*walkWobbleIntensity) + smoothCamShake.x, (walkWobbleX*walkWobbleIntensity) + smoothCamShake.y, smoothCamShake.z);
    }

    void FixedUpdate()
    {
            if (isGrounded)
            {
                if (inputDistance > 0)
                {
                    rb.AddForce((transform.forward*Input.GetAxis("Vertical")+transform.right*Input.GetAxis("Horizontal"))/Mathf.Clamp(inputDistance, 1, 1.5f)*WalkSpeed);
                    
                }
                rb.linearVelocity = new Vector3(rb.linearVelocity.x / (1+(Friction)), rb.linearVelocity.y, rb.linearVelocity.z / (1+(Friction)));
            }
    }
}
