using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum item_type
{
	Normal,Seed,Clothing
}
public enum item_clothing_part
{
	Head,Torso,Pants
}


[CreateAssetMenu(menuName = "SO/Item")]
public class SO_Item : ScriptableObject
{

	public int id;
	public string item_name;
	[TextArea]
	public string description;
	public Sprite icon;
	public int selling_price;
	public item_type type;
	public List<AnimationClip> clothing_animations = new List<AnimationClip>(); // 0 - idle, 1 - run
	public item_clothing_part Clothing_part;





}
