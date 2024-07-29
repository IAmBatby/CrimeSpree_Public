using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cargo : MonoBehaviour
{
    public Randomiser randomiser;
    public Animator animator;

    public void SpawnLoot()
    {
        GameObject randomLoot = randomiser.Randomise();
        GameObject loot = Instantiate(randomLoot, transform.position, randomLoot.transform.rotation);
        loot.transform.position = randomiser.transform.position;
        loot.transform.SetParent(transform);
        animator.SetTrigger("Open");
        Debug.Log("D");
    }
}
