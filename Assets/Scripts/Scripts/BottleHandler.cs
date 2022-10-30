using System.Collections.Generic;
using dotmob;
using MainMenu;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BottleHandler : MonoBehaviour
{
    #region Public Fields

    public TMP_Text buttonText;
    public Button buttonConfirm;

    #endregion

    #region Private Fields

    private int _selectedIndex;

    #endregion

    #region Unity Callbacks

    private void Awake()
    {
        OnChangeBottle(0);
    }

    private void OnEnable()
    {
        if (!PrefManager.HasKey("OwnBottles")) return;
        var arr = JsonConvert.DeserializeObject<List<int>>(PrefManager.GetString("OwnBottles"));
        foreach (var index in arr)
        {
            GameManager.Instance.tubeData.tubes[index].itemPurchase = ItemPurchase.Purchased;
        }
    }

    #endregion

    #region Public Methods

    public void OnChangeBottle(int index)
    {
        _selectedIndex = index;
        if (_selectedIndex == GameManager.Instance.bottleIndex)
        {
            buttonText.text = "Selected";
            buttonConfirm.interactable = false;
        }
        else if (GameManager.Instance.tubeData.tubes[_selectedIndex].itemPurchase == ItemPurchase.Purchased)
        {
            buttonConfirm.interactable = true;
            buttonText.text = "Select";
        }
        else
        {
            buttonConfirm.interactable = true;
            buttonText.text = GameManager.Instance.tubeData.tubes[_selectedIndex].tubePrice.ToString();
        }
    }

    public void OnConfirm()
    {
        if (GameManager.Instance.tubeData.tubes[_selectedIndex].itemPurchase == ItemPurchase.Purchased)
        {
            GameManager.Instance.holder = GameManager.Instance.tubeData.tubes[_selectedIndex].tubePrefab;
            GameManager.Instance.bottleIndex = _selectedIndex;
            buttonText.text = "Selected";
            buttonConfirm.interactable = false;
            UIManager.Instance.OnShowMessage("Bottle Selected");
        }
        else
        {
            if (GameManager.GOLD_COIN >= GameManager.Instance.tubeData.tubes[_selectedIndex].tubePrice)
            {
                GameManager.GOLD_COIN -= GameManager.Instance.tubeData.tubes[_selectedIndex].tubePrice;
                GameManager.Instance.tubeData.tubes[_selectedIndex].itemPurchase = ItemPurchase.Purchased;
                GameManager.Instance.bottleIndex = _selectedIndex;
                buttonText.text = "Selected";
                buttonConfirm.interactable = false;
                if (PrefManager.HasKey("OwnBottles"))
                {
                    var arr = JsonConvert.DeserializeObject<List<int>>(PrefManager.GetString("OwnBottles"));
                    arr.Add(_selectedIndex);
                    PrefManager.SetString("OwnBottles", JsonConvert.SerializeObject(arr));

                }
                else
                {
                    var arr = new List<int> {_selectedIndex};
                    PrefManager.SetString("OwnBottles", JsonConvert.SerializeObject(arr));
                }
                GameManager.Instance.holder = GameManager.Instance.tubeData.tubes[_selectedIndex].tubePrefab;
                UIManager.Instance.OnShowMessage("Bottle Purchased");

            }
            else
            {
                UIManager.Instance.OnShowMessage("Not Enough Gold To Buy!!!");
                Debug.Log("Not Enough Money!!!");
            }
        }

        UIManager.Instance.PopupUtils.Hide();
    }

    #endregion

}
