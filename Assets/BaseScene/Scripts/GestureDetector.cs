using Oculus.Platform.Models;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class GestureDetector : MonoBehaviour
{
    [System.Serializable]
    public struct Gesture
    {
        public string name;
        public List<Vector3> fingerData;
        public UnityEvent onRecognised;
    }

    public string Name;
    public float threshhold = 0.1f;
    public OVRSkeleton leftSkeleton;
    public OVRSkeleton rightSkeleton;

    public List<OVRBone> leftFingerBones;
    public List<OVRBone> rightfingerBones;

    public List<Gesture> leftGestures;
    public List<Gesture> rightGestures;

    public bool debugMode = true;
    public CurrentHandInfo currentGestureInfo;
    public CurrentHandInfo previousGestureInfo;

    // Start is called before the first frame update
    void Start()
    {
        leftFingerBones = new List<OVRBone>(leftSkeleton.Bones);
        rightfingerBones = new List<OVRBone>(rightSkeleton.Bones);

        Debug.Log("Started tracking");
    }

    // Update is called once per frame
    void Update()
    {
        leftFingerBones = new List<OVRBone>(leftSkeleton.Bones);
        rightfingerBones = new List<OVRBone>(rightSkeleton.Bones);

        if (debugMode && Input.GetKeyDown(KeyCode.LeftArrow))
        {
            Save(true);
            Debug.Log("starting");
        }

        if (debugMode && Input.GetKeyDown(KeyCode.RightArrow))
        {
            Save(false);
            Debug.Log("starting");
        }

    }

    void Save(bool leftGesture)
    {
        Gesture g = new Gesture();
        g.name = "New Gesture";
        List<Vector3> data = new List<Vector3>();

        if (leftGesture)
        {
            foreach (var bone in leftFingerBones)
            {
                data.Add(leftSkeleton.transform.InverseTransformPoint(bone.Transform.position));
            }
            g.fingerData = data;
            leftGestures.Add(g);
        }
        else
        {
            foreach (var bone in rightfingerBones)
            {
                data.Add(rightSkeleton.transform.InverseTransformPoint(bone.Transform.position));
            }
            g.fingerData = data;
            rightGestures.Add(g);
        }
    }

    public CurrentHandInfo Recognise()
    {
        CurrentHandInfo outputInfo = new CurrentHandInfo();

        float currentMin = Mathf.Infinity;
        foreach (var gesture in leftGestures)
        {
            float sumDistance = 0;
            bool isDiscarded = false;
            for (int i = 0; i < leftFingerBones.Count; i++)
            {
                Vector3 currentData = leftSkeleton.transform.InverseTransformPoint(leftFingerBones[i].Transform.position);
                float distance = Vector3.Distance(currentData, gesture.fingerData[i]);
                if (distance > threshhold)
                {
                    isDiscarded = true;
                    break;
                }

                sumDistance += distance;
            }
            if (!isDiscarded && sumDistance < currentMin)
            {
                currentMin = sumDistance;

                outputInfo.leftHandGesture = gesture.name;
                outputInfo.leftConfidence = 1 - sumDistance;
            }
        }

        currentMin = Mathf.Infinity;
        foreach (var gesture in rightGestures)
        {
            float sumDistance = 0;
            bool isDiscarded = false;
            for (int i = 0; i < rightfingerBones.Count; i++)
            {
                Vector3 currentData = rightSkeleton.transform.InverseTransformPoint(rightfingerBones[i].Transform.position);
                float distance = Vector3.Distance(currentData, gesture.fingerData[i]);
                if (distance > threshhold)
                {
                    isDiscarded = true;
                    break;
                }

                sumDistance += distance;
            }
            if (!isDiscarded && sumDistance < currentMin)
            {
                currentMin = sumDistance;

                outputInfo.rightHandGesture = gesture.name;
                outputInfo.rightConfidence = 1 - sumDistance;
            }
        }

        currentGestureInfo = outputInfo;
        return outputInfo;
    }

    public CurrentHandInfo getCurrentGestureInfo()
    {
        try
        {
            return currentGestureInfo;
        }
        catch (System.Exception)
        {
            CurrentHandInfo errorInfo = new CurrentHandInfo();
            errorInfo.leftConfidence = -1;
            errorInfo.leftHandGesture = "error";
            errorInfo.rightConfidence = -1;
            errorInfo.rightHandGesture = "error";
            return errorInfo;
        }
      
    }
}
