using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameState State;

    public static event Action<GameState> OnGameStateChanged;

    public List<BoardPlayer> activePlayers = new List<BoardPlayer>();

    public List<int> readyToStart = new List<int>();

    public List<Dice> activeDice = new List<Dice>();

    public GameObject dicePrefab;

    public int lastNumberRolled;

    public CameraFollow boardCam;

    public GameObject DialogObject;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        UpdateGameState(GameState.SelectOrder);
    }

    private void Update()
    {
        if (State == GameState.SelectOrder)
        {
            if (readyToStart.Count == activePlayers.Count)
            {
                UpdateGameState(GameState.ActivePlayerTurn);
            }
        }
    }

    public void DestroyPlayerDice(int playerId)
    {
        int count = 0;
        foreach (Dice dice in activeDice)
        {
            if (playerId == dice.GetLinkedPlayerID())
            {

                activeDice.RemoveAt(count);
                dice.DestroyDice();
                HandleDiceBlock(playerId);
                break;
            }
            count++;
        }
    }

    public void DestroyPlayerDiceAfterTeleport(int playerId)
    {
        int count = 0;
        foreach (Dice dice in activeDice)
        {
            if (playerId == dice.GetLinkedPlayerID())
            {
                activeDice.RemoveAt(count);
                dice.DestroyDice();                
                break;
            }
            count++;
        }
    }

    public void AddDice(Dice dice)
    {
        activeDice.Add(dice);
    }

    public void UpdateGameState(GameState newState, int rolledNumber = 0)
    {
        State = newState;

        switch (newState)
        {
            case GameState.SelectOrder:
                RollForGameOrder();
                UIManager.Instance.ActivatePlayerUI();
                break;
            case GameState.ActivePlayerTurn:
                //Find Active Player
                int activePlayerId = RoundManager.Instance.GetActivePlayer();

                //TODO: Set camera on active player


                //Spawn Dice
                SpawnDice(activePlayerId);
                break;
            case GameState.ItemSelect:
                break;
            case GameState.UseItem:
                break;
            case GameState.MovePlayer:
                BoardPlayer activeBoardPlayer = GetActivePlayer();
                StartCoroutine(activeBoardPlayer.WaitThenMove(rolledNumber));
                break;
            case GameState.SpaceInteraction:
                break;
            case GameState.TrophySpace:
                DialogObject.SetActive(true);
                break;
            case GameState.ChangeTurn:
                UpdateGameState(GameState.ActivePlayerTurn);

                //Not sure why it had to be named this way
                BoardPlayer activeBoardPlayer2 = GetActivePlayer();
                if (activeBoardPlayer2.playerItems.Count > 0)
                {
                    //Show UI button
                    UIManager.Instance.ShowInventoryUIButton(true);
                }
                break;
            case GameState.EndRound:
                break;
            case GameState.EndGame:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }

        OnGameStateChanged?.Invoke(newState);
    }

    private int RollForGameOrder()
    {
        int diceSpawned = 0;
        foreach (BoardPlayer player in activePlayers)
        {
            SpawnDice(player.playerID);
            diceSpawned++;
        }
        return diceSpawned;
    }

    private void SpawnDice(int playerID)
    {
        var diceBlock = GameObject.Instantiate(dicePrefab, GetSpawnPosition(playerID), Quaternion.identity);
        diceBlock.GetComponent<Dice>().SetPlayerId(playerID);
    }


    public void HandleDiceBlock(int playerID)
    {
        bool isPlayerFound = false;

        int rolledNumber = GenerateRandomNumberForDice();
        Debug.Log("Player " + playerID + " Rolled: " + rolledNumber);

        if (State.Equals(GameState.SelectOrder))
        {
            if (readyToStart.Count == 0)
            {
                readyToStart.Add(playerID);
                RoundManager.Instance.AddToRoundOrder(playerID, rolledNumber);
            }
            else
            {
                foreach (int activePlayer in readyToStart)
                {
                    if (activePlayer == playerID)
                    {
                        isPlayerFound = true;
                        break;
                    }
                }
                if (!isPlayerFound)
                {
                    RoundManager.Instance.AddToRoundOrder(playerID, rolledNumber);
                    readyToStart.Add(playerID);
                }
            }

            //Start Round 1 on call that is the last one to add player to order list
            if (readyToStart.Count == activePlayers.Count)
            {
                Debug.Log("Starting Round 1");
                RoundManager.Instance.StartRound();

                UpdateGameState(GameState.ActivePlayerTurn);
            }
            //Do not start round until indicated that everyone has rolled;
            else
            {
                int playersLeft = readyToStart.Count - activePlayers.Count;
                Debug.Log("Waiting on " + playersLeft + "Players");
            }
        }
        else if (State.Equals(GameState.ActivePlayerTurn))
        {
            UpdateGameState(GameState.MovePlayer, rolledNumber);
        }
    }

    private Vector3 GetSpawnPosition(int playerID)
    {
        Vector3 playerPosition = new Vector3(0, 0, 0);
        Vector3 spawnPosition = new Vector3(0, 0, 0);
        foreach (BoardPlayer player in activePlayers)
        {
            if (playerID == player.playerID)
            {
                playerPosition = player.transform.position;
                spawnPosition = new Vector3(playerPosition.x, playerPosition.y + 4f, playerPosition.z);
            }
        }

        return spawnPosition;
    }

    public BoardPlayer GetActivePlayer()
    {
        BoardPlayer activeBoardPlayer = null;

        foreach (BoardPlayer player in activePlayers)
        {
            if (player.GetPlayerID() == RoundManager.Instance.GetActivePlayer())
            {
                activeBoardPlayer = player;
                break;
            }
        }
        return activeBoardPlayer;
    }

    public BoardPlayer GetPlayerByName(string playerToAffect)
    {
        foreach (BoardPlayer player in activePlayers)
        {
            if (player.playerName == playerToAffect)
            {
                return player;
            }
        }

        Debug.Log(playerToAffect + " was not found");
        return null;
    }

    public int GenerateRandomNumberForDice()
    {
        BoardPlayer activePlayer = GetActivePlayer();

        if (GameManager.Instance.GetState() == GameState.SelectOrder)
        {
            System.Random rand = new System.Random();
            return rand.Next(1, 10);
        }
        else
        {
            //Player was hit with a curse block
            if (activePlayer.IsPlayerCursed())
            {
                activePlayer.RemoveCurse();

                System.Random rand = new System.Random();
                return rand.Next(1, 5);
            }
            else
            {
                System.Random rand = new System.Random();
                return rand.Next(1, 10);
            }
        }
    }

    /// <summary>
    /// Method that can be used to add a player, may no longer need in the future
    /// </summary>
    /// <param name="player"></param>
    public void AddPlayer(BoardPlayer player)
    {
        bool isPlayerFound = false;
        if (activePlayers.Count == 0)
        {
            activePlayers.Add(player);
        }
        else
        {
            //Check if player already exists
            foreach (BoardPlayer existingPlayer in activePlayers)
            {
                if (existingPlayer.GetPlayerID() == player.playerID)
                {
                    isPlayerFound = true;
                    break;
                }
            }

            //Do not add existing player
            if (isPlayerFound)
            {
                Debug.Log("Cannot add existing character");
            }
            else
            {
                activePlayers.Add(player);
            }
        }
    }

    public GameState GetState()
    {
        return State;
    }

    public void ActivateDialog()
    {
        DialogObject.SetActive(true);
    }
}

public enum GameState
{
    SelectOrder,
    ActivePlayerTurn,
    ItemSelect,
    UseItem,
    MovePlayer,
    SpaceInteraction,
    TrophySpace,
    ChangeTurn,
    EndRound,
    EndGame
}