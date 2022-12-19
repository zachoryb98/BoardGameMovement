using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using UnityEngine.UI;
using System;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public List<GameObject> PlayerUIObjects = new List<GameObject>();
    public List<GameObject> PlayerSelectionButtons = new List<GameObject>();

    public string selectedItem = "";

    int playerCount = 0;
    Dictionary<BoardPlayer, GameObject> playerUILink = new Dictionary<BoardPlayer, GameObject>();

    [Header("Inventoiry Buttons")]
    public GameObject inventoryButton;
    public GameObject btnCoinSteal;
    public GameObject btnUnluckyDice;
    public GameObject btnTeleport;
    public GameObject btnShield;
    public GameObject btnCloseInventory;


    private void Awake()
    {
        Instance = this;
    }

    public void ShowInventoryUIButton(bool result)
    {
        inventoryButton.SetActive(result);
    }

    public void ActivatePlayerUI()
    {
        foreach (BoardPlayer player in GameManager.Instance.activePlayers)
        {
            PlayerUIObjects[playerCount].SetActive(true);

            GameObject PlayerUIObject = PlayerUIObjects[playerCount].gameObject;

            TextMeshProUGUI playerNameTxt = PlayerUIObject.transform.Find("ActivePlayer").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI playerCoinTxt = PlayerUIObject.transform.Find("CoinsBG").Find("CoinsText").GetComponent<TextMeshProUGUI>();

            playerNameTxt.text = player.playerName;
            playerCoinTxt.text = "Coins: " + player.coins.ToString();

            playerUILink.Add(player, PlayerUIObject);

            playerCount++;
        }
    }

    GameObject FindChildWithTag(GameObject parent, string tag)
    {
        GameObject child = null;


        foreach (Transform transform in parent.transform)
        {
            if (transform.CompareTag(tag))
            {
                child = transform.gameObject;
                break;
            }
        }

        return child;
    }

    public void UpdateCoins(BoardPlayer player)
    {
        foreach (KeyValuePair<BoardPlayer, GameObject> entry in playerUILink)
        {
            if (entry.Key.playerID == player.playerID)
            {
                TextMeshProUGUI playerCoinTxt = entry.Value.transform.Find("CoinsBG").Find("CoinsText").GetComponent<TextMeshProUGUI>();
                playerCoinTxt.text = "Coins: " + player.coins.ToString();
            }
        }
    }

    public void UpdateGems(BoardPlayer player)
    {
        foreach (KeyValuePair<BoardPlayer, GameObject> entry in playerUILink)
        {
            if (entry.Key.playerID == player.playerID)
            {

                var gem1 = entry.Value.transform.Find("GemBG").Find("Gem1").gameObject;
                var gem2 = entry.Value.transform.Find("GemBG").Find("Gem2").gameObject;
                var gem3 = entry.Value.transform.Find("GemBG").Find("Gem3").gameObject;

                int gems = player.gems;

                switch (gems)
                {
                    case 0:
                        gem1.SetActive(true);
                        break;
                    case 1:
                        gem1.SetActive(true);
                        break;
                    case 2:
                        gem2.SetActive(true);
                        break;
                    case 3:
                        gem3.SetActive(true);
                        break;
                    default:
                        //End Game?
                        break;
                }
            }
        }
    }

    public void UpdatePlayerOrder()
    {
        int counter = 0;
        string lastPersonsRank = "";
        BoardPlayer previousPlayer = null;

        Dictionary<string, int> sortedByCoins = new Dictionary<string, int>();
        foreach (KeyValuePair<BoardPlayer, GameObject> entry in playerUILink.OrderByDescending(key => key.Key.coins))
        {

            Debug.Log("player: " + entry.Key.playerName + "Coins: " + entry.Key.coins);
            TextMeshProUGUI playerPositionTxt = entry.Value.transform.Find("PositionBG").Find("PositionText").GetComponent<TextMeshProUGUI>();

            if (counter > 0)
            {
                switch (lastPersonsRank)
                {
                    case "1st":
                        if (previousPlayer.coins == entry.Key.coins)
                        {
                            playerPositionTxt.text = "1st";
                            lastPersonsRank = "1st";
                        }
                        else
                        {
                            playerPositionTxt.text = "2nd";
                            lastPersonsRank = "2nd";
                        }
                        break;
                    case "2nd":
                        if (previousPlayer.coins == entry.Key.coins)
                        {
                            playerPositionTxt.text = "2nd";
                            lastPersonsRank = "2nd";
                        }
                        else
                        {
                            playerPositionTxt.text = "3rd";
                            lastPersonsRank = "3rd";
                        }
                        break;
                    case "3rd":
                        if (previousPlayer.coins == entry.Key.coins)
                        {
                            playerPositionTxt.text = "3rd";
                            lastPersonsRank = "3rd";
                        }
                        else
                        {
                            playerPositionTxt.text = "4th";
                            lastPersonsRank = "4th";
                        }
                        break;
                }
            }
            else
            {
                playerPositionTxt.text = "1st";
                lastPersonsRank = "1st";
            }
            previousPlayer = entry.Key;
            counter++;
        }
    }

    public void OpenInventoryButtonClicked()
    {        
        ShowInventoryUIButton(false);

        //Count of each item
        int coinTransferCount = 0;
        int unluckyDiceCount = 0;
        int positionSwapCount = 0;
        int shieldCount = 0;

        //Set Item select active
        btnCoinSteal.SetActive(true);
        btnUnluckyDice.SetActive(true);
        btnTeleport.SetActive(true);
        btnShield.SetActive(true);
        btnCloseInventory.SetActive(true);

        //Get Active Player Items
        BoardPlayer activePlayer = GameManager.Instance.GetActivePlayer();

        List<PlayerItem> activePlayerInventory = activePlayer.playerItems;

        //Display Inventory UI
        foreach (var item in activePlayerInventory)
        {
            switch (item)
            {
                case PlayerItem.CoinTransfer:
                    coinTransferCount++;
                    break;
                case PlayerItem.CursedDice:
                    unluckyDiceCount++;
                    break;
                case PlayerItem.PositionSwap:
                    positionSwapCount++;
                    break;
                case PlayerItem.Shield:
                    shieldCount++;
                    break;
            }
        }

        //Enable buttons for items
        if (coinTransferCount > 0)
        {
            btnCoinSteal.GetComponent<Button>().interactable = true;
        }
        else
        {
            btnCoinSteal.GetComponent<Button>().interactable = false;
        }

        if (unluckyDiceCount > 0)
        {
            btnUnluckyDice.GetComponent<Button>().interactable = true;
        }
        else
        {
            btnUnluckyDice.GetComponent<Button>().interactable = false;
        }

        if (positionSwapCount > 0)
        {
            btnTeleport.GetComponent<Button>().interactable = true;
        }
        else
        {
            btnTeleport.GetComponent<Button>().interactable = false;
        }

        if (shieldCount > 0)
        {
            btnShield.GetComponent<Button>().interactable = true;
        }
        else
        {
            btnShield.GetComponent<Button>().interactable = false;
        }
    }

    public void CoinTransferSetUpPlayerButtons()
    {
        //Set Item select active
        btnCoinSteal.SetActive(false);
        btnUnluckyDice.SetActive(false);
        btnTeleport.SetActive(false);
        btnShield.SetActive(false);

        selectedItem = "Coin Steal";
        DisplayPlayersAvailableToHit();
    }

    public void CurseBlockSetUpPlayerButtons()
    {
        //Set Item select active
        btnCoinSteal.SetActive(false);
        btnUnluckyDice.SetActive(false);
        btnTeleport.SetActive(false);
        btnShield.SetActive(false);

        selectedItem = "Cursed Block";
        DisplayPlayersAvailableToHit();
    }

    private void DisplayPlayersAvailableToHit()
    {
        //Bring up UI to select players
        if (GameManager.Instance.activePlayers.Count == 4)
        {
            BoardPlayer activePlayer = GameManager.Instance.GetActivePlayer();
            int counter = 0;
            foreach (var button in PlayerSelectionButtons)
            {
                //Don't let player use an item on themself
                if (activePlayer.playerName != GameManager.Instance.activePlayers[counter].playerName)
                {
                    button.GetComponent<Button>().GetComponentInChildren<TextMeshProUGUI>().text = GameManager.Instance.activePlayers[counter].playerName;
                    button.SetActive(true);
                }
                counter++;
            }
        }
    }

    public void UseItemOnPlayer(TextMeshProUGUI playerName)
    {
        string selectedPlayerName = playerName.text;

        BoardPlayer activePlayer = GameManager.Instance.GetActivePlayer();
        BoardPlayer playerToAffect = GameManager.Instance.GetPlayerByName(selectedPlayerName);

        switch (selectedItem)
        {
            case "Coin Steal":                                
                //Deduct
                playerToAffect.coins -= 5;
                UpdateCoins(playerToAffect);

                //Add
                activePlayer.coins += 5;
                UpdateCoins(activePlayer);

                Debug.Log(activePlayer.playerName + " stole 5 coins from " + playerToAffect.playerName);
                break;
            case "Cursed Block":
                playerToAffect.CursePlayer();
                Debug.Log(activePlayer.playerName + " Cursed " + playerToAffect.playerName);
                break;
            default:
                Debug.Log("Item not implemented");
                break;
        }

        foreach (var button in PlayerSelectionButtons)
        {
            button.SetActive(false);
        }

        btnCloseInventory.SetActive(false);
        selectedItem = "";
    }

    public void CloseInventory()
    {
        btnCoinSteal.SetActive(false);
        btnUnluckyDice.SetActive(false);
        btnTeleport.SetActive(false);
        btnShield.SetActive(false);

        foreach (var button in PlayerSelectionButtons)
        {
            button.SetActive(false);
        }

        selectedItem = "";

        btnCloseInventory.SetActive(false);
        inventoryButton.SetActive(true);
    }
}
