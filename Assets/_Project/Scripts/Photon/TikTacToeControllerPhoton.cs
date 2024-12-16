using System;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TikTacToeControllerPhoton : MonoBehaviour
{
    public Button[] buttons;
    private string[] board = new string[9];
    private bool gameOver = false;
    public ulong currentPlayerId = ulong.MaxValue;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI gameOverText;
    private string yourText = "X";
    private string opponentText = "O";
    PhotonView photonView;
    int actorNumber;
    public bool yourTurn = false;

    private readonly int[,] winPatterns = new int[,]
    {
        { 0, 1, 2 }, { 3, 4, 5 }, { 6, 7, 8 }, // Rows
        { 0, 3, 6 }, { 1, 4, 7 }, { 2, 5, 8 }, // Columns
        { 0, 4, 8 }, { 2, 4, 6 } // Diagonals
    };
    private void Start()
    {
        photonView = GetComponent<PhotonView>();
        actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
        yourText = PhotonNetwork.IsMasterClient ? "X" : "O";
        opponentText = PhotonNetwork.IsMasterClient ? "O" : "X";
        yourTurn = PhotonNetwork.IsMasterClient;

        for (int i = 0; i < buttons.Length; i++)
        {
            int index = i;
            buttons[i].onClick.AddListener(() =>
                photonView.RPC("OnCellClickServerRpc", RpcTarget.All, index, PhotonNetwork.LocalPlayer.ActorNumber));
            board[i] = "";
        }
    }

    private void Update()
    {
        if (gameOver && Input.GetKeyDown(KeyCode.Space))
        {
            LeaveRoom();
        }
    }

    [PunRPC]
    private void OnCellClickServerRpc(int index, int id)
    {
        if (gameOver || board[index] != "") return;
        
        if(actorNumber == id && !yourTurn || actorNumber != id && yourTurn)
            return;

        UpdateBoard(index, id);
        CheckWinner();
    }

    private void UpdateBoard(int index, int id)
    {
        if (board[index] != "")
            return;
        yourTurn = !yourTurn;
        GUIManagerPhoton.Instance.SetStatusText(yourTurn ? "Your turn" : "Opponent's turn");
        var symbol = actorNumber == id ? yourText : opponentText;
        board[index] = symbol;
        buttons[index].GetComponentInChildren<TextMeshProUGUI>().text = symbol;
    }

    void CheckWinner()
    {
        for (int i = 0; i < winPatterns.GetLength(0); i++)
        {
            if (board[winPatterns[i, 0]] == board[winPatterns[i, 1]] &&
                board[winPatterns[i, 1]] == board[winPatterns[i, 2]] &&
                board[winPatterns[i, 0]] != "")
            {
                gameOver = true;
                gameOverPanel.SetActive(true);
                if (board[winPatterns[i, 0]] == yourText)
                {
                    gameOverText.text = "Winner";
                }
                else
                {
                    gameOverText.text = "Loser";
                }

                return;
            }
        }

        bool isDraw = true;
        foreach (string cell in board)
        {
            if (cell == "")
            {
                isDraw = false;
                break;
            }
        }

        if (isDraw)
        {
            gameOver = true;
            gameOverPanel.SetActive(true);
            gameOverText.text = "Draw";
        }
    }

    void LeaveRoom()
    {
        ResetGame();
        PhotonNetwork.LeaveRoom();
    }

    void ResetGame()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].GetComponentInChildren<TextMeshProUGUI>().text = "";
            board[i] = "";
        }

        gameOver = false;
        gameOverPanel.SetActive(false);
    }
}