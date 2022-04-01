using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField, Range(1, 100)] float UserSpeed = 12f;
    [SerializeField, Range(0, 2)] float shootCooldown = .7f;

    public int damage = 2;
    public int magCapacity = 6;
    public int totalAmmo = 36;
    public int bulletCount;
    public bool isGrounded;
    public bool canShoot;
    public float gravity = -9.81f;
    public float groundDistance = 0.4f;
    public float reloadTime = 3f;

    //public Transform firePoint;
    public Transform groundCheck;
    //private CameraShake cameraShake;
    public GameObject cam;
    //public GameObject reloadText;
    public CharacterController controller;

    public LayerMask groundMask;
    //public LayerMask enemyMask;

    private float speed;

    private Vector3 velocity;

    private void Start()
    {
        speed = UserSpeed;
        //cameraShake = cam.GetComponent<CameraShake>();
        //cameraShake.enabled = false;
        bulletCount = magCapacity;
    }

    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        if (Input.GetKey(KeyCode.LeftShift))
        {
            speed = UserSpeed / 2;
            //While walking
            //No footstep audio
        }

        else if (Input.GetKey(KeyCode.LeftControl))
        {
            controller.height = 1.9f;
            speed = UserSpeed / 3;
        }

        else if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            speed = UserSpeed;
            controller.height = 3.8f;
            //While running
            //Footstep audio on
        }
        //else
        //{ 
        //    controller.height = 3.8f;
        //}

        // if (Input.GetKeyDown(KeyCode.Mouse0) && canShoot && bulletCount > 0)
        // {
        //     Shoot();
        //     //cameraShake.enabled = true;
        //     StartCoroutine(ShootCooldwon());
        //     //cameraShake.enabled = false;
        // }
        //
        // if (((bulletCount == 0 && Input.GetKeyDown(KeyCode.Mouse0)) || Input.GetKeyDown(KeyCode.R)) && totalAmmo != 0 && bulletCount != magCapacity)
        // {
        //     StartCoroutine(ReloadTime());
        // }

        Vector3 move = transform.right * x + transform.forward * z;

        controller.Move(move * speed * Time.deltaTime);

        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
    }
    
    // private void Shoot()
    // {
    //     Debug.DrawRay(firePoint.position, firePoint.forward * 100, Color.red, .5f);
    //     bulletCount -= 1;
    //     Ray ray = new Ray(firePoint.position, firePoint.forward);
    //     RaycastHit hitInfo;
    //
    //     if (Physics.Raycast(ray, out hitInfo, 100))
    //     {
    //         var health = hitInfo.collider.GetComponent<Health>();
    //         if (health != null)
    //         {
    //             health.TakeDamage(damage);
    //         }
    //     }
    //
    // }

    // void Reload()
    // {
    //     Debug.Log("Reloading");
    //     for (int i = bulletCount; i<magCapacity; i++)
    //     {
    //         if (totalAmmo != 0)
    //         {
    //             totalAmmo -= 1;
    //             bulletCount += 1;
    //         }
    //     }
    // }
    //
    // IEnumerator ReloadTime()
    // {
    //     reloadText.SetActive(true);
    //     canShoot = false;
    //     yield return new WaitForSeconds(reloadTime);
    //     Reload();
    //     reloadText.SetActive(false);
    //     canShoot = true;
    // }
    // IEnumerator ShootCooldwon()
    // {
    //     canShoot = false;
    //     cameraShake.enabled = true;
    //     yield return new WaitForSeconds(shootCooldown);
    //     cameraShake.enabled = false;
    //     canShoot = true;
    // }
}
