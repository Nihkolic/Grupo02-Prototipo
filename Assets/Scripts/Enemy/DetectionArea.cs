using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectionArea : MonoBehaviour
{

    [SerializeField] private LayerMask playerLayerMask;
    bool detected;
    GameObject target;
    public Transform enemy;
    public GameObject bullet;
    public Transform shootPoint;
    public float shootSpeed = 10f;
    public float timeToShoot = 1.3f;
    float originalTime;
    private bool isShooting = false;
    [SerializeField] private float distance;


    void Start()
    {
        target = GameObject.Find("Player");
        originalTime = timeToShoot;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, distance);
    }
    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(transform.position,target.transform.position) < distance)
        {
            detected = true;
        }
        else
        {
            detected = false;
        }
        if (detected)
        {
            enemy.LookAt(target.transform);
        }
    }

    private void FixedUpdate()
    { 
        if (detected)
        {
            timeToShoot -= Time.deltaTime;

            if (timeToShoot < 0)
            {
                //InvokeRepeating("ShootPlayer", 0.0f, 5.0f);
                StartCoroutine(ShootPlayer());
                timeToShoot = originalTime;
            }
        }
    }

    /*private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            detected = true;
            target = other.gameObject;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            detected = false;
        }
    }*/

    private IEnumerator ShootPlayer()
    {
        for (int i = 1; i <=3; i++)
        {
            GameObject currentBullet = Instantiate(bullet, shootPoint.position, shootPoint.rotation);
            Rigidbody rig = currentBullet.GetComponent<Rigidbody>();

            rig.AddForce(transform.forward * shootSpeed, ForceMode.VelocityChange);
            yield return new WaitForSeconds(0.25f);

        } 

           
    }

    private void DeacivatedTurret()
    {
        isShooting = false;
    }
}
