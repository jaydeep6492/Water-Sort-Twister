using System;
using System.Collections.Generic;
using dotmob;
using MainMenu;
using TMPro;
using UnityEngine;

public class LeaderBoardHandler : MonoBehaviour
{
    #region Public Fields

    public TMP_Text firstScore;
    public TMP_Text secondScore;
    public TMP_Text thirdScore;
    public LeaderBoardItem[] leaderBoardItems;

    #endregion
    
    #region Private Fields
    private int _playerIndex;
    #endregion
    
    #region Unity Callbacks

    private void OnEnable()
    {
        var leaderBoard = GameManager.Instance.leaderBoard;
        Debug.Log(leaderBoard == null ? "Nulled" : "Not Null");
        firstScore.text = leaderBoard.firstScore.ToString();
        secondScore.text = leaderBoard.secondScore.ToString();
        thirdScore.text = leaderBoard.thirdScore.ToString();
        for (int i = 0; i < leaderBoardItems.Length; i++)
        {
            if (leaderBoard.playerNames[i] == GameManager.PLAYER_NAME)
            {
                _playerIndex = i;
            }
            leaderBoardItems[i].InitItem(leaderBoard.playerNames[i], leaderBoard.ranks[i], leaderBoard.scores[i]);
        }
    }

    #endregion
    
    #region Public Methods

    public void OnClose()
    {
        UIManager.Instance.PopupUtils.Hide();
    }
    #endregion
}

[Serializable]
public class LeaderBoardConfig
{
    public int firstScore;
    public int secondScore;
    public int thirdScore;
    public List<string> playerNames;
    public List<string> scores;
    public List<string> ranks;
    public string date;
}