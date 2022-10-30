using System;
using TMPro;
using UnityEngine;

public class ChallengeDay : MonoBehaviour
{
    #region Public Fields

    public TMP_Text currentDate;
    public TMP_Text completeDate;
    public TMP_Text activeDate;

    public GameObject activeDay;
    public GameObject completeDay;
    
    #endregion
    
    #region Public Methods

    public void InitDay(int date,int month)
    {
        currentDate.text = date.ToString();
        completeDate.text = date.ToString();
        activeDate.text = date.ToString();
        if (date < DateTime.Now.Day && month == DateTime.Now.Month)
        {
            completeDay.SetActive(true);
        }
        else if (date == DateTime.Now.Day && month == DateTime.Now.Month)
        {
            activeDay.SetActive(true);
        }
    }
    #endregion
}
