using System.Collections;
using System.Collections.Generic;
using MainMenu;
using UnityEngine;
using UnityEngine.Purchasing;

public class RemoveAds : ShowHidable
{

    #region Public Methods

    public void OnPurchase(Product product)
    {
        ResourceManager.EnableAds = false;
        UIManager.Instance.OnShowMessage("Purchased Successfully");
    }

    #endregion
}
