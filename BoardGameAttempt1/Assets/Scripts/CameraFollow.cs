using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{

    public BoardPlayer targetPlayer;

    public Vector3 cameraOffset;

    public GameObject offsetObj;

    public float smoothFactor = 0.5f;

    private void Start()
    {
        cameraOffset = transform.position - offsetObj.transform.position;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if(GameManager.Instance.GetState() == GameState.ActivePlayerTurn || GameManager.Instance.GetState() == GameState.MovePlayer)
        {
            if (targetPlayer != null && targetPlayer.playerID == RoundManager.Instance.GetActivePlayer())
            {
                Vector3 newPosition = targetPlayer.transform.position + cameraOffset;
                transform.position = Vector3.Slerp(transform.position, newPosition, smoothFactor);
            }
            else
            {
                targetPlayer = GameManager.Instance.GetActivePlayer();
            }
        }
    }
}