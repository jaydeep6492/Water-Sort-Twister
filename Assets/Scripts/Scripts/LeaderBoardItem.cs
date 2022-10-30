using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LeaderBoardItem : MonoBehaviour
{
   #region Public Fields

   public TMP_Text playerName;
   public TMP_Text playerRank;
   public TMP_Text playerScore;

   public Image playerProfile;
   #endregion

   #region Public Methods

   public void InitItem(string playerName,string rank,string score)
   {
      this.playerName.text = playerName;
      playerRank.text = rank;
      playerScore.text = score;
   }
   #endregion
}
