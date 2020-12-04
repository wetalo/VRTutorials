using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    public List<GameObject> objects;

    public void Spawn(string objectName)
    {
        Debug.Log("Spawn: " + objectName);
        foreach (var item in objects)
        {
            item.SetActive(objectName == item.name);
        }
    }
}
