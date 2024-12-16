using System;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RPSControllerPhoton : MonoBehaviour
{
    public Button rockButton;
    public Button paperButton;
    public Button scissorsButton;
    
    public TextMeshProUGUI resultText;

    public OpponentChoise opponentChoiseImage;

    public Choice yourChoice;
    public Choice opponentChoice;
    
    private bool isGameOver;
    private PhotonView photonView;
    
    private void Start()
    {
        photonView = GetComponent<PhotonView>();
        resultText.text = "";
        GUIManagerPhoton.Instance.SetStatusText("");
        
        rockButton.onClick.AddListener(() => photonView.RPC("OnPlayerChoiceServerRpc", RpcTarget.All, Choice.Rock, PhotonNetwork.LocalPlayer.ActorNumber));
        paperButton.onClick.AddListener(() => photonView.RPC("OnPlayerChoiceServerRpc", RpcTarget.All, Choice.Paper, PhotonNetwork.LocalPlayer.ActorNumber));
        scissorsButton.onClick.AddListener(() => photonView.RPC("OnPlayerChoiceServerRpc", RpcTarget.All, Choice.Scissors, PhotonNetwork.LocalPlayer.ActorNumber));
    }

    private void Update()
    {
        if (isGameOver && Input.GetKeyDown(KeyCode.Space))
        {
            ResetGame();
            PhotonNetwork.LeaveRoom();
        }
    }

    private void ResetGame()
    {
        yourChoice = Choice.None;
        opponentChoice = Choice.None;
        isGameOver = false;
        resultText.text = "";
        rockButton.interactable = true;
        paperButton.interactable = true;
        scissorsButton.interactable = true;
        opponentChoiseImage.Reset();
    }

    [PunRPC]
    private void OnPlayerChoiceServerRpc(Choice choice, int actorNumber)
    {
        if(isGameOver) return;

        if (PhotonNetwork.LocalPlayer.ActorNumber == actorNumber)
        {
            yourChoice = choice;
            GUIManagerPhoton.Instance.SetStatusText("Waiting for opponent...");
        }
        else
        {
            opponentChoice = choice;
        }
        
        if (yourChoice != Choice.None && opponentChoice != Choice.None)
        {
            isGameOver = true;
            GUIManagerPhoton.Instance.SetStatusText("Press Space to leave room");
            CalculateWinner();
        }
    }

    private void CalculateWinner()
    {
        
        opponentChoiseImage.SetOpponentChoice(opponentChoice);
        if (yourChoice == opponentChoice)
        {
            resultText.text = "Draw";
        }
        else if (yourChoice == Choice.Rock && opponentChoice == Choice.Scissors ||
                 yourChoice == Choice.Paper && opponentChoice == Choice.Rock ||
                 yourChoice == Choice.Scissors && opponentChoice == Choice.Paper)
        {
            resultText.text = "You Win";
        }
        else
        {
            resultText.text = "You Lose";
        }
        
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
}
