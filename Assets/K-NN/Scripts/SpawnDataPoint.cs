using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnDataPoint : MonoBehaviour
{
    public GameObject SpacePoint;
    public Material UnknownMat;
    public GameObject Button;

    public Knn KNNmodel;

    private void OnTriggerEnter(Collider other)
    {
        GameObject temp = Instantiate(SpacePoint);
        temp.transform.position = Button.transform.position + new Vector3(0, 0.2f, 0);
        temp.GetComponent<MeshRenderer>().material = UnknownMat;
    }
}
