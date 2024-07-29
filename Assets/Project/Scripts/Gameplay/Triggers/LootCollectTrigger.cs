using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootCollectTrigger : MonoBehaviour
{
    GameManager gameManager;

    void Start()
    {
        gameManager = GameManager.Instance;
    }

    void OnTriggerEnter(Collider other)
    {
    }
}
