using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using Random = System.Random;

public enum Species
{
    setosa,
    versicolor,
    virginica,
    unclassified
}

public class IrisDetail
{
    public float sepal_length { get; set; }
    public float sepal_width { get; set; }
    public float petal_length { get; set; }
    public float petal_width { get; set; }
    public Species speciesActual { get; set; }
    public Species speciesPrediction { get; set; }
    public double distanceFromPoint { get; set; }
    public bool selfUpdate { get; set; }
}

public class Knn : MonoBehaviour
{
    [SerializeField]
    public Material SetosaMat;
    [SerializeField]
    public Material VersicolorMat;
    [SerializeField]
    public Material VirginicaMat;
    [SerializeField]
    public Material UnknownMat;

    public List<IrisDetail> testingData;
    public List<IrisDetail> trainingData;

    private void Start()
    {
        demonstrationStage = 0;
        delayCounter = 0;
        tempTestingObjects = new List<GameObject>();
    }

    public int demonstrationStage;

    public TextMeshPro textTitle;
    public TextMeshPro textContent;

    int delayCounter;
    public List<IrisDetail> predictions;
    public List<IrisDetail> allIrisData;

    private void Update()
    {
        if (delayCounter < 75)
        {
            delayCounter++;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (delayCounter == 75)
        {
            delayCounter = 0;
            switch (demonstrationStage)
            {
                case 0: // Explaination of KNN
                    {
                        //Read in data set
                        allIrisData = GetIrisData();

                        demonstrationStage++;
                        break;
                    }
                case 1: //Data split
                    {
                        textTitle.text = "Splitting the data";

                        textContent.text = "It's important to be able to evaluate the model and to help us do this we set aside some of the data we started out with.";

                        List<List<IrisDetail>> seperatedData = SplitData(allIrisData, 25f);
                        testingData = seperatedData[0];
                        trainingData = seperatedData[1];

                        demonstrationStage++;
                        break;
                    }
                case 2:
                    {
                        textContent.text = "Here you can just the training data which in this case is 75% of the total dataset. These points have been chosen randomly and there should ideally be an equal number of all three Iris species here.";

                        VisualiseTraining(trainingData);

                        demonstrationStage++;
                        break;
                    }
                case 3:
                    {
                        textContent.text = "Here in white you can see the points used for training the model, take note of how they change between here and the next scene";

                        VisualiseRawTesting(testingData);

                        demonstrationStage++;
                        break;
                    }
                case 4: // Applying the model
                    {
                        textTitle.text = "Applying the model";

                        textContent.text = "The model has now been applied to the testing dataset, ";

                        VisualiseAppliedTesting(testingData);

                        //Run testing data against training data
                        predictions = PredictAllPoints(testingData, trainingData);

                        demonstrationStage++;
                        break;
                    }
                case 5: //Accuracy
                    {
                        textTitle.text = "Evaluating the model";

                        textContent.text = "On the left you can see the raw accuracy score of the model based on the data set aside for texting. Try experimenting with different values of K to see how you can improve this figure. ON the right you can see a confusion matrix which gives a more in depth look to the accuracy of the model.";

                        //Evaluate performance of the model
                        EvaluatePerformance(predictions);

                        demonstrationStage++;
                        break;
                    }
                case 6: //Have a go
                    {
                        textTitle.text = "Have a go!";

                        textContent.text = "Press the orange button to spawn a point, you can then drag it around the graph to see how moving it about affects how it is classified.";

                        demonstrationStage++;
                        break;
                    }

                default:
                    break;
            }

        }

    }




    public void EvaluatePerformance(List<IrisDetail> predictions)
    {
        int perfCounter = 0;

        foreach (IrisDetail dataPoint in predictions)
        {
            if (dataPoint.speciesActual == dataPoint.speciesPrediction)
            {
                perfCounter++;
            }
        }

        double performance = ((double)perfCounter / (double)predictions.Count) * 100;

        Debug.Log("The model is " + performance + "% accurate");
    }

    public static void ViewSpecies(List<IrisDetail> rawData)
    {
        foreach (IrisDetail x in rawData)
        {
            Debug.Log(x.speciesActual);
        }
    }

    public List<List<IrisDetail>> SplitData(List<IrisDetail> inputData, float percentForTesting)
    {
        List<IrisDetail> testingData = new List<IrisDetail>();
        List<IrisDetail> trainingData = new List<IrisDetail>();

        //Randomise order of data in list (Fisher-Yates shuffle)
        Random rng = new Random();

        int n = inputData.Count;

        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            IrisDetail value = inputData[k];
            inputData[k] = inputData[n];
            inputData[n] = value;
        }

        //Figure out how many to set aside for testing
        double testingPointsRequired = ((float)inputData.Count / 100f) * percentForTesting;
        testingPointsRequired = Math.Round(testingPointsRequired, 0);

        //Seperate data into testing and training 

        int testingCounter = 0;
        foreach (IrisDetail dataPoint in inputData)
        {
            if (testingCounter < testingPointsRequired)
            {
                testingData.Add(dataPoint);
                testingCounter++;
            }
            else
            {
                trainingData.Add(dataPoint);
            }
        }

        List<List<IrisDetail>> returnData = new List<List<IrisDetail>>();
        returnData.Add(testingData);
        returnData.Add(trainingData);

        //Summary for debugging
        Debug.Log("  " + trainingData.Count + " Values set aside for training");
        Debug.Log("  " + testingData.Count + " Values set asisde for testing\n");

        return returnData;
    }

