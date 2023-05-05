using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DetectionArea : MonoBehaviour
{

    [SerializeField] private LayerMask playerLayerMask;
    [SerializeField] private float distance;
    bool detected;
    GameObject target;
    public Transform enemy;
    public GameObject bullet;
    public Transform shootPoint;
    public float shootSpeed = 10f;
    public float timeToShoot = 1.3f;
    float originalTime;
    private bool isShooting = false;
    private bool enableShooting = true;

    [SerializeField] private List<Transform> points;
    [SerializeField] private NavMeshAgent IA2;
    [SerializeField] private float distance2;
    private bool running = false;
    


    void Start()
    {
        target = GameObject.Find("Player");
        originalTime = timeToShoot;

        StartCoroutine(RunningLogic());
    }

    private IEnumerator RunningLogic()
    {
        while (true)
        {
            while(Vector3.Distance(transform.position, target.transform.position) > distance2)
            {
                yield return null;
            }
            Vector3 position = points[Random.Range(0, points.Count)].position;
            IA2.SetDestination(position);
            IA2.isStopped = false;
            enableShooting = false;
            while (Vector3.Distance(transform.position, position) > 1)
            {
                yield return null;
            }
            enableShooting = true;
            IA2.isStopped = true;
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, distance);
        Gizmos.DrawWireSphere(transform.position, distance2);
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
            if (enableShooting)
            {
                GameObject currentBullet = Instantiate(bullet, shootPoint.position, shootPoint.rotation);
                Rigidbody rig = currentBullet.GetComponent<Rigidbody>();

                rig.AddForce(transform.forward * shootSpeed, ForceMode.VelocityChange);
                yield return new WaitForSeconds(0.25f);
            }

        } 

           
    }

    private void DeacivatedTurret()
    {
        isShooting = false;
    }
}
