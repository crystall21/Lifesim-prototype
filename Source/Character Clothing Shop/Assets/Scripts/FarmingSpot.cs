using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public enum Farming_Crop
{
    Empty,Carrot,Pumpkin
}
public class FarmingSpot : Interactable
{
    bool ready_to_harvest;
    [SerializeField]
    Farming_Crop crop;

    int growing_stage; // 1 - 5

    [SerializeField]
    float growing_time, current_growing_time;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (ready_to_harvest) return;
        if (crop != Farming_Crop.Empty) {

            if (current_growing_time < growing_time)
            {
                current_growing_time += Time.deltaTime;
                checkGrowingStage();
            }
            else
            {
                ready_to_harvest = true;
            }
        }
    }


    public void checkGrowingStage()
	{
        var returned_stage = GameManager.instance.crops.First(o => o.cropType == crop).getStage(current_growing_time);
        growing_stage = returned_stage.Key;
        transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = returned_stage.Value;
	}



	public override void Execute()
	{
		base.Execute();
		switch (crop)
		{
            case Farming_Crop.Empty:
                //put seed
                if (GameManager.instance.getSelectedItem() == null) { 
                    MainCanvas.instance.popupTextTrigger("Select a seed in your bag", transform.position); 
                    return; 
                }
                if(GameManager.instance.getSelectedItem().type == item_type.Seed)
				{
					switch (GameManager.instance.getSelectedItem().id)
					{
                        case 0: // Carrot seed
                            growing_time = 30;
                            current_growing_time = 0;
                            crop = Farming_Crop.Carrot;
                            break;
                        case 1: // pumpkin seed
                            growing_time = 60;
                            current_growing_time = 0;
                            crop = Farming_Crop.Pumpkin;
                            break;
					}
                    GameManager.instance.removeSelectedItem();
				}
				else
				{
                    // empty hand
                    MainCanvas.instance.popupTextTrigger("Select a seed in your bag", transform.position);
				}
                break;
            case Farming_Crop.Carrot:
            case Farming_Crop.Pumpkin:
                if(growing_stage == 5)
				{
                    // harvest
                    GameManager.instance.spawnItemDrop(transform.position, GameManager.instance.crops.First(o => o.cropType == crop).itemDrop);
                    crop = Farming_Crop.Empty;
                    transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = null;

                }
				else
				{
                    // cancel crop
                    GameManager.instance.spawnItemDrop(transform.position, GameManager.instance.crops.First(o => o.cropType == crop).seedDrop);
                    crop = Farming_Crop.Empty;
                    transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = null;
                }
                ready_to_harvest = false;
                break;
		}


	}
}