    public List<IrisDetail> PredictAllPoints(List<IrisDetail> testingData, List<IrisDetail> trainingData)
    {
        Debug.Log("Please enter a value for K:");
        int k = 1;

        List<IrisDetail> predictedData = new List<IrisDetail>();

        if (testingData.Count == 0)
        {
            IrisDetail emptyData = new IrisDetail
            {
                speciesActual = Species.unclassified
            };

            predictedData.Add(emptyData);
        }

        foreach (IrisDetail testingPoint in testingData)
        {
            predictedData.Add(PredictOnePoint(testingPoint, trainingData, k));
        }

        PopulateConfusionMatrix(predictedData);

        return predictedData;
    }

    //Calculate distance from single testing value to all training values 
    public IrisDetail PredictOnePoint(IrisDetail toPredict, List<IrisDetail> trainingData, int kValue)
    {

        if (trainingData.Count == 0)
        {
            IrisDetail emptyData = new IrisDetail
            {
                speciesActual = Species.unclassified
            };

            return emptyData;
        }

        foreach (IrisDetail trainingPoint in trainingData)
        {
            double xVal = Math.Pow(toPredict.sepal_length - trainingPoint.sepal_length, 2);
            double yVal = Math.Pow(toPredict.sepal_width - trainingPoint.sepal_width, 2);
            double zVal = Math.Pow(toPredict.petal_length - trainingPoint.petal_length, 2);

            trainingPoint.distanceFromPoint = Math.Sqrt(xVal + yVal + zVal);
        }

        List<IrisDetail> sorted = trainingData.OrderBy(p => p.distanceFromPoint).ToList();
        toPredict.speciesPrediction = sorted[0].speciesActual;

        return toPredict;
    }

    //Obtaining Iris data from csv file
    public List<IrisDetail> GetIrisData()
    {
        List<IrisDetail> irisData = new List<IrisDetail>();

        using (StreamReader reader = new StreamReader("iris.csv"))
        {
            string line;

            bool skippedTitles = false;
            while ((line = reader.ReadLine()) != null)
            {
                if (skippedTitles)
                {
                    string[] details = line.Split(',');
                    Species tempSpecies = new Species();
                    if (details[4] == "setosa")
                        tempSpecies = Species.setosa;
                    else if (details[4] == "versicolor")
                        tempSpecies = Species.versicolor;
                    else if (details[4] == "virginica")
                        tempSpecies = Species.virginica;

                    IrisDetail tempDetail = new IrisDetail()
                    {
                        sepal_length = float.Parse(details[0]),
                        sepal_width = float.Parse(details[1]),
                        petal_length = float.Parse(details[2]),
                        petal_width = float.Parse(details[3]),
                        speciesActual = tempSpecies,
                        speciesPrediction = Species.unclassified,
                        distanceFromPoint = float.PositiveInfinity,
                        selfUpdate = false
                    };

                    irisData.Add(tempDetail);
                }
                else
                {
                    skippedTitles = true;
                }

            }
        }

        return irisData;
    }

    public GameObject SpacePoint;
    public GameObject TempSpacePoint;


