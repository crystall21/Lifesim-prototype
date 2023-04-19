using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ShopItem
{
	public SO_Item item;
	public int price;
}


[CreateAssetMenu(menuName = "SO/Shop")]
public class SO_Shop : ScriptableObject
{
	public string shop_Name;
	public List<ShopItem> shop_items = new List<ShopItem>();

}
