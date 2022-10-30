using System;
using System.Collections.Generic;
using dotmob;
using UnityEngine;

public class PopupUtils : ShowHidable
{
    public List<GameObject> popups;
    public GameObject btnMuteVibrate;
    private int currentIndex;
    public override void Show(params object[] args)
    {
        if (!(args[0] is int index)) return;
        currentIndex = index;
        for (var i = 0; i < popups.Count; i++)
        {
            popups[i].SetActive(i==currentIndex);
        }
        base.Show(args);
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        btnMuteVibrate.SetActive(!GameManager.GAME_VIBRATION);
    }

    public override void Hide(bool animate = true, Action completed = null)
    {
        Debug.Log("Hide");
        popups[currentIndex].SetActive(false);
        base.Hide(animate, completed);
    }

    public void OnPrivacyPolicy()
    {
        SharedUIManager.ConsentPanel.Show();
        Hide();
    }

    public void OnVibration(bool vibrate)
    {
        GameManager.GAME_VIBRATION = vibrate;
    }
}
