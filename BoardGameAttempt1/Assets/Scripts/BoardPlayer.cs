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

    public GameObject shieldObject;

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
    private bool isRotating;

    public List<PlayerItem> playerItems = new List<PlayerItem>();

    public int coins;
    public int gems;

    private bool cursed = false;
    private bool shielded = false;

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

        coins = 20;
        gems = 0;
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
                        //Disable Button for item UI
                        UIManager.Instance.ShowInventoryUIButton(false);

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

                    Vector3 direction = currentWaypoint.position - transform.position;
                    Quaternion rotation = Quaternion.LookRotation(direction, Vector3.up);
                    if(transform.rotation != rotation)
                    {
                        transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, rotSpeed * 6 * Time.deltaTime);
                    }

                    if (Vector3.Distance(transform.position, currentWaypoint.position) < distanceThreshold)
                    {
                        if (!waypoints.CheckIfWaypointZero(currentWaypoint))
                        {
                            if(currentWaypoint.gameObject.GetComponent<Waypoint>().GetWayPointType() == WayPointType.Yellow)
                            {
                                Debug.Log("Yellow");
                                GameManager.Instance.UpdateGameState(GameState.TrophySpace);
                            }
                            pointsToMove--;

                            if(pointsToMove == 0)
                            {
                                Debug.Log("Waypoint type is: " + currentWaypoint.gameObject.GetComponent<Waypoint>().GetWayPointType());
                                HandleFinalMove(currentWaypoint.gameObject.GetComponent<Waypoint>().GetWayPointType());
                                UIManager.Instance.UpdatePlayerOrder();
                            }
                        }

                        currentWaypoint = waypoints.GetNextWaypoint(currentWaypoint);
                        //Rotate to next waypoint
                    }
                }
                else
                {
                    Quaternion rotation = Quaternion.LookRotation(Vector3.zero);
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, OGRot, rotSpeed * 6 * Time.deltaTime);
                    SetNextPlayer();

                }
            }
            else
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, OGRot, rotSpeed * 6 * Time.deltaTime);
            }
        }


        if (GameManager.Instance.GetState() == GameState.TrophySpace)
        {
            if(GameManager.Instance.GetActivePlayer().GetPlayerID() == playerID)
            {
                isMoving = false;
                anim.SetBool("isMoving", false);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, OGRot, rotSpeed * 6 * Time.deltaTime);
            }
        }
    }

    public void HandleFinalMove(WayPointType type)
    {
        switch (type)
        {
            case WayPointType.Blue:
                Debug.Log("Space is blue");
                coins += 5;
                UIManager.Instance.UpdateCoins(this);
                break;
            case WayPointType.Red:
                Debug.Log("Space is red");
                if(coins <= 5)
                {
                    coins = 0;
                }
                else
                {
                    coins -= 5;
                }

                UIManager.Instance.UpdateCoins(this);
                break;
            case WayPointType.Green:
                Debug.Log("Space is green");
                PlayerItem playerItem = (PlayerItem)UnityEngine.Random.Range(0, 3);

                Debug.Log("Player got " + playerItem.ToString());

                playerItems.Add(playerItem);

                break;
            case WayPointType.Yellow:
                Debug.Log("Space is yellow");
                //Implement trophy reward logic
                break;
            default:
                Debug.Log("Not a space type");
                break;
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

    public void CursePlayer()
    {
        cursed = true;
    }

    public void ProtectPlayer()
    {
        shielded = true;
    }

    internal bool IsPlayerCursed()
    {
        return cursed;
    }

    internal bool isPlayerProtected()
    {
        return shielded;
    }

    internal void RemoveCurse()
    {
        cursed = false;
    }

    internal void RemoveShield()
    {
        shielded = false;
    }
}

public enum PlayerItem
{
    CoinTransfer,
    CursedDice,
    PositionSwap,
    Shield
}