using System;
using TMPro;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public enum Choice
{
    None,
    Rock,
    Paper,
    Scissors
}

public class RPSController : NetworkBehaviour
{
    public Button rockButton;
    public Button paperButton;
    public Button scissorsButton;
    public TextMeshProUGUI statusText;
    public TextMeshProUGUI resultText;

    public OpponentChoise opponentChoiseImage;

    public Choice yourChoice;
    public Choice opponentChoice;

    private bool isGameOver;

    private void Start()
    {
        if (IsHost)
        {
            statusText.text = "Waiting for Player...";
        }
        resultText.text = "";
        statusText.text = "";

        NetworkManager.Singleton.OnClientConnectedCallback += clientId => { statusText.text = "Your Turn..."; };


        // Assign button listeners
        rockButton.onClick.AddListener(() =>
            OnPlayerChoiceServerRpc(Choice.Rock, NetworkManager.Singleton.LocalClientId));
        paperButton.onClick.AddListener(() =>
            OnPlayerChoiceServerRpc(Choice.Paper, NetworkManager.Singleton.LocalClientId));
        scissorsButton.onClick.AddListener(() =>
            OnPlayerChoiceServerRpc(Choice.Scissors, NetworkManager.Singleton.LocalClientId));
    }

    private void Update()
    {
        if (isGameOver && Input.GetKeyDown(KeyCode.Space))
        {
            ResetGameServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void OnPlayerChoiceServerRpc(Choice choice, ulong clientId)
    {
        if (isGameOver) return;
        SendChoiceToOpponentClientRpc(choice, clientId);
    }

    [ClientRpc]
    public void SendChoiceToOpponentClientRpc(Choice choice, ulong clientId)
    {
        Debug.Log("Client " + clientId + " NetworkManager.Singleton.LocalClientId " +
                  NetworkManager.Singleton.LocalClientId);
        if (clientId == NetworkManager.Singleton.LocalClientId)
        {
            yourChoice = choice;
            statusText.text = "Opponent's Turn...";
        }
        else
        {
            opponentChoice = choice;
        }

        CheckWinner();
    }

    private void CheckWinner()
    {
        if (yourChoice == Choice.None || opponentChoice == Choice.None)
        {
            return;
        }

        // Logic to determine the winner

        opponentChoiseImage.SetOpponentChoice(opponentChoice);
        if (yourChoice == opponentChoice)
        {
            resultText.text = "It's a Draw!";
        }
        else if ((yourChoice == Choice.Rock && opponentChoice == Choice.Scissors) ||
                 (yourChoice == Choice.Paper && opponentChoice == Choice.Rock) ||
                 (yourChoice == Choice.Scissors && opponentChoice == Choice.Paper))
        {
            resultText.text = "You Win!";
        }
        else
        {
            resultText.text = "You Lose!";
        }
        
        resultText.text += " \nPress Space to Play Again!";

        statusText.text = "Game Over!";
        isGameOver = true;

        if (yourChoice == Choice.Rock)
        {
            scissorsButton.interactable = false;
            paperButton.interactable = false;
        }
        else if (yourChoice == Choice.Paper)
        {
            rockButton.interactable = false;
            scissorsButton.interactable = false;
        }
        else if (yourChoice == Choice.Scissors)
        {
            rockButton.interactable = false;
            paperButton.interactable = false;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void ResetGameServerRpc()
    {
        ResetGameClientRpc();
    }

    [ClientRpc]
    private void ResetGameClientRpc()
    {
        yourChoice = Choice.None;
        opponentChoice = Choice.None;
        opponentChoiseImage.Reset();
        resultText.text = "";
        statusText.text = "Your Turn...";
        isGameOver = false;

        rockButton.interactable = true;
        paperButton.interactable = true;
        scissorsButton.interactable = true;
    }
}