    public void VisualiseTraining(List<IrisDetail> dataset)
    {
        float maxSepalWidth = 4.4f; //x
        float maxSepalLength = 7.9f; //y
        float maxPetalLength = 6.9f; //z

        List<DataPoint> dataPointList = new List<DataPoint>();
        GameObject Origin = GameObject.Find("Origin");

        foreach (var dataPoint in dataset)
        {
            //Linking data to a position
            DataPoint temp = new DataPoint
            {
                flowerData = dataPoint,
                position = new Vector3(0, 0, 0)
            };
            dataPointList.Add(temp);
        }

        Debug.Log("count: " + dataPointList.Count);

        foreach (var dataPoint in dataPointList)
        {
            float x = dataPoint.flowerData.sepal_length / maxSepalLength * 10; // sepal length
            float y = dataPoint.flowerData.sepal_width / maxSepalWidth * 10; // sepal width
            float z = dataPoint.flowerData.petal_length / maxPetalLength * 10; // petal length

            // Ensuring point position is relative to origin
            dataPoint.position = Origin.gameObject.transform.position + new Vector3(x * -1 / 20, y / 20, z / 20);

            //Instantiating a each data point with the correct material depending on the species
            GameObject temp = Instantiate(SpacePoint);
            temp.transform.position = dataPoint.position;

            if (dataPoint.flowerData.speciesActual == Species.setosa)
            {
                temp.GetComponent<MeshRenderer>().material = SetosaMat;
            }

            if (dataPoint.flowerData.speciesActual == Species.versicolor)
            {
                temp.GetComponent<MeshRenderer>().material = VersicolorMat;
            }

            if (dataPoint.flowerData.speciesActual == Species.virginica)
            {
                temp.GetComponent<MeshRenderer>().material = VirginicaMat;
            }

        }
    }

    public List<GameObject> tempTestingObjects;

    public void VisualiseRawTesting(List<IrisDetail> dataset)
    {
        if (tempTestingObjects != null)
        {
            foreach (GameObject tempPoint in tempTestingObjects)
            {
                Destroy(tempPoint);
            }
        }
        

        float maxSepalWidth = 4.4f; //x
        float maxSepalLength = 7.9f; //y
        float maxPetalLength = 6.9f; //z

        List<DataPoint> dataPointList = new List<DataPoint>();
        GameObject Origin = GameObject.Find("Origin");

        foreach (var dataPoint in dataset)
        {
            //Linking data to a position
            DataPoint temp = new DataPoint
            {
                flowerData = dataPoint,
                position = new Vector3(0, 0, 0)
            };
            dataPointList.Add(temp);
        }

        Debug.Log("count: " + dataPointList.Count);

        foreach (var dataPoint in dataPointList)
        {
            float x = dataPoint.flowerData.sepal_length / maxSepalLength * 10; // sepal length
            float y = dataPoint.flowerData.sepal_width / maxSepalWidth * 10; // sepal width
            float z = dataPoint.flowerData.petal_length / maxPetalLength * 10; // petal length

            // Ensuring point position is relative to origin
            dataPoint.position = Origin.gameObject.transform.position + new Vector3(x * -1 / 20, y / 20, z / 20);

            //Instantiating a each data point with the unknown colour
            GameObject temp = Instantiate(TempSpacePoint);
            tempTestingObjects.Add(temp);
            temp.transform.position = dataPoint.position;
            temp.GetComponent<MeshRenderer>().material = UnknownMat;
        }
    }

    public void VisualiseAppliedTesting(List<IrisDetail> dataset)
    {
        GameObject[] oldPoints;
        oldPoints = GameObject.FindGameObjectsWithTag("TempPoint");

        foreach (GameObject point in oldPoints)
        {
            Destroy(point);
        }


        float maxSepalWidth = 4.4f; //x
        float maxSepalLength = 7.9f; //y
        float maxPetalLength = 6.9f; //z

        List<DataPoint> dataPointList = new List<DataPoint>();
        GameObject Origin = GameObject.Find("Origin");

        foreach (var dataPoint in dataset)
        {
            //Linking data to a position
            DataPoint temp = new DataPoint
            {
                flowerData = dataPoint,
                position = new Vector3(0, 0, 0)
            };
            dataPointList.Add(temp);
        }

        Debug.Log("count: " + dataPointList.Count);

        foreach (var dataPoint in dataPointList)
        {
            float x = dataPoint.flowerData.sepal_length / maxSepalLength * 10; // sepal length
            float y = dataPoint.flowerData.sepal_width / maxSepalWidth * 10; // sepal width
            float z = dataPoint.flowerData.petal_length / maxPetalLength * 10; // petal length

            // Ensuring point position is relative to origin
            dataPoint.position = Origin.gameObject.transform.position + new Vector3(x * -1 / 20, y / 20, z / 20);

            //Instantiating a each data point with the unknown colour
            GameObject temp = Instantiate(SpacePoint);
            temp.transform.position = dataPoint.position;

            if (dataPoint.flowerData.speciesActual == Species.setosa)
            {
                temp.GetComponent<MeshRenderer>().material = SetosaMat;
            }

            if (dataPoint.flowerData.speciesActual == Species.versicolor)
            {
                temp.GetComponent<MeshRenderer>().material = VersicolorMat;
            }

            if (dataPoint.flowerData.speciesActual == Species.virginica)
            {
                temp.GetComponent<MeshRenderer>().material = VirginicaMat;
            }
        }
    }

  



