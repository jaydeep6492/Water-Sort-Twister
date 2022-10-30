using System.Collections;
using System.Collections.Generic;
using Game;
using dotmob;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelCompletePanel : ShowHidable
{
    [SerializeField] private Text _toastTxt;
    [SerializeField] private Text _levelTxt;
    [SerializeField]private List<string> _toasts = new List<string>();

    public int rewardCoin = 50;
    public TMP_Text coinText;
    public GameObject coin;

    private void Awake()
    {
        _levelTxt.text = $" Level {LevelManager.Instance.Level.no}";
        GameManager.Instance.OnVibrate();
    }

    protected override void OnShowCompleted()
    {
        base.OnShowCompleted();
        _toastTxt.text = _toasts.GetRandom();
        _toastTxt.gameObject.SetActive(true);
        AdsManager.ShowOrPassAdsIfCan();
    }


    public void OnClickContinue()
    {
        UIManager.Instance.LoadNextLevel();
    }

    public void OnClickMainMenu()
    {
        GameManager.LoadScene("MainMenu");
        SharedUIManager.PausePanel.Hide();
    }
}