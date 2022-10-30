using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "InventoryItems",menuName = "ColorSortTwister/InventoryItems",order = 1)]
public class InventoryData : ScriptableObject
{
    public List<InventoryConfig> items;
}
[Serializable]
public class InventoryConfig
{
   public string itemName;
   public CurrencyType currencyType;
   public int price;
   public int quantity;
}

public enum CurrencyType
{
   GD,DM
}