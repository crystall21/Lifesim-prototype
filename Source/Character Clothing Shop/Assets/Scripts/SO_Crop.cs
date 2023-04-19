using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GrowingStageSprite
{
	[HideInInspector]
	public string  name;
	public float time;
	public Sprite sprite;

	public void Validate(int stage)
	{

		name = "Stage " + stage + " @ " + time + "s";
	}
}


[CreateAssetMenu(menuName = "SO/Crop")]
public class SO_Crop : ScriptableObject
{
	public Farming_Crop cropType;
	public List<GrowingStageSprite> stageSprites = new List<GrowingStageSprite>();
	 public SO_Item itemDrop,seedDrop;

	public KeyValuePair<int,Sprite> getStage(float time)
	{
		Sprite result = stageSprites[0].sprite;
		int current_stage = 0;
		for(int i = 0; i < stageSprites.Count; i++)
		{
			if(time >= stageSprites[i].time)
			{
				result = stageSprites[i].sprite;
				current_stage = i + 1;
			}
		}
		return new KeyValuePair<int, Sprite>(current_stage,result);
	}

	void OnValidate()
	{
		stageSprites.ForEach(o => o.Validate(stageSprites.IndexOf(o)+1));
	}


}
