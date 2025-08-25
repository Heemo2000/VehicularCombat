using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

public class WeaponMachine : MonoBehaviour
{
    [Header("References")]
    [SerializeField] CarController carController;
    [SerializeField] Transform bulletPoint;
    [SerializeField] Transform machineGun;
    [SerializeField] GameObject bulletPrefab;

    [Header("AssualtGun Settings")]
  
    [SerializeField] float bulletspeed;
    public float fireRate = 0.25f;   
    private float nextFireTime = 0f;

    [SerializeField] int bulletCapacity = 21;
    [SerializeField] int bulletsPerBurst = 3;
    public float burstDelay = 0.1f;

    

    void Update()
    {
        if(carController.IsMobile)
        {
            if (Input.GetButtonDown("Fire1") && Time.time >= nextFireTime && bulletCapacity > 0)
            {
                StartCoroutine(BurstFire());
                nextFireTime = Time.time + fireRate; 
            }

        }
        
    }

    IEnumerator BurstFire()
    {
        for (int i = 0; i < bulletsPerBurst; i++)
        {
            Shoot();
            yield return new WaitForSeconds(burstDelay); // small delay between each bullet
        }
    }

    void Shoot()
    {
        GameObject bullet = Instantiate(bulletPrefab, bulletPoint.position, bulletPoint.rotation);
        bullet.GetComponent<Rigidbody>().linearVelocity = bulletPoint.forward * bulletspeed;
        bulletCapacity--;
        Destroy(bullet,1);
        


    }

    public void MobileShoot()
    {
        if (Time.time >= nextFireTime && bulletCapacity > 0 )
        {
            StartCoroutine(BurstFire());
            nextFireTime = Time.time + fireRate; // cooldown between bursts
        }
    }


}
