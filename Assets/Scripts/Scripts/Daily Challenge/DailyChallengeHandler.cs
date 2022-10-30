using System;
using dotmob;
using TMPro;
using UnityEngine;

public class DailyChallengeHandler : MonoBehaviour
{
    #region Public Fields
    public TMP_Text daysText;
    #endregion

    #region Unity Callbacks

    private void OnEnable()
    {
        daysText.text = $"{DateTime.Now.Day} / {DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month)}";
    }

    #endregion
    #region Public Methods
    public void OnPlayChallenge()
    {
        GameManager.LoadGame(new LoadGameData
        {
            Level = ResourceManager.GetLevel((GameMode) 1, ResourceManager.GetCompletedLevel((GameMode) 1) + 1),
            GameMode = (GameMode) 1
        });
    }
    #endregion
}
