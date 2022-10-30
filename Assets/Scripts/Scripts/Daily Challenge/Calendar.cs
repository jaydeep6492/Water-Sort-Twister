using UnityEngine;
using System;
using TMPro;

public class Calendar : MonoBehaviour
{
    /// <summary>
    /// Setup in editor since there will always be six weeks. 
    /// Try to figure out why it must be six weeks even though at most there are only 31 days in a month
    /// </summary>
    public ChallengeWeek[] weeks;

    /// <summary>
    /// This is the text object that displays the current month and year
    /// </summary>
    public TMP_Text MonthAndYear;

    /// <summary>
    /// In start we set the Calendar to the current date
    /// </summary>
    private void Start()
    {
        UpdateCalendar(DateTime.Now.Year, DateTime.Now.Month);
    }

    /// <summary>
    /// Anytime the Calendar is changed we call this to make sure we have the right days for the right month/year
    /// </summary>
    void UpdateCalendar(int year, int month)
    {
        DateTime temp = new DateTime(year, month, 1);
        MonthAndYear.text = temp.ToString("MMMM") + "," + temp.Year;
        var startDay = GetMonthStartDay(year, month);
        var endDay = GetTotalNumberOfDays(year, month);
        for (var w = 0; w < 6; w++)
        {
            for (var i = 0; i < 7; i++)
            {
                var currDay = (w * 7) + i;
                if (currDay - startDay < 0 || currDay - startDay >= endDay) continue;
                weeks[w].challengeDays[i].InitDay(currDay - startDay +1,month);
            }
        }

    }

    /// <summary>
    /// This returns which day of the week the month is starting on
    /// </summary>
    int GetMonthStartDay(int year, int month)
    {
        DateTime temp = new DateTime(year, month, 1);

        //DayOfWeek Sunday == 0, Saturday == 6 etc.
        return (int)temp.DayOfWeek;
    }

    /// <summary>
    /// Gets the number of days in the given month.
    /// </summary>
    int GetTotalNumberOfDays(int year, int month)
    {
        return DateTime.DaysInMonth(year, month);
    }
}