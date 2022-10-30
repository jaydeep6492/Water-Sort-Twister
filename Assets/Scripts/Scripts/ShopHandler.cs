using dotmob;
using MainMenu;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;
using Product = UnityEngine.Purchasing.Product;

public class ShopHandler : ShowHidable
{
    #region Public Fields
    public GameObject[] selectedShop;
    public Toggle[] selectedTabs;
    #endregion

    #region Unity callbacks
    
    #endregion
    
    #region Public Methods

    public void OnChangeItem(int index)
    {
        for (var i = 0; i < selectedShop.Length; i++)
        {
            selectedShop[i].SetActive(i==index);
        }
    }

    public void OnPurchase(Product product)
    {
        foreach (var item in product.definition.payouts)
        {
            switch (item.type)
            {
                case PayoutType.Currency:
                    switch (item.subtype)
                    {
                        case "Gold":
                            GameManager.GOLD_COIN += (int)item.quantity;
                            break;
                        case "Diamond":
                            GameManager.GEM_COIN += (int)item.quantity;
                            break;
                    }

                    break;
                case PayoutType.Item:
                    switch (item.subtype)
                    {
                        case "Bottle":
                            Debug.Log("called");
                            UIManager.Instance.PopupUtils.Show(2);
                            break;
                        case "Hint":
                            GameManager.HINT_COUNT += (int) item.quantity;
                            break;
                        case "Undo":
                            GameManager.UNDO_COUNT += (int) item.quantity;
                            break;
                    }
                    break;
            }
        }
        UIManager.Instance.OnShowMessage("Item Purchased Successfully!!");
    }

    public void OnPurchaseFail()
    {
        UIManager.Instance.OnShowMessage("Something Wrong Try Again Later!!!");
    }
    public void OnPurchaseFail(Product product, PurchaseFailureReason result)
    {
        UIManager.Instance.OnShowMessage($"Failed :::: {result}");
    }
    public void OnPurchase(int index)
    {
        var purchased = false;
        var item = GameManager.Instance.inventoryData.items[index];
        if (GameManager.GEM_COIN >= item.price)
        {
            switch (item.itemName)
            {
                case "Gold":
                    GameManager.GOLD_COIN += item.quantity;
                    purchased = true;
                    break;
                case "Hint":
                    GameManager.HINT_COUNT += item.quantity;
                    purchased = true;
                    break;
                case "Undo":
                    GameManager.UNDO_COUNT += item.quantity;
                    purchased = true;
                    break;
                case "Bottle":
                    UIManager.Instance.PopupUtils.Show(2);
                    break;
            }
            if (!purchased) return;
            GameManager.GEM_COIN -= item.price;
            UIManager.Instance.OnShowMessage("Item Purchased Successfully!!!");
        }
        else
        {
            UIManager.Instance.OnShowMessage("Not Enough Diamonds");
        }
    }
    #endregion

    #region Iherited Methods

    public override void Show(params object[] args)
    {
        base.Show(args);
        if (!(args[0] is int index)) return;
        selectedTabs[index].isOn = true;
        OnChangeItem(index);
    }

    #endregion
}
