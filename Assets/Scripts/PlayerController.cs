using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 1f;
    private Rigidbody2D rb2d;
    private float moveX, moveY;
    public float interactDistance = 2f;
    private Animator anim;
    public bool moving = false, holdingPlate = false, preparingToss = false, tossing = false;
    public Vector2 walkToFirst, walkToSecond, plateLeftPos;
    public int walkStretch = 0, walkToFood = 0;
    // private GameObject pickedObject = null;
    public PlateController plateController = null;
    public int throwDir = 1;            // 1 for right, -1 for left
    public SpawnController spawnController;

    public enum playerState
    {
        idle, walking, plate, plateWalking, tossing, frozen
    }

    public playerState currentState;

    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        currentState = playerState.idle;
        spawnController = GameObject.Find("GameController").GetComponent<SpawnController>();
    }

    void Update()
    {
        ResetVars();
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Interact();
        }
        if (walkStretch != 0)
        {
            WalkToServe();
        }
        if (walkToFood != 0)
        {
            WalkToGetFood();
        }
        if (currentState != playerState.frozen)
        {
            Movement();
        }
        if (currentState == playerState.plate || currentState == playerState.plateWalking)
        {
            CheckToss();
        }
        SetAnimVars();
        SetPlayerState();
    }

    void SetAnimVars()
    {
        anim.SetBool("moving", moving);
        anim.SetBool("preparingToss", preparingToss);
        anim.SetBool("tossing", tossing);
        anim.SetBool("plate", holdingPlate);
    }

    void CheckToss ()
    {
        if (holdingPlate)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                preparingToss = true;
                plateController.throwDir = throwDir;
                plateController.BeginTossProcedure();
                plateController = null;
                // pickedObject = null;
                // WalkToGetFood();
            }
            
        }
    }

    void Interact ()
    {
        if (!holdingPlate)
        {
            int layerMask = ~ (1 << 8);
            RaycastHit2D hit = Physics2D.Raycast (transform.position, -Vector2.up, 
                                                    interactDistance, layerMask);
            if (hit)
            {
                Pickup (hit.collider.gameObject);
                plateController = hit.collider.gameObject.GetComponent<PlateController>();
            }
        }
    }

    void Pickup (GameObject plateObject)
    {
        holdingPlate = true;
        plateObject.transform.parent = transform;
        plateObject.transform.localPosition = new Vector3 (plateLeftPos.x, plateLeftPos.y, 
                                                            transform.position.z);
        walkStretch = 1;
        spawnController.DetachPlate(plateObject.GetComponent<PlateController>().plateNumber);
    }

    void WalkToServe ()
    {
        currentState = playerState.frozen;
        Vector3 pos1 = new Vector3 (walkToFirst.x, walkToFirst.y, transform.position.z);
        Vector3 pos2 = new Vector3 (walkToSecond.x, walkToSecond.y, transform.position.z);
        if (walkStretch == 1)
        {
            rb2d.MovePosition(Vector3.MoveTowards(transform.position,
                                                    pos1, speed * Time.deltaTime));
            moving = true;
        }
        if (transform.position == pos1)
        {
            walkStretch = 2;
        }
        if (walkStretch == 2)
        {
            rb2d.MovePosition(Vector3.MoveTowards(transform.position,
                                                    pos2, speed * Time.deltaTime));
            moving = true;
        }
        if (transform.position == pos2)
        {
            walkStretch = 0;
            currentState = playerState.plate;
        }
    }

        public void WalkToGetFood ()
    {
        currentState = playerState.frozen;
        Vector3 pos2 = new Vector3 (walkToFirst.x, walkToFirst.y, transform.position.z);
        Vector3 pos1 = new Vector3 (walkToSecond.x, walkToSecond.y, transform.position.z);
        if (walkToFood == 1)
        {
            rb2d.MovePosition(Vector3.MoveTowards(transform.position,
                                                    pos1, speed * Time.deltaTime));
            moving = true;
        }
        if (transform.position == pos1)
        {
            walkToFood = 2;
        }
        if (walkToFood == 2)
        {
            rb2d.MovePosition(Vector3.MoveTowards(transform.position,
                                                    pos2, speed * Time.deltaTime));
            moving = true;
        }
        if (transform.position == pos2)
        {
            walkToFood = 0;
            currentState = playerState.plate;
        }
    }

    void ResetVars ()
    {
        moveX = 0;
        moveY = 0;
        moving = false;
        anim.SetBool("moving", false);
    }

    void Movement()
    {
        if (currentState == playerState.plate || currentState == playerState.plateWalking)
        {
            moveY = Input.GetAxis("Vertical");
            if (Mathf.Abs(moveY) > 0)
            {
                moving = true;
                rb2d.MovePosition(transform.position + (new Vector3 (0, moveY, 0) * speed * Time.deltaTime));
                if (moveY < 0)
                {
                    throwDir = 1;
                }
                else
                {
                    throwDir = -1;
                }
            }
        }
        if (currentState == playerState.idle || currentState == playerState.walking)
        {
            moveX = Input.GetAxis("Vertical");
            if (Mathf.Abs(moveX) > 0)
            {
                moving = true;
                rb2d.MovePosition(transform.position + (new Vector3 (moveX, 0, 0) * speed * Time.deltaTime));
            }
        }
    }

    void SetPlayerState ()
    {
        if (moving)
        {
            if (holdingPlate)
            {
                currentState = playerState.plateWalking;
            }
            else
            {
                currentState = playerState.walking;
            }
        }
        else
        {
            if (holdingPlate)
            {
                currentState = playerState.plate;
            }
            else
            {
                currentState = playerState.idle;
            }
        }
    }
}
