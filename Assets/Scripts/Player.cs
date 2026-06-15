using System;
using System.Collections;
using System.Threading.Tasks;
using TMPro;
using Unity.Mathematics;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.Apple.ReplayKit;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;

public class Player : MonoBehaviour // kut kjelt blijf uit me kanker code
{
    private Rigidbody rb;
    public Camera Camera;
    public Arsenal Arsenal;
    public Inventory Inventory;
    public WaveSystem WaveSystem;
    public GameObject[] BulletImpact;
    public Image healthImage;
    public TextMeshProUGUI WeaponUIText;
    public TextMeshProUGUI WaveText1;
    public TextMeshProUGUI WaveText2;
    public Image reloadBar;
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
    private float smoothRecoil = 0;
    private float smoothRecoil2 = 0;
    private float lastFire;
    private bool firing = false;
    private bool switchingWeapon = false;
    private float switchingAnimation = 0f;
    private int switchTo = 0;
    private float reloadStart = 0f;
    private int reloadID = 0;
    private bool reloading = false;
    private float fireStart = 0;
    private bool fired = false;
    private KeyCode[] keyCodes = {
		KeyCode.Alpha1,
		KeyCode.Alpha2,
		KeyCode.Alpha3,
		KeyCode.Alpha4,
		KeyCode.Alpha5,
		KeyCode.Alpha6,
		KeyCode.Alpha7,
		KeyCode.Alpha8,
		KeyCode.Alpha9,
	};
    

    [Header("Player")]
    public float Health = 100f;
    [Header("Movement")]
    public float Sensitivity = 2f;
    public float WalkSpeed = 5f;
    public float SprintSpeedMultiplier = 1.5f;
    public float JumpPower = 15f;
    public float Friction = 0.5f;
    public float JumpMovementBoost = 1.25f;
    public bool useSprintToggle = true;
    public float sprintTransitionSpeed = 0.1f;
    [Header("Camera")]
    public float CameraFollowSpeed = 0.75f;
    public float walkWobbleSpeed = 25f;
    public float walkWobbleIntensity = 5f;
    public float camShakeDamping = 0.15f;
    public float MouseDeltaCap = 8;
    public float SmoothMouseDelta = 0.12f;
    [Header("Weapons")]
    public Transform WeaponHolder;
    public GameObject WeaponPrefab;
    public float weaponSwitchSpeed = 0.05f;
    public float recoilSpeed = 0.06f;
    public float weaponFireRaycastBackwardsOffset = 1;
    public float weaponShakeDamping = 0.15f;
    public float weaponWobbleIntensity = 0.04f;
    public float weaponLowerIntensity = 0.2f;
    public Vector3 DefaultWeaponOffsetPosition = new Vector3(0.5f, -0.5f, 1f);
    public Quaternion DefaultWeaponOffsetRotation = Quaternion.Euler(0f, 0f, 0f);
    public Vector3 DefaultSprintWeaponOffsetPosition = new Vector3(0.5f, -0.8f, 1f);
    public Quaternion DefaultSprintWeaponOffsetRotation = Quaternion.Euler(0f, 45f, 35f);


    void Reload()
    {
        if (Inventory.Items[CurrentWeapon].Ammo < Arsenal.Items[Inventory.Items[CurrentWeapon].itemNumber].magSize && reloading == false)
        {
            int missingAmmo = Arsenal.Items[Inventory.Items[CurrentWeapon].itemNumber].magSize- Inventory.Items[CurrentWeapon].Ammo;
            if (missingAmmo > Inventory.ReserveAmmo[Arsenal.Items[Inventory.Items[CurrentWeapon].itemNumber].ammoType].Ammo)
            {
                missingAmmo = Inventory.ReserveAmmo[Arsenal.Items[Inventory.Items[CurrentWeapon].itemNumber].ammoType].Ammo;
            }
            if (missingAmmo > 0)
            {
                reloading = true;
                reloadStart = Time.time;

                reloadID += 1;
                int currentReloadID = reloadID;

                StartCoroutine(ReloadFinish(currentReloadID, missingAmmo));

            }
        }
    }
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
        CameraFollowHeight = Camera.transform.position.y;
        lastFire = Time.time;

