using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OculusSampleFramework;
using OVRTouchSample;

public class HandTrackingGrabber : OVRGrabber
{
    private Hand hand;
    public float pinchThreshhold = 0.7f;


    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        hand = GetComponent<Hand>();
    }

    public override void Update()
    {
        base.Update();
        CheckIndexPinch();
    }

    void CheckIndexPinch()
    {
        float pinchStrength = GetComponent<OVRHand>().GetFingerPinchStrength(OVRHand.HandFinger.Index);
        bool isPinching = pinchStrength > pinchThreshhold;

        if (!m_grabbedObj && isPinching &&m_grabCandidates.Count > 0)
            GrabBegin();
        else if (m_grabbedObj && !isPinching)
            GrabEnd();
    }

    protected override void GrabEnd()
    {
        if (m_grabbedObj)
        {
            Vector3 linearVelocity = (transform.position - m_lastPos) / Time.fixedDeltaTime;
            Vector3 angularVelocity = (transform.eulerAngles - m_lastRot.eulerAngles) / Time.fixedDeltaTime;

            GrabbableRelease(linearVelocity, angularVelocity);
        }

        GrabVolumeEnable(true);
    }
}
