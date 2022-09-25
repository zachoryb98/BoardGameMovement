using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class RoundManager : MonoBehaviour
{
    public static RoundManager Instance;

    int currentRound = 0;

    public Dictionary<int, int> playerOrder = new Dictionary<int, int>();

    int activePlayer = 0;

    int playersLeftInTurn = 0;


    private void Awake()
    {
        Instance = this;
    }


    private void IncrementRound()
    {
        currentRound++;
        playersLeftInTurn = playerOrder.Count;
    }

    public void SetNextActivePlayer(BoardPlayer boardPlayer)
    {
        int counter = 0;
        foreach (KeyValuePair<int, int> entry in playerOrder)
        {            
            if(entry.Key == boardPlayer.playerID)
            {
                if((counter) < playerOrder.Count -1){
                    activePlayer = playerOrder.ElementAt(counter + 1).Key;
                    GameManager.Instance.UpdateGameState(GameState.ChangeTurn);
                    break;
                }
                Debug.Log("No more players. Start new round");
                activePlayer = playerOrder.ElementAt(0).Key;
                
                GameManager.Instance.UpdateGameState(GameState.ChangeTurn);
            }
            counter++;
        }
    }

    public void StartRound()
    {
        //Set the active player
        activePlayer = playerOrder.ElementAt(0).Key;
        Debug.Log("Player " + activePlayer + " is the active player");
        IncrementRound();
    }

    //Add the player to the list, sort it by highest Rolled number
    public void AddToRoundOrder(int playerId, int initialRoll)
    {
        playerOrder.Add(playerId, initialRoll);

        var ordered = playerOrder.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
        playerOrder = ordered;
        playersLeftInTurn++;
    }

    public void SetPlayersLeftInTurn(int playersLeft)
    {
        playersLeftInTurn = playersLeft;
    }

    public int GetActivePlayer()
    {
        return activePlayer;
    }
}
