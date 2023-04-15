using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public Renderer rendBody;
    public Material matCurrent;
    public Material matHurt;

    public float enemyHealth = 100f;
    public bool isEnemyDead;

    //public GameObject goEnemyDeath;
    //public Transform enemyDeath;
    private void Start()
    {
        isEnemyDead = false;
    }
    public void DeductHealth(float deductHealth)
    {
        if (!isEnemyDead)
        {
            enemyHealth -= deductHealth;
            rendBody.material = matHurt;
            StartCoroutine(Back());
            //sfx here
        }
        /*
        if (enemyHealth <= 0)
            EnemyDead();
        */
    }
    void EnemyDead()
    {
        if (isEnemyDead == false)
        {
            //Instantiate(goEnemyDeath, enemyDeath.position, Quaternion.identity);
            isEnemyDead = true;
        }
        Destroy(gameObject);
    }

    IEnumerator Back()
    {
        while (Time.timeScale != 1.0f)
            yield return null;//wait for hit stop to end
        rendBody.material = matCurrent;
    }
}
