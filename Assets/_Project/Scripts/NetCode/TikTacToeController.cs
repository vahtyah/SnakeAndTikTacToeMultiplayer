using System;
using TMPro;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TikTacToeController : NetworkBehaviour, IEventListener<GameEvent>
{
    public Button[] buttons;
    private string[] board = new string[9];
    public string currentText = "X";
    private bool gameOver = false;
    public ulong currentPlayerId = ulong.MaxValue;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI gameOverText;


    private void Start()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        for (int i = 0; i < buttons.Length; i++)
        {
            int index = i; // Tạo index để tránh lỗi closure trong lambda expression
            buttons[i].onClick.AddListener(() => OnCellClickServerRpc(index, NetworkManager.Singleton.LocalClientId));
            board[i] = ""; // Khởi tạo trạng thái các ô là trống
        }
    }

    private void Update()
    {
        if(gameOver && Input.GetKeyDown(KeyCode.Space))
        {
            NetworkManager.Singleton.Shutdown();
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void OnCellClickServerRpc(int index, ulong id)
    {
        if (gameOver || board[index] != "") return; // Nếu game đã kết thúc hoặc ô đã được chọn
        UpdateBoardClientRpc(index, id); // Gọi ClientRpc từ Server
        CheckWinnerClientRpc(); // Kiểm tra người thắng
    }
    [ClientRpc]
    void CheckWinnerClientRpc()
    {
        // Các chỉ số có thể tạo thành một hàng, cột hoặc chéo
        int[,] winPatterns = new int[,]
        {
            { 0, 1, 2 }, { 3, 4, 5 }, { 6, 7, 8 }, // Hàng
            { 0, 3, 6 }, { 1, 4, 7 }, { 2, 5, 8 }, // Cột
            { 0, 4, 8 }, { 2, 4, 6 } // Chéo
        };

        for (int i = 0; i < winPatterns.GetLength(0); i++)
        {
            // Kiểm tra nếu ba ô liên tiếp có cùng giá trị
            if (board[winPatterns[i, 0]] == board[winPatterns[i, 1]] &&
                board[winPatterns[i, 1]] == board[winPatterns[i, 2]] &&
                board[winPatterns[i, 0]] != "")
            {
                gameOver = true;
                gameOverPanel.SetActive(true);
                if (board[winPatterns[i, 0]] == currentText)
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

        // Kiểm tra hòa
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

    public void OnEvent(GameEvent e)
    {
        switch (e.EventType)
        {
            case GameEventType.GameStart:
                break;
        }
    }

    [ClientRpc]
    public void UpdateBoardClientRpc(int index, ulong id)
    {
        string symbol = (id == currentPlayerId) ? currentText : (currentText == "X" ? "O" : "X");

        Debug.Log(id + " " + currentPlayerId);
        if (board[index] != symbol)
        {
            board[index] = symbol;
            var buttonText = buttons[index].GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText.text != symbol)
            {
                buttonText.text = symbol;
            }

            Debug.Log(buttonText.text);
        }
    }


    private void OnClientConnected(ulong obj)
    {
        if(currentPlayerId != ulong.MaxValue) return;
        if (IsHost)
        {
            currentText = "X";
            currentPlayerId = obj;
        }
        else if (IsClient)
        {
            currentText = "O";
            currentPlayerId = obj;
        }
    }

    private void OnEnable()
    {
        this.StartListening();
    }

    private void OnDisable()
    {
        this.StopListening();
    }
}