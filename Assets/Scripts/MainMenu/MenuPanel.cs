using System;
using dotmob;
using TMPro;
using UnityEngine;

namespace MainMenu
{
    public class MenuPanel : MonoBehaviour
    {
        #region Public Fields

        public TMP_Text goldCoin;
        public TMP_Text gemCoin;

        #endregion

        #region Unity Callbacks

        private void OnEnable()
        {
           SetCurrency();
        }

        private void Start()
        {
            GameManager.OnChangeCurrency.AddListener(SetCurrency);
        }

        private void OnDisable()
        {
            GameManager.OnChangeCurrency.RemoveListener(SetCurrency);
        }

        #endregion

        #region Private Methods

        private void SetCurrency()
        {
            goldCoin.text = GameManager.GOLD_COIN.ToString();
            gemCoin.text = GameManager.GEM_COIN.ToString();
        }

        #endregion

        #region Public Methods

        public void OnSetting()
        {
            UIManager.Instance.PopupUtils.Show(0);
        }

        public void OnLeaderBoard()
        {
            UIManager.Instance.PopupUtils.Show(1);
        }

        public void OnTube()
        {
            UIManager.Instance.PopupUtils.Show(2);
        }

        public void OnShop()
        {
            UIManager.Instance.ShopView.Show();
        }

        public void OnClickPlay()
        {
            var levelsPanel = UIManager.Instance.LevelsPanel;
            levelsPanel.GameMode = (GameMode)1;
            levelsPanel.Show();
            //UIManager.Instance.GameModePanel.Show();
        }

        public void OnRemoveAds()
        {
            SharedUIManager.RemoveAdPanel.Show();
        }
        public void OnDailyChallenge()
        {
            UIManager.Instance.PopupUtils.Show(3);
        }
        public void OnClickExit()
        {

        }

        public void OnAddCoin()
        {
            UIManager.Instance.ShopView.Show(2);
        }

        public void OnAddDiamond()
        {
            UIManager.Instance.ShopView.Show(1);
        }
        #endregion
    }
}