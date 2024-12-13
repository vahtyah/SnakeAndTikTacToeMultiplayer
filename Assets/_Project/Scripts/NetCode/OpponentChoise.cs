using UnityEngine;
using UnityEngine.UI;

public class OpponentChoise : MonoBehaviour
{
    [SerializeField] private Sprite rockSprite;
    [SerializeField] private Sprite paperSprite;
    [SerializeField] private Sprite scissorsSprite;
    
    
    Image image;
    
    private void Start()
    {
        image = GetComponent<Image>();
        image.enabled = false;
    }
    
    public void SetOpponentChoice(Choice choice)
    {
        image.enabled = true;
        switch (choice)
        {
            case Choice.Rock:
                image.sprite = rockSprite;
                break;
            case Choice.Paper:
                image.sprite = paperSprite;
                break;
            case Choice.Scissors:
                image.sprite = scissorsSprite;
                break;
        }
    }
    
    public void Reset()
    {
        image.enabled = false;
    }
}
