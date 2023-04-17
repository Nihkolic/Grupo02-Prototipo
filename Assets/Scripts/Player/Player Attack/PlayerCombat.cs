using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    // Reference to the weapon object to be used for attacking
    public GameObject weapon;
    public Animator animator;

    [SerializeField] private MovementState state;

    public enum MovementState //dashing, atacking
    {
        melee,
        shooting
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Attack();
        }
    }

    void Attack()
    {
        weapon.SetActive(true); // Activate weapon collider for a short period of time
        StartCoroutine(DisableWeaponCollider());
        animator.SetTrigger("Attack");
    }

    IEnumerator DisableWeaponCollider()
    {
        // Wait for a short period of time
        yield return new WaitForSeconds(0.2f);

        // Deactivate weapon collider
        weapon.SetActive(false);
    }
}
