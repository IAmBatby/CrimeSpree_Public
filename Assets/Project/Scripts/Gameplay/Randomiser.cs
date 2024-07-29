using AYellowpaper.SerializedCollections;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Randomiser : SerializedMonoBehaviour
{
    public SerializedDictionary<GameObject, int> randomiserDictionary;
    public List<GameObject> randomiserList = new List<GameObject>();

    public UnityEvent onEmptied;

    public bool onAwake;

    public bool removeOnRandomize;

    void Start()
    {
        foreach (KeyValuePair<GameObject, int> item in randomiserDictionary)
        {
            for (int i = 0; i < item.Value; i++)
            {
                randomiserList.Add(item.Key);
            }
        }

        if (onAwake)
        {
            Randomise();
        }
    }

    public GameObject Randomise(bool removeOnRandomizeParameter = false)
    {
        GameObject randomizedObject = null;
        if(randomiserList.Count > 0)
        {
            Debug.Log("Randomising!");
            randomizedObject = randomiserList[Random.Range(0, randomiserList.Count)];
            if (removeOnRandomize == true || removeOnRandomizeParameter == true)
                randomiserList.Remove(randomizedObject);
            if (randomiserList.Count == 0)
                onEmptied.Invoke();
            return randomizedObject;
        }
        else
        {
            Debug.Log("Randomiser Error! List Not Populated!");
            return null;
        }
    }
}
