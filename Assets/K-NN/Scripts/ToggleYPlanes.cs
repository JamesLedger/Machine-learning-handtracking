using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleYPlanes : MonoBehaviour
{

    public bool showlines;
    GameObject[] rulerPlanes;

    private void Start()
    {
        showlines = true;
        rulerPlanes = GameObject.FindGameObjectsWithTag("rulerPlane");
    }

    private void OnTriggerEnter(Collider other)
    {
        foreach (GameObject x in rulerPlanes)
        {
            x.SetActive(!showlines);
        }
        showlines = !showlines;
    }
}
