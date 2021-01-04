using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static GestureDetector;

public class StatsBoard : MonoBehaviour
{
   
    // Update is called once per frame
    void Update()
    {
        WriteGesturesToBoard();
    }

    public TextMeshPro leftGestureName;
    public TextMeshPro leftGestureConfidence;
    public TextMeshPro rightGestureName;
    public TextMeshPro rightGestureConfidence;

    void WriteGesturesToBoard()
    {
        GestureDetector detector = GetComponent<GestureDetector>();
        CurrentHandInfo handStats = detector.Recognise();
        try
        {
            if (handStats.leftConfidence != 0)
                leftGestureConfidence.text = (handStats.leftConfidence).ToString().Substring(0, 4) + "%";
            else
                leftGestureConfidence.text = "--";

            if (handStats.leftHandGesture == null)
                leftGestureName.text = "unknown";
            else
                leftGestureName.text = handStats.leftHandGesture;
        }
        catch 
        {

        }

        try
        {
            if (handStats.rightConfidence != 0)
                rightGestureConfidence.text = (handStats.rightConfidence).ToString().Substring(0, 4) + "%";
            else
                rightGestureConfidence.text = "--";

            if (handStats.rightHandGesture == null)
                rightGestureName.text = "unknown";
            else
                rightGestureName.text = handStats.rightHandGesture;
        }
        catch
        {

           
        }
        
    }
}