    [SerializeField]
    public List<GameObject> ConfusionMatrixObjects;

    public void PopulateConfusionMatrix(List<IrisDetail> inputData)
    {

        int S_Ver = 0;
        int S_Vir = 0;
        int Ver_S = 0;
        int Ver_Ver = 0;
        int Ver_Vir = 0;
        int Vir_S = 0;
        int Vir_Ver = 0;
        int Vir_Vir = 0;
        int S_S = 0;


        //Counters for each option in the matrix
        //Not optimal method but is nice for clarity
        //S = Setosa, Ver = Versicolor, Vir = Virginica
        int correctCounter = 0;
        foreach (var currentPoint in inputData)
        {
            //Correct cases 
            if (currentPoint.speciesActual == currentPoint.speciesPrediction)
            {
                if (currentPoint.speciesPrediction == Species.setosa)
                {
                    S_S++;
                }
                else if (currentPoint.speciesPrediction == Species.virginica)
                {
                    Vir_Vir++;
                }
                else if (currentPoint.speciesPrediction == Species.versicolor)
                {
                    Ver_Ver++;
                }
                correctCounter++;
            }
            else //Mistakes
            {
                if (currentPoint.speciesPrediction == Species.setosa && currentPoint.speciesActual == Species.virginica)
                {
                    S_Vir++;
                }
                if (currentPoint.speciesPrediction == Species.setosa && currentPoint.speciesActual == Species.versicolor)
                {
                    S_Ver++;
                }
                if (currentPoint.speciesPrediction == Species.virginica && currentPoint.speciesActual == Species.setosa)
                {
                    Vir_S++;
                }
                if (currentPoint.speciesPrediction == Species.virginica && currentPoint.speciesActual == Species.versicolor)
                {
                    Vir_Ver++;
                }
                if (currentPoint.speciesPrediction == Species.versicolor && currentPoint.speciesActual == Species.setosa)
                {
                    Ver_S++;
                }
                if (currentPoint.speciesPrediction == Species.versicolor && currentPoint.speciesActual == Species.virginica)
                {
                    Ver_Vir++;
                }
            }
        }

        float accuracyFloat = (float)correctCounter / (float)inputData.Count;
        //display accuracy
        TextMeshPro accuracy = ConfusionMatrixObjects[9].GetComponent<TextMeshPro>();
        accuracy.text = Math.Round((accuracyFloat * 100), 2).ToString() + "%";

        //display test/train counts
        TextMeshPro testcount = ConfusionMatrixObjects[11].GetComponent<TextMeshPro>();
        testcount.text = inputData.Count.ToString();

        TextMeshPro traincount = ConfusionMatrixObjects[10].GetComponent<TextMeshPro>();
        traincount.text = (150 - inputData.Count).ToString();

        //top row conf mat
        TextMeshPro a1 = ConfusionMatrixObjects[0].GetComponent<TextMeshPro>();
        a1.text = S_S.ToString();
        TextMeshPro b1 = ConfusionMatrixObjects[1].GetComponent<TextMeshPro>();
        b1.text = S_Ver.ToString();
        TextMeshPro c1 = ConfusionMatrixObjects[2].GetComponent<TextMeshPro>();
        c1.text = S_Vir.ToString();

        //middle row conf mat
        TextMeshPro a2 = ConfusionMatrixObjects[3].GetComponent<TextMeshPro>();
        a2.text = Ver_S.ToString();
        TextMeshPro b2 = ConfusionMatrixObjects[4].GetComponent<TextMeshPro>();
        b2.text = Ver_Ver.ToString();
        TextMeshPro c2 = ConfusionMatrixObjects[5].GetComponent<TextMeshPro>();
        c2.text = Ver_Vir.ToString();

        //bottom row conf mat
        TextMeshPro a3 = ConfusionMatrixObjects[6].GetComponent<TextMeshPro>();
        a3.text = Vir_S.ToString();
        TextMeshPro b3 = ConfusionMatrixObjects[7].GetComponent<TextMeshPro>();
        b3.text = Vir_Ver.ToString();
        TextMeshPro c3 = ConfusionMatrixObjects[8].GetComponent<TextMeshPro>();
        c3.text = Vir_Vir.ToString();
    }

    public void Reclassify()
    {

    }
}
