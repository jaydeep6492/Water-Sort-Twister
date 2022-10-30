using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TubeData",menuName = "ColorSortTwister/Tube",order = 1)]
public class TubeData : ScriptableObject
{
    public int currentTubeId;
    public List<TubeConfig> tubes;
}

[Serializable]
public class TubeConfig
{
    public Holder tubePrefab;
    public int tubePrice;
    public ItemPurchase itemPurchase;
}

public enum ItemPurchase
{
    Buy,
    Purchased
}