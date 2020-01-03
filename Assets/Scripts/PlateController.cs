using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlateController : MonoBehaviour
{
    public float maxSpeed = 3f, tossedSpeed = 3f, horizontalDecceleration = 0.25f;
    private Rigidbody2D rb2d;
    public float verticalDecceleration = 0.25f, maxSizeScaling = 0.25f;
    private float initialY, verticalVelocity, initialVerticalVelocity, verticalDiff;
    public int throwDir = 1;            // 1 for right, -1 for left
    public bool tossed = false, startCount = false, landed = false;
    public float releaseTime = 0, maxReleaseTime = 0.8f, minReleaseTime = 0.3f, lowerSpeedThreshold = 0f, travelTime = 0f;
    private PlayerController playerController;
    public int dishOnPlate = -1, plateNumber = 0;
    public GameObject activeTable = null;
    public ScoreScript scoreScript;
    public SpawnController spawnController;
    public Animator anim;

    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        tossedSpeed = maxSpeed;
        releaseTime = minReleaseTime;
        playerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        scoreScript = GameObject.FindWithTag("Score").GetComponent<ScoreScript>();
        spawnController = GameObject.Find("GameController").GetComponent<SpawnController>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (startCount)
        {
            releaseTime += Time.deltaTime;
            if (releaseTime > maxReleaseTime)
            {
                releaseTime = maxReleaseTime;
                Toss();
            }
            if (Input.GetKeyUp(KeyCode.Space))
            {
                Toss();
            }
        }
        if (tossed)
        {
            transform.parent = null;
            if (throwDir == 1)
            {
                transform.position = transform.position + (new Vector3 (tossedSpeed, verticalVelocity, 0) * Time.deltaTime);
            }
            else
            {
                transform.position = transform.position + (new Vector3 (-tossedSpeed, verticalVelocity, 0) * Time.deltaTime);
            }
            tossedSpeed = tossedSpeed - horizontalDecceleration;
            verticalVelocity -= verticalDecceleration;
            verticalDiff = transform.position.y - initialY;
            if (verticalDiff < 0f)
            {
                verticalDiff = 0f;
            }
            transform.localScale = new Vector3 (1, 1, 1) * (1 + (verticalDiff * maxSizeScaling));
            if (transform.position.y <= initialY)
            {
                verticalVelocity = 0f;
            }
            if (tossedSpeed <= lowerSpeedThreshold)
            {
                tossedSpeed = 0;
                if (verticalVelocity == 0)
                {
                    transform.localScale = new Vector3(1, 1, 1);
                    tossed = false;
                    playerController.tossing = false;
                    playerController.holdingPlate = false;
                    landed = true;
                }
            }
            // if (Input.GetKeyUp(KeyCode.Space))
            // {
            //     tossedSpeed *= 0.3f;
            // }
            if (landed)
            {
                CheckDish();
                // Die();
            }
        }
    }

    void CheckDish ()
    {
        if (activeTable != null)
        {
            TableController tableController = activeTable.GetComponent<TableController>();
            if (tableController.dishesOnTable[dishOnPlate] > 0)
            {
                tableController.dishesOnTable[dishOnPlate] -= 1;
                IncreaseScore(10);
            }
        }
        StartCoroutine("Die");
    }

    IEnumerator Die ()
    {
        yield return new WaitForSeconds(2);
        Destroy(gameObject);
        yield return null;
    }

    void IncreaseScore (int inc)
    {
        scoreScript.score += inc;
    }

    public void ChangeDishNumber (int newDishNo)
    {
        dishOnPlate = newDishNo;
        anim.SetInteger("dish", newDishNo);
    }

    void Toss()
    {
        playerController.preparingToss = false;
        playerController.walkToFood = 1;
        playerController.tossing = true;
        startCount = false;
        tossedSpeed = maxSpeed * releaseTime / maxReleaseTime;
        travelTime = tossedSpeed / horizontalDecceleration;
        tossed = true;
        initialVerticalVelocity = verticalDecceleration * travelTime / 2; 
        verticalVelocity = initialVerticalVelocity;
        initialY = transform.position.y;
        verticalDiff = 0f;
    }

    public void BeginTossProcedure()
    {
        startCount = true;
    }

    void OnTriggerEnter2D (Collider2D col)
    {
        if (col.tag == "Table")
        {
            activeTable = col.gameObject;
        }
    }

    void OnTriggerExit2D (Collider2D col)
    {
        if (col.tag == "Table")
        {
            activeTable = null;
        }
    }

}
