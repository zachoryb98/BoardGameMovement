using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dice : MonoBehaviour
{
    public bool hasBeenHit = false;
    public int linkedPlayerID = 0;


    Vector3 rot = Vector3.one;

    void Start()
    {
        //Spin animation
        GameManager.Instance.AddDice(this);
    }


    void Update()
    {
        transform.Rotate(rot);
    }

    public void DestroyDice()
    {
        Debug.Log("Dice Hit");
        Destroy(this.gameObject);
    }

    public void SetPlayerId(int playerId)
    {
        linkedPlayerID = playerId;
    }

    public int GetLinkedPlayerID()
    {
        return linkedPlayerID;
    }
}