        if (Arsenal.Items[Inventory.Items[CurrentWeapon].itemNumber].Prefab != null)
        {
            WeaponPrefab = Instantiate(Arsenal.Items[Inventory.Items[CurrentWeapon].itemNumber].Prefab, WeaponHolder.transform.position, WeaponHolder.transform.rotation);
            WeaponPrefab.transform.SetParent(WeaponHolder.transform);
        }
    }

    void Update()
    {
//player input, mouse input
        inputDistance = math.sqrt(Input.GetAxis("Vertical") * Input.GetAxis("Vertical") + Input.GetAxis("Horizontal") * Input.GetAxis("Horizontal"));
        

        Vector2 MouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        xRotation += Input.GetAxis("Mouse X")*Sensitivity;
        yRotation = math.clamp(yRotation-Input.GetAxis("Mouse Y")*Sensitivity, -75f, 75f);

        float CappedDeltaX = Mathf.Clamp(MouseDelta.x, -MouseDeltaCap,MouseDeltaCap);
        float CappedDeltaY = Mathf.Clamp(MouseDelta.y, -MouseDeltaCap,MouseDeltaCap);
        SmoothDeltaX -= ( SmoothDeltaX - CappedDeltaX ) * (SmoothMouseDelta*(Time.deltaTime*60));
        SmoothDeltaY -= ( SmoothDeltaY - CappedDeltaY ) * (SmoothMouseDelta*(Time.deltaTime*60));

        transform.rotation = Quaternion.Euler(0, xRotation, 0f);

        CameraFollowHeight = CameraFollowHeight - ( CameraFollowHeight - (transform.position.y + 0.6f) ) / CameraFollowSpeed*(Time.deltaTime*60); // smooth camera
        if (Physics.SphereCast(transform.position, 0.2f, -Vector3.up, out RaycastHit hit, (float)(rb.GetComponent<Collider>().bounds.extents.y + 0.1)))
        {
            //player land effect
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


//weapon switching
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if ( scroll != 0f )
        {
            if ( scroll > 0 )
            {
                switchTo ++;
            }
            else
            {
                switchTo --;
            }
            int clamped = Mathf.Clamp(switchTo, 0, Inventory.Items.Length-1);
            if (switchTo == clamped)
            {
                switchingWeapon = true;
            }
            switchTo = clamped;
        }
        //switch using keyboard
        for(int i = 0 ; i < keyCodes.Length; i ++ )
        {
            if(Input.GetKeyDown(keyCodes[i]))
            {
                switchTo = i;
                int clamped = Mathf.Clamp(switchTo, 0, Inventory.Items.Length-1);
                if (switchTo == clamped)
                {
                    switchingWeapon = true;
                }
                switchTo = clamped;
            }
        
        }

        if (switchingWeapon == true)
        {
            if (switchTo == CurrentWeapon)
            {
                switchingWeapon = false;
            }
            else
            {
                switchingAnimation = Mathf.Clamp(switchingAnimation+weaponSwitchSpeed*(Time.deltaTime*60),  0, 1);
                if (switchingAnimation == 1)
                {
                    switchingWeapon = false;
                    CurrentWeapon = switchTo;
                    firing = false;
                    
                    if (WeaponPrefab != null)
                    {
                        Destroy(WeaponPrefab);
                    }
                    if (Arsenal.Items[Inventory.Items[CurrentWeapon].itemNumber].Prefab != null)
                    {
                        WeaponPrefab = Instantiate(Arsenal.Items[Inventory.Items[CurrentWeapon].itemNumber].Prefab, WeaponHolder.transform.position, WeaponHolder.transform.rotation);
                        WeaponPrefab.transform.SetParent(WeaponHolder.transform);
                    }

                    if (reloading == true)
                    {
                        reloading = false;
                        reloadID += 1;
                    }
                    Debug.Log(CurrentWeapon);
                }
            }
        }
        else
        {
            switchingAnimation = Mathf.Clamp(switchingAnimation-weaponSwitchSpeed*(Time.deltaTime*60),  0, 1);
        }

        

//sprint on/off
        if (Input.GetMouseButton(0) == false)
        {
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
        }
        if (sprinting == true && isGrounded == true && inputDistance > .95)
        {
            sprintingAnimation = Mathf.Clamp(sprintingAnimation + (sprintTransitionSpeed*(Time.deltaTime*60)), 0, 1);
        }
        else
        {
            sprintingAnimation = Mathf.Clamp(sprintingAnimation - (sprintTransitionSpeed*(Time.deltaTime*60)), 0, 1);
        }
        

//jumping
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

//reload
        if (Input.GetKeyDown(KeyCode.R))
        {
            Reload();
        }
        if (Inventory.Items[CurrentWeapon].Ammo == 0 && Arsenal.Items[Inventory.Items[CurrentWeapon].itemNumber].magSize > 0 && Inventory.ReserveAmmo[Arsenal.Items[Inventory.Items[CurrentWeapon].itemNumber].ammoType].Ammo > 0 && Time.time - 0.2 > lastFire)
        {
            Reload();
        }


//recoil values
        Recoil -= recoilSpeed*(Time.deltaTime*60);
        if (Recoil < 0)
        {
            Recoil = 0;
        }
        smoothRecoil = smoothRecoil - ( smoothRecoil - Recoil ) * 0.75f*(Time.deltaTime*60);
        //second recoil to make it look better
        smoothRecoil2 = smoothRecoil2 - ( smoothRecoil2 - Recoil ) * 0.225f*(Time.deltaTime*60);

//UI
        healthImage.fillAmount = Health / 100f;
        if (Arsenal.Items[Inventory.Items[CurrentWeapon].itemNumber].magSize > 0)
        {
            WeaponUIText.text = Arsenal.Items[Inventory.Items[CurrentWeapon].itemNumber].name + " - " + Inventory.Items[CurrentWeapon].Ammo + " / " + Inventory.ReserveAmmo[Arsenal.Items[Inventory.Items[CurrentWeapon].itemNumber].ammoType].Ammo;
        }
        else
        {
            WeaponUIText.text = Arsenal.Items[Inventory.Items[CurrentWeapon].itemNumber].name;
        }
        if (reloading == true)
        {
            reloadBar.fillAmount = (Time.time-reloadStart)/Arsenal.Items[Inventory.Items[CurrentWeapon].itemNumber].reloadDuration;
        }
        else
        {
            reloadBar.fillAmount = 0;
        }
        WaveText1.text = "Wave " + WaveSystem.currentWave.ToString();
        if (WaveSystem.levelEnding == true)
        {
            WaveText2.text = "Level done";
        }
        else
        {
            if (WaveSystem.timeBetweenWaves - (Time.time-WaveSystem.waveStartTime) < 0)
            {
                WaveText2.text = WaveSystem.currentAliveEnemies.ToString() + " Enemies left";
            }
            else
            {
                WaveText2.text = math.ceil(WaveSystem.timeBetweenWaves - (Time.time-WaveSystem.waveStartTime)).ToString() + " Seconds";
            }
        }


//setting weapon and camera position
        float walkWobbleX = Mathf.Sin(walkWobbleTime * walkWobbleSpeed) * smoothMovementIntensity;
        float walkWobbleY =  Mathf.Sin(walkWobbleTime * walkWobbleSpeed*2) * smoothMovementIntensity;
        float walkWobbleX2 = Mathf.Cos(walkWobbleTime * walkWobbleSpeed) * smoothMovementIntensity;

        Vector3 WeaponOffsetPosition = Vector3.Lerp(Arsenal.Items[Inventory.Items[CurrentWeapon].itemNumber].WeaponOffsetPosition, Arsenal.Items[Inventory.Items[CurrentWeapon].itemNumber].SprintWeaponOffsetPosition, sprintingAnimation);
        Quaternion WeaponOffsetRotation = Quaternion.Lerp(Arsenal.Items[Inventory.Items[CurrentWeapon].itemNumber].WeaponOffsetRotation, Arsenal.Items[Inventory.Items[CurrentWeapon].itemNumber].SprintWeaponOffsetRotation, sprintingAnimation);

        camShake = Vector3.Lerp(camShake, Vector3.zero, Mathf.Clamp(camShakeDamping*(Time.deltaTime*60), 0f, 1f));
        smoothCamShake = Vector3.Lerp(smoothCamShake, camShake, Mathf.Clamp(camShakeDamping*(Time.deltaTime*60), 0f, 1f));
        weaponShake = Vector3.Lerp(weaponShake, Vector3.zero, Mathf.Clamp(weaponShakeDamping*(Time.deltaTime*60), 0f, 1f));
        smoothWeaponShake = Vector3.Lerp(smoothWeaponShake, weaponShake, Mathf.Clamp(weaponShakeDamping*(Time.deltaTime*60), 0f, 1f));

        Camera.transform.position = new Vector3(Camera.transform.position.x, CameraFollowHeight, Camera.transform.position.z);
        Camera.transform.localRotation = Quaternion.Euler(yRotation + (walkWobbleY*walkWobbleIntensity) + smoothCamShake.x - smoothRecoil*3f+smoothRecoil2*1.5f,    (walkWobbleX*walkWobbleIntensity) + smoothCamShake.y, smoothCamShake.z);
        
        //weapon position
        Vector3 weaponFinalOffsetPosition = new Vector3(walkWobbleX*weaponWobbleIntensity*(1+sprintingAnimation*1.5f) - switchingAnimation*.75f + Mathf.Sin(Time.time*10)*smoothRecoil2*0.15f,    -smoothMovementIntensity*weaponLowerIntensity+Mathf.Abs(-walkWobbleX2)*1.5f*weaponWobbleIntensity*(1+sprintingAnimation*1.5f) +smoothRecoil*0.075f - switchingAnimation*1.5f,   -smoothRecoil*0.35f - smoothRecoil2*0.5f) + smoothWeaponShake;
        WeaponHolder.transform.rotation = Camera.transform.rotation * quaternion.Euler(-SmoothDeltaY*0.5f,0,0);;
        WeaponHolder.transform.position = Camera.transform.position + (WeaponHolder.transform.forward*(WeaponOffsetPosition.z+weaponFinalOffsetPosition.z)) + (WeaponHolder.transform.up*(WeaponOffsetPosition.y+weaponFinalOffsetPosition.y)) + (WeaponHolder.transform.right*(WeaponOffsetPosition.x+weaponFinalOffsetPosition.x));
        WeaponHolder.transform.rotation *= WeaponOffsetRotation;
        WeaponHolder.transform.rotation *= quaternion.Euler(-smoothRecoil*0.825f + smoothRecoil2*0.4f, SmoothDeltaX*1.5f - switchingAnimation*2f, 0);
    }

    IEnumerator ReloadFinish(int currentReloadID, int missingAmmo)
    {
        // simple reload delay (in seconds)
        float reloadTime = Arsenal.Items[Inventory.Items[CurrentWeapon].itemNumber].reloadDuration;
        yield return new WaitForSeconds(reloadTime);
        if (currentReloadID == reloadID && reloading == true)
        {
            Inventory.Items[CurrentWeapon].Ammo += missingAmmo;
            Inventory.ReserveAmmo[Arsenal.Items[Inventory.Items[CurrentWeapon].itemNumber].ammoType].Ammo -= missingAmmo;
            reloading = false;
        }
    }

    void FixedUpdate()
    {
//weapon fire
        if (Input.GetMouseButton(0))
        {
            bool canFire = false;
            if (Inventory.Items[CurrentWeapon].Ammo > 0 || Arsenal.Items[Inventory.Items[CurrentWeapon].itemNumber].weaponType == "melee")
            {
                canFire = true;
            }
            sprinting = false;
            if (Time.time - (60/Arsenal.Items[Inventory.Items[CurrentWeapon].itemNumber].fireRate) > lastFire && firing == false && sprintingAnimation < 0.5f && switchingAnimation == 0 && canFire == true)
            {
                if (reloading == true)
                {
                    reloading = false;
                    reloadID += 1;
                }
                

                if (Arsenal.Items[Inventory.Items[CurrentWeapon].itemNumber].fireMode != "auto")
                {
                    firing = true;
                }
                lastFire = Time.time;

                fireStart = Time.time;
                fired = false;
                

                if (WeaponPrefab != null)
                {
                    Animator animator = WeaponPrefab.GetComponent<Animator>();
                    Debug.Log(animator);
                    if (animator != null)
                    {
                        animator.SetTrigger("Fire");
                    }
                }

            }
        }
        else
        {
            firing = false;
        }

        if (Time.time - Arsenal.Items[Inventory.Items[CurrentWeapon].itemNumber].fireDelay > fireStart && fired == false)
        {
            fired = true;
            if (Physics.Raycast(Camera.transform.position + (Camera.transform.TransformDirection(Vector3.forward)*weaponFireRaycastBackwardsOffset), Camera.transform.TransformDirection(Vector3.forward), out RaycastHit hit, Arsenal.Items[Inventory.Items[CurrentWeapon].itemNumber].range-weaponFireRaycastBackwardsOffset, ~LayerMask.GetMask("Player")))
            {
                bool createImpactEffect = true;
                bool parentToPart = true;
                int impactType = 0;
                if (hit.transform.gameObject.tag == "Enemy")
                {
                    impactType = 1;
                    Vector3 dist = hit.transform.position-transform.position;
                    dist = new Vector3(dist.x,0,dist.z);
                    hit.transform.GetComponent<Rigidbody>().linearVelocity += (dist.normalized + new Vector3(0,1,0))*Arsenal.Items[Inventory.Items[CurrentWeapon].itemNumber].damageKnockback;
                    hit.transform.gameObject.GetComponent<EnemyMovement>().health -= Arsenal.Items[Inventory.Items[CurrentWeapon].itemNumber].damage;
                    if (hit.transform.gameObject.GetComponent<EnemyMovement>().health < 0)
                    {
                        parentToPart = false;
                        if ((float)UnityEngine.Random.Range(0,100)/100 < hit.transform.gameObject.GetComponent<EnemyMovement>().itemDropChance)
                        {
                            if (hit.transform.gameObject.GetComponent<EnemyMovement>().itemDrops.Length > 0)
                            {
                                GameObject itemDrop = hit.transform.gameObject.GetComponent<EnemyMovement>().itemDrops[UnityEngine.Random.Range(0, hit.transform.gameObject.GetComponent<EnemyMovement>().itemDrops.Length)];
                                
                                GameObject newItem = Instantiate(
                                    itemDrop,
                                    hit.transform.gameObject.transform.position,
                                    Quaternion.identity
                                );
                                newItem.GetComponent<Rigidbody>().AddForce(0,70,0);
                            }
                        }
                        GameObject deathEffect = Instantiate(hit.transform.gameObject.GetComponent<EnemyMovement>().deathParticles, hit.transform.position, Quaternion.LookRotation(hit.normal));
                        ParticleSystem ps = deathEffect.GetComponent<ParticleSystem>();
                        ps.Emit(15);
                        

                        WaveSystem.currentAliveEnemies -= 1;
                        Destroy(hit.transform.gameObject);
                        createImpactEffect = false;
                    }
                }
                
                //hit.transform.
                if (createImpactEffect == true)
                {
                    GameObject impact = Instantiate(BulletImpact[impactType], hit.point, Quaternion.LookRotation(hit.normal));
                    if (parentToPart == true)
                    {
                        impact.transform.SetParent(hit.transform);
                    }
                    ParticleSystem ps = impact.GetComponent<ParticleSystem>();
                    ps.Emit(5);
                    impact.transform.Find("Hole").rotation *= Quaternion.Euler(0, 0, UnityEngine.Random.Range(0f, 360f));
                }
            }
            if (Arsenal.Items[Inventory.Items[CurrentWeapon].itemNumber].weaponType != "melee")
            {
                Recoil = (Recoil*0.75f) + Arsenal.Items[Inventory.Items[CurrentWeapon].itemNumber].recoil;
                Inventory.Items[CurrentWeapon].Ammo -= 1;
            }
        }


//player physics
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