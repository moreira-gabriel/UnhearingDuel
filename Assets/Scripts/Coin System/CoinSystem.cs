using UnityEngine;
using TMPro;

public class CoinSystem : MonoBehaviour
{
    public int coinCount {get; private set;}
    [SerializeField] private TMP_Text coinText; 
    void Start()
    {
        if(PlayerPrefs.GetInt("Coins") == 0) PlayerPrefs.SetInt("Coins", 0);
        else coinCount = PlayerPrefs.GetInt("Coins");
        UpdateText();
    }

    public void UpdateText()
    {
        coinText.text = coinCount.ToString();
    }

    public void AddCoins(int amount)
    {
        coinCount += amount;
        PlayerPrefs.SetInt("Coins", coinCount);
    }

    public void RemoveCoins(int amount)
    {
        coinCount -= amount;
        PlayerPrefs.SetInt("Coins", coinCount);        
    }
    
}
