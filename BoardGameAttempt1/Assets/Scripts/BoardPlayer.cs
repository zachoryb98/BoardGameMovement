using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardPlayer : MonoBehaviour
{
    public int playerID;
    public string playerName;

    public Quaternion OGRot;

    private Animator anim = null;
    Rigidbody rigidbody;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    public float jumpHeight = 3f;

    Vector3 velocity;
    public bool isGrounded;
    public bool isMoving;

    public float rotSpeed = 2f;

    Transform movetoWaypoint = null;
    public int pointsToMove;

    [SerializeField]
    public Waypoints waypoints;
    public float speed = 5f;
    private Transform currentWaypoint;
    private float distanceThreshold = 0.1f;


    private BoardPlayer()
    {
        //Do nothing
    }

    private void Start()
    {
        GameManager.Instance.AddPlayer(this);
        Debug.Log("Added " + playerName + " as Player " + playerID);

        rigidbody = this.GetComponent<Rigidbody>();
        anim = this.GetComponent<Animator>();

        OGRot = gameObject.transform.rotation;
    }

    private void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (GameManager.Instance.GetState() == GameState.SelectOrder)
        {
            if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
            {
                StartCoroutine(Jump());
            }
        }

        if (GameManager.Instance.GetState() == GameState.ActivePlayerTurn)
        {
            if (RoundManager.Instance.GetActivePlayer() == playerID)
            {
                isMoving = false;
                if (isGrounded)
                {
                    if (Input.GetKeyDown(KeyCode.Space) && !isMoving)
                    {
                        //GameManager will give us rolled number and tell us how many spaces to move
                        StartCoroutine(Jump());
                    }
                }
            }
        }

        if (GameManager.Instance.GetState() == GameState.ChangeTurn)
        {
            transform.rotation = Quaternion.LookRotation(Vector3.zero, Vector3.zero);
        }

        if (GameManager.Instance.GetState() == GameState.MovePlayer)
        {
            if (GameManager.Instance.GetActivePlayer().GetPlayerID() == playerID && isMoving)
            {
                if (currentWaypoint == null)
                {
                    currentWaypoint = waypoints.GetNextWaypoint(currentWaypoint);
                    transform.LookAt(currentWaypoint);
                    return;
                }
                if (pointsToMove > 0)
                {
                    //Spot position + 1 on the y
                    //var wireMeshPosition = new Vector3(currentWaypoint.position.x, currentWaypoint.position.y + 1f + currentWaypoint.position.z);

                    
                    transform.position = Vector3.MoveTowards(transform.position, currentWaypoint.position, speed * Time.deltaTime);
                    if (Vector3.Distance(transform.position, currentWaypoint.position) < distanceThreshold)
                    {
                        if (!waypoints.CheckIfWaypointZero(currentWaypoint))
                        {
                            pointsToMove--;

                            if(pointsToMove == 0)
                            {
                                Debug.Log("Waypoint type is: " + currentWaypoint.gameObject.GetComponent<Waypoint>().GetWayPointType());
                            }
                        }

                        currentWaypoint = waypoints.GetNextWaypoint(currentWaypoint);
                        //Rotate to next waypoint
                        Vector3 direction = currentWaypoint.position - transform.position;
                        Quaternion rotation = Quaternion.LookRotation(direction);
                        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, rotSpeed * 6 * Time.deltaTime);
                    }
                }
                else
                {
                    Quaternion rotation = Quaternion.LookRotation(Vector3.zero);
                    transform.rotation = Quaternion.Lerp(transform.rotation, OGRot, rotSpeed * 6 * Time.deltaTime);
                    SetNextPlayer();

                }
            }
            else
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, OGRot, rotSpeed * 6 * Time.deltaTime);
            }
        }
    }




    public IEnumerator WaitThenMove(int rolledNumber)
    {
        pointsToMove = rolledNumber;
        //Print the time of when the function is first called.
        Debug.Log("Started Coroutine at timestamp : " + Time.time);
        isMoving = false;

        yield return new WaitForSeconds(1);

        if(currentWaypoint != null)
        {
            Vector3 direction = currentWaypoint.position - transform.position;
            Quaternion rotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, rotSpeed * 3 * Time.deltaTime);
        }

        isMoving = true;
        anim.SetBool("isMoving", true);
    }

    private IEnumerator Jump()
    {
        //To do create a jump animation        
        anim.SetTrigger("Jump");
        yield return new WaitForSeconds(.35f);
        GameManager.Instance.DestroyPlayerDice(playerID);
    }

    private void SetNextPlayer()
    {
        isMoving = false;
        anim.SetBool("isMoving", isMoving);
        Quaternion rotation = Quaternion.LookRotation(Vector3.zero);
        transform.rotation = Quaternion.Lerp(transform.rotation, OGRot, rotSpeed * 3 * Time.deltaTime);
        RoundManager.Instance.SetNextActivePlayer(this);
    }

    public int GetPlayerID()
    {
        return playerID;
    }
}
