using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class QuestionDialogUI : MonoBehaviour
{
    public TextMeshProUGUI textMeshPro;
    public Button yesBtn;
    public Button noBtn;

    private void Awake()
    {

        textMeshPro = transform.Find("DialogText").GetComponent<TextMeshProUGUI>();
        yesBtn = transform.Find("YesButton").GetComponent<Button>();
        noBtn = transform.Find("NoButton").GetComponent<Button>();

        ShowQuestion("Do you want to buy a Jar of Golden Toe Nails? for 20 coins?", () =>
        {
            Debug.Log(GameManager.Instance.GetActivePlayer().playerName + "Says Yes");

            //Congrats transition
            AddJarDeductCoins();
            GameManager.Instance.UpdateGameState(GameState.MovePlayer, GameManager.Instance.GetActivePlayer().pointsToMove);
            GameManager.Instance.DialogObject.SetActive(false);

        }, () =>
        {

            Debug.Log(GameManager.Instance.GetActivePlayer().playerName + "Says No");
            GameManager.Instance.UpdateGameState(GameState.MovePlayer, GameManager.Instance.GetActivePlayer().pointsToMove);
            GameManager.Instance.DialogObject.SetActive(false);
        });
    }

    private void Update()
    {
        BoardPlayer player = GameManager.Instance.GetActivePlayer();
        if (player.coins < 20)
        {
            yesBtn.gameObject.SetActive(false);
        }
        else if(player.coins > 20)
        {
            yesBtn.gameObject.SetActive(true);
        }
    }

    private void ShowQuestion(string questionText, Action yesAction, Action noAction)
    {
        if (GameManager.Instance.GetActivePlayer().coins > 20)
        {
            GameObject yesButton = yesBtn.gameObject;

            yesButton.SetActive(false);
        }
        textMeshPro.text = questionText;
        yesBtn.onClick.AddListener(new UnityEngine.Events.UnityAction(yesAction));
        noBtn.onClick.AddListener(new UnityEngine.Events.UnityAction(noAction));
    }

    public void AddJarDeductCoins()
    {
        BoardPlayer activePlayer = GameManager.Instance.GetActivePlayer();
        activePlayer.gems++;
        activePlayer.coins -= 20;



        UIManager.Instance.UpdateCoins(activePlayer);
        UIManager.Instance.UpdateGems(activePlayer);
    }
}
