using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatePoint : MonoBehaviour
{
    public GameObject SpacePoint;
    public GameObject Origin;

    public List<IrisData> updatingPoints;

    public Knn dataStore;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            GameObject temp = Instantiate(SpacePoint);
            temp.transform.position = Origin.transform.position + new Vector3(-2f, 2f, 2f);

        }
    }
}
