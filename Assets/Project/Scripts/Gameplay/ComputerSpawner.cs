using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComputerSpawner : MonoBehaviour
{
    public Randomiser randomiser;
    public GameObject parent;

    void Start()
    {
        GameObject computer = randomiser.Randomise();
        GameObject computerObject = Instantiate(computer, transform.position, transform.rotation);
        computerObject.transform.SetParent(parent.transform);
        //GameManager.Instance.BatchObjects(computerObject);
        Destroy(this);
    }
}
