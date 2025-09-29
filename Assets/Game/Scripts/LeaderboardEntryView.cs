using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LeaderboardEntryView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI usernameText;
    [SerializeField] private TextMeshProUGUI userIdText;
    [SerializeField] private TextMeshProUGUI scoreText;
    
    public void Init(string username, string userId, int score)
    {
        usernameText.text = $"Username: {username}";
        userIdText.text = $"User Id: {userId}";
        scoreText.text = score.ToString();
    }
}
