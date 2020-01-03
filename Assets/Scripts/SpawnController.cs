using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnController : MonoBehaviour
{
    public Vector3[] platePositions = new Vector3[12];
    public GameObject[] plates = new GameObject[12];
    public GameObject parentPlate, currentTable, newPlate;
    public int currentPlateNo = 0;
    public float timeSincePreviousOrder = 0, lowerTimeLim = 7f, upperTimeLim = 12f;
    public int activeOrders = 0, lowestDishNo = 0, highestDishNo = 5, lowestTableNo = 0, highestTableNo = 7, 
                currentTableNo, currentDishNo, maxOrdersOnTable = 5;
    public float nextOrderTime = 0, minFoodGenerationTime, maxFoodGenerationTime, currentFoodGenerationTime;
    public TableController currentTableController;
    public int[] currentOrdersOnTableCount = new int[10];

    void Start()
    {
        parentPlate = GameObject.Find("ParentPlate");
        for (int i = 0; i < 10; i++)
        {
            currentOrdersOnTableCount[i] = 0;
        }
    }

    void Update()
    {
        timeSincePreviousOrder += Time.deltaTime;
        CheckOrders();
    }

    void CheckOrders()
    {
        if (timeSincePreviousOrder >= nextOrderTime)
        {
            timeSincePreviousOrder = 0f;
            CreateOrder();
        }
    }

    void CreateOrder()
    {
        currentTableNo = Random.Range(lowestTableNo, highestTableNo + 1);
        while (currentOrdersOnTableCount[currentTableNo] >= maxOrdersOnTable)
        {
            currentTableNo = Random.Range(lowestTableNo, highestTableNo + 1);
        }
        currentOrdersOnTableCount[currentTableNo] += 1;
        currentTable = GameObject.Find("table" + currentTableNo.ToString());
        currentTableController = currentTable.GetComponent<TableController>();
        currentDishNo = Random.Range(lowestDishNo, highestDishNo + 1);
        currentTableController.dishesOnTable[currentDishNo] += 1;
        currentFoodGenerationTime = Random.Range(minFoodGenerationTime, maxFoodGenerationTime);
        Debug.Log("Order of " + currentDishNo.ToString() + " created on table " + currentTableNo.ToString() + " wait time " + currentFoodGenerationTime.ToString());
        nextOrderTime = Random.Range (lowerTimeLim, upperTimeLim);
        StartCoroutine(GenerateFoodOnTable(currentDishNo));
    }

    IEnumerator GenerateFoodOnTable (int dishNumber)
    {
        yield return new WaitForSeconds (currentFoodGenerationTime);
        GenerateFood(dishNumber);
        yield return null;
    }

    void GenerateFood(int dishNumber)
    {
        currentPlateNo = ChoosePlateNo();
        plates[currentPlateNo] = Instantiate (parentPlate, platePositions[currentPlateNo], Quaternion.identity);
        plates[currentPlateNo].name = "plate" + currentPlateNo.ToString();
        plates[currentPlateNo].GetComponent<PlateController>().plateNumber = currentPlateNo;
        plates[currentPlateNo].GetComponent<PlateController>().ChangeDishNumber(dishNumber);
        // currentPlateNo += 1;
    }

    public void DetachPlate (int plateNumber)
    {
        plates[plateNumber] = null;
        // currentPlateNo -= 1;
    }

    int ChoosePlateNo()
    {
        for (int i = 0; i < plates.Length; i++)
        {
            if (plates[i] == null)
            {
                currentPlateNo = i;
                break;
            }
        }
        return currentPlateNo;
    }

}
