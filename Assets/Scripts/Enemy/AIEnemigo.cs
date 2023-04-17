using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIEnemigo : MonoBehaviour
{
    public Transform Target;
    public float Velocidad;
    public NavMeshAgent IA;
    [SerializeField] private float distance;
    [SerializeField] private float waitingTime;
    [SerializeField] private LayerMask playerLayerMask;

    private void Start()
    {
        IA.speed = Velocidad;
        StartCoroutine(Logic());
    }
    void Update()
    {        
        
    }

    IEnumerator Logic()
    {
        while (true)
        {
            RaycastHit hit;
            
            while (Vector3.Distance(Target.position, transform.position) > distance)
            {
                IA.SetDestination(Target.position);
                yield return null;

            }
            Physics.Raycast(transform.position, Target.position - transform.position, out hit, Vector3.Distance(transform.position, Target.position), playerLayerMask);
            Debug.Log("aqui");
            //IA.isStopped = true;
            IA.enabled = false;
            Rigidbody rigidbody =  gameObject.AddComponent<Rigidbody>();
            rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            yield return new WaitForSeconds(waitingTime);
            rigidbody.AddForce((Target.position - transform.position).normalized * 20, ForceMode.Impulse);
            yield return new WaitForSeconds(waitingTime/2);
            Destroy(rigidbody);
            //IA.isStopped = false;
            IA.enabled = true;
            yield return new WaitForEndOfFrame();
                /*
            IA.speed *= 5;
            IA.isStopped = false;
            Vector3 targetPosition = hit. point;
            IA.SetDestination(targetPosition);
            IA.stoppingDistance = 0.5f;
            yield return null;
            while (Vector2.Distance(
                new Vector2(targetPosition.x, targetPosition.z),
                new Vector2(transform.position.x, transform.position.z)
                ) > 0.1f)
            {
                Debug.Log(Vector2.Distance(
                new Vector2(targetPosition.x, targetPosition.z),
                new Vector2(transform.position.x, transform.position.z)
                ));
                yield return null;
            }
            Debug.Log("Aca");
            IA.speed /= 5;
            IA.stoppingDistance = 1;*/
        }
       
    }
}
