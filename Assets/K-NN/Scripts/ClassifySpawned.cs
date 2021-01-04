using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClassifySpawned : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        GameObject[] gos;
        gos = GameObject.FindGameObjectsWithTag("DataPoint");

        if (gos != null)
        {
            GameObject closest = null;
            float distance = Mathf.Infinity;
            Vector3 position = transform.position;
            foreach (GameObject go in gos)
            {
                Vector3 diff = go.transform.position - position;
                float curDistance = diff.sqrMagnitude;
                if (curDistance < distance)
                {
                    closest = go;
                    distance = curDistance;
                }
            }

            if (closest != null)
            {
                Material target = closest.GetComponent<Material>();
                gameObject.GetComponent<MeshRenderer>().material = target;
            }
         
        }
    }
}
