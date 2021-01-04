using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlterPointSize : MonoBehaviour
{
    public float changefactor;
    GameObject[] dataPoints;

    private void OnTriggerEnter(Collider other)
    {
        dataPoints = GameObject.FindGameObjectsWithTag("DataPoint");

        Debug.Log(dataPoints.Length.ToString());

        foreach (GameObject x in dataPoints)
        {
            x.transform.localScale += new Vector3(changefactor, changefactor, changefactor);
        }
    }
}
