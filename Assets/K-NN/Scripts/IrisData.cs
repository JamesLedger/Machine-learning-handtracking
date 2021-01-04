using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IrisData
{
    // Start is called before the first frame update
    public float sepal_length { get; set; }
    public float sepal_width { get; set; }
    public float petal_length { get; set; }
    public float petal_width { get; set; }
    public string species { get; set; }
}

public class DataPoint
{
    // Start is called before the first frame update
    public IrisDetail flowerData { get; set; }
    public Vector3 position { get; set; }
}
