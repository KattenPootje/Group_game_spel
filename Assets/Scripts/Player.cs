using System;
using System.Threading.Tasks;
using Mono.Cecil.Cil;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SocialPlatforms;

public class Player : MonoBehaviour // kut kjelt blijf uit me kanker code
{
    private Rigidbody rb;
    public Camera Camera;
    public Arsenal Arsenal;
    private float xRotation = 0f;
    private float yRotation = 0f;
    private bool isGrounded = false;
    private float CameraFollowHeight;
    private float walkWobbleTime = 0f;
    private float smoothMovementIntensity = 0f;
    private Vector3 camShake = Vector3.zero;
    private Vector3 smoothCamShake = Vector3.zero;
    private Vector3 weaponShake = Vector3.zero;
    private Vector3 smoothWeaponShake = Vector3.zero;
    private float inputDistance = 0f;
    private float SmoothDeltaX = 0f;
    private float SmoothDeltaY = 0f;
    private bool sprinting = false;
    private float sprintingAnimation = 0f;
    private int CurrentWeapon = 0;
    private float Recoil = 0;
    private float lastFire;
    private bool firing = false;


    public float Sensitivity = 2f;
    public float WalkSpeed = 5f;
    public float SprintSpeedMultiplier = 1.5f;
    public float JumpPower = 15f;
    public float Friction = 0.5f;
    public float JumpMovementBoost = 1.25f;
    public bool useSprintToggle = true;
    public float sprintTransitionSpeed = 0.1f;
    public float CameraFollowSpeed = 0.75f;
    public float walkWobbleSpeed = 25f;
    public float walkWobbleIntensity = 5f;
    public float camShakeDamping = 0.15f;
    public float MouseDeltaCap = 8;
    public float SmoothMouseDelta = 0.12f;
    public Transform WeaponHolder;
    public float recoilSpeed = 0.06f;
    public float weaponShakeDamping = 0.15f;
    public float weaponWobbleIntensity = 0.04f;
    public float weaponLowerIntensity = 0.2f;
    public Vector3 DefaultWeaponOffsetPosition = new Vector3(0.5f, -0.5f, 1f);
    public Quaternion DefaultWeaponOffsetRotation = Quaternion.Euler(0f, 0f, 0f);
    public Vector3 DefaultSprintWeaponOffsetPosition = new Vector3(0.5f, -0.8f, 1f);
    public Quaternion DefaultSprintWeaponOffsetRotation = Quaternion.Euler(0f, 45f, 35f);

    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
        CameraFollowHeight = Camera.transform.position.y;
        lastFire = Time.time;
    }

    void Update()
    {
        inputDistance = math.sqrt(Input.GetAxis("Vertical") * Input.GetAxis("Vertical") + Input.GetAxis("Horizontal") * Input.GetAxis("Horizontal"));
        

        //mouse input
        Vector2 MouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        xRotation += Input.GetAxis("Mouse X")*Sensitivity;
        yRotation = math.clamp(yRotation-Input.GetAxis("Mouse Y")*Sensitivity, -75f, 75f);

        float CappedDeltaX = Mathf.Clamp(MouseDelta.x, -MouseDeltaCap,MouseDeltaCap);
        float CappedDeltaY = Mathf.Clamp(MouseDelta.y, -MouseDeltaCap,MouseDeltaCap);
        SmoothDeltaX -= ( SmoothDeltaX - CappedDeltaX ) * (SmoothMouseDelta*(Time.deltaTime*60));
        SmoothDeltaY -= ( SmoothDeltaY - CappedDeltaY ) * (SmoothMouseDelta*(Time.deltaTime*60));

        transform.rotation = Quaternion.Euler(0, xRotation, 0f);

        CameraFollowHeight = CameraFollowHeight - ( CameraFollowHeight - (transform.position.y + 0.6f) ) / CameraFollowSpeed*(Time.deltaTime*60);
        if (Physics.SphereCast(transform.position, 0.2f, -Vector3.up, out RaycastHit hit, (float)(rb.GetComponent<Collider>().bounds.extents.y + 0.1)))
        {
            if (isGrounded == false)
            {
                camShake += new Vector3(5,0,0);
                weaponShake += new Vector3(0,-0.2f,0);
            }
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if ( scroll != 0f )
        {
            if ( scroll > 0 )
            {
                CurrentWeapon ++;
            }
            else
            {
                CurrentWeapon --;
            }
            CurrentWeapon = Mathf.Clamp(CurrentWeapon, 0, Arsenal.Items.Length-1);
            Debug.Log(CurrentWeapon);
        }


        if (useSprintToggle == true)
        {
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                if (sprinting == true)
                {
                    sprinting = false;
                }
                else
                {
                    sprinting = true;
                }
            }
        }
        else
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                sprinting = true;
            }
            else
            {
                sprinting = false;
            }
                
        }
        if (sprinting == true && isGrounded == true && inputDistance == 1)
        {
            sprintingAnimation = Mathf.Clamp(sprintingAnimation + (sprintTransitionSpeed*(Time.deltaTime*60)), 0, 1);
        }
        else
        {
            sprintingAnimation = Mathf.Clamp(sprintingAnimation - (sprintTransitionSpeed*(Time.deltaTime*60)), 0, 1);
        }
        
        if (isGrounded)
        {
            smoothMovementIntensity = Mathf.Lerp(smoothMovementIntensity, Mathf.Clamp(inputDistance, 0f, 1f), Mathf.Clamp(0.15f*(Time.deltaTime*60), 0f, 1f));
            walkWobbleTime += smoothMovementIntensity*(1+SprintSpeedMultiplier*sprintingAnimation)*(Time.deltaTime*60);
            if (Input.GetKeyDown(KeyCode.Space))
            {
                camShake += new Vector3(-5,0,0);
                weaponShake += new Vector3(0,-0.2f,0);
                rb.linearVelocity = new Vector3(rb.linearVelocity.x*JumpMovementBoost, rb.linearVelocity.y+JumpPower, rb.linearVelocity.z*JumpMovementBoost);
            }
        }
        else
        {
            smoothMovementIntensity = Mathf.Lerp(smoothMovementIntensity, 0, Mathf.Clamp(0.15f*(Time.deltaTime*60), 0f, 1f));
        }

        if (Input.GetMouseButton(0))
        {
            if (Time.time - (60/Arsenal.Items[CurrentWeapon].fireRate) > lastFire && firing == false)
            {
                if (Arsenal.Items[CurrentWeapon].fireMode != "full")
                {
                    firing = true;
                }
                lastFire = Time.time;
                Debug.Log("Fire!");
                Recoil += 1f;
            }
        }
        else
        {
            firing = false;
        }
        Recoil -= recoilSpeed*(Time.deltaTime*60);
        if (Recoil < 0)
        {
            Recoil = 0;
        }

        float walkWobbleX = Mathf.Sin(walkWobbleTime * walkWobbleSpeed) * smoothMovementIntensity;
        float walkWobbleY =  Mathf.Sin(walkWobbleTime * walkWobbleSpeed*2) * smoothMovementIntensity;
        float walkWobbleX2 = Mathf.Cos(walkWobbleTime * walkWobbleSpeed) * smoothMovementIntensity;

        Vector3 WeaponOffsetPosition = Vector3.Lerp(Arsenal.Items[CurrentWeapon].WeaponOffsetPosition, Arsenal.Items[CurrentWeapon].SprintWeaponOffsetPosition, sprintingAnimation);
        Quaternion WeaponOffsetRotation = Quaternion.Lerp(Arsenal.Items[CurrentWeapon].WeaponOffsetRotation, Arsenal.Items[CurrentWeapon].SprintWeaponOffsetRotation, sprintingAnimation);

        camShake = Vector3.Lerp(camShake, Vector3.zero, Mathf.Clamp(camShakeDamping*(Time.deltaTime*60), 0f, 1f));
        smoothCamShake = Vector3.Lerp(smoothCamShake, camShake, Mathf.Clamp(camShakeDamping*(Time.deltaTime*60), 0f, 1f));
        weaponShake = Vector3.Lerp(weaponShake, Vector3.zero, Mathf.Clamp(weaponShakeDamping*(Time.deltaTime*60), 0f, 1f));
        smoothWeaponShake = Vector3.Lerp(smoothWeaponShake, weaponShake, Mathf.Clamp(weaponShakeDamping*(Time.deltaTime*60), 0f, 1f));

        Camera.transform.position = new Vector3(Camera.transform.position.x, CameraFollowHeight, Camera.transform.position.z);
        Camera.transform.localRotation = Quaternion.Euler(yRotation + (walkWobbleY*walkWobbleIntensity) + smoothCamShake.x, (walkWobbleX*walkWobbleIntensity) + smoothCamShake.y, smoothCamShake.z);
    
        Vector3 weaponFinalOffsetPosition = new Vector3(walkWobbleX*weaponWobbleIntensity*(1+sprintingAnimation*1.5f),-smoothMovementIntensity*weaponLowerIntensity+Mathf.Abs(-walkWobbleX2)*1.5f*weaponWobbleIntensity*(1+sprintingAnimation*1.5f),0) + smoothWeaponShake;
        WeaponHolder.transform.rotation = Camera.transform.rotation * quaternion.Euler(-SmoothDeltaY*0.5f,0,0);;
        WeaponHolder.transform.position = Camera.transform.position + (WeaponHolder.transform.forward*(WeaponOffsetPosition.z+weaponFinalOffsetPosition.z)) + (WeaponHolder.transform.up*(WeaponOffsetPosition.y+weaponFinalOffsetPosition.y)) + (WeaponHolder.transform.right*(WeaponOffsetPosition.x+weaponFinalOffsetPosition.x));
        WeaponHolder.transform.rotation *= quaternion.Euler(Recoil,SmoothDeltaX*1.5f,0)*WeaponOffsetRotation;
    }

    void FixedUpdate()
    {
            if (isGrounded)
            {
                if (inputDistance > 0)
                {
                    rb.AddForce((transform.forward*Input.GetAxis("Vertical")+transform.right*Input.GetAxis("Horizontal"))/Mathf.Clamp(inputDistance, 1, 1.5f)*WalkSpeed*(1+(SprintSpeedMultiplier*sprintingAnimation)));
                }
                else
                {
                    sprinting = false;
                }
                rb.linearVelocity = new Vector3(rb.linearVelocity.x / (1+(Friction)), rb.linearVelocity.y, rb.linearVelocity.z / (1+(Friction)));
            }
    }

    
}
