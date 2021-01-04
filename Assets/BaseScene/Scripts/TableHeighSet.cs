using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableHeighSet : MonoBehaviour
{
    public GameObject table;
    public GameObject handAnchor;
    public bool setHeight;
    public float zOffset;

    private void Start()
    {
        setHeight = false;
        zOffset = 0.61f;
    }

    // Update is called once per frame
    void Update()
    {
        GestureDetector detector = GetComponent<GestureDetector>();
        CurrentHandInfo handInfo = new CurrentHandInfo();

        try
        {
            handInfo = detector.Recognise();
        }
        catch (System.Exception)
        {

            handInfo.leftConfidence = -1;
            handInfo.leftHandGesture = "error";
            handInfo.rightConfidence = -1;
            handInfo.rightHandGesture = "error";
        }
   
        
        if (setHeight == false && handInfo.leftHandGesture == "Left Height Selection")
        {
            float fingerHeight = handAnchor.transform.position.y;
            float fingerZ = handAnchor.transform.position.z;
            table.transform.position = new Vector3(0, fingerHeight - 0.1f, fingerZ + zOffset);
        }

        if (handInfo.leftHandGesture == "Left Height Confirm")
        {
            setHeight = true;
        }
    }
}
