using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using System.Linq;
public class GameManager : MonoBehaviour
{
    int coins;
    public int Coins { 
        get { 
            return coins; 
        } 
        set { 
            coins = value; 
            MainCanvas.instance.updateCoinsText(); 
        } 
    }

    public List<SO_Crop> crops = new List<SO_Crop>();
    public List<SO_Item> item_database = new List<SO_Item>();

    public ObservableCollection<SO_Item> inventory = new ObservableCollection<SO_Item>();
    public int selectedItem;

    public GameObject item_drop_prefab;

    GameObject player;

    public static GameManager instance;

    
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        for(int i =0;i< 20; i++)
		{
            inventory.Add(null);
		}
        inventory.CollectionChanged += OnItemsChanged;
        player = FindObjectOfType<PlayerController>().gameObject;
        MainCanvas.instance.updateCoinsText();
    }

    // Update is called once per frame
    void Update()
    {
        // give random item
		//if (Input.GetKeyDown(KeyCode.Space))
		//{
  //          if (inventory.Where(o => o == null).Count() > 0)
  //          {
  //              inventory[inventory.IndexOf(inventory.First(o=>o == null))] = (item_database[Random.Range(0, item_database.Count)]);
  //          }
		//}
    }
    private void OnItemsChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        MainCanvas.instance.updateInventory();
    }

    public void spawnItemDrop(Vector2 position,SO_Item item)
	{
        var go = Instantiate(item_drop_prefab, position, Quaternion.identity);
        go.GetComponent<ItemDrop>().item = item;
        go.SetActive(true);
	}


    public bool addItem(SO_Item item)
	{
        bool result = false;
        if (inventory.Where(o => o == null).Count() > 0)
        {
            inventory[inventory.IndexOf(inventory.First(o => o == null))] = item;
            result = true;
		}
        return result;
    }

    public SO_Item getSelectedItem()
	{
        SO_Item result;
        if(selectedItem >= 0)
		{
            result = inventory[selectedItem];

		}
		else
		{
            result = null;
		}
        return result;
	}

    public void removeSelectedItem()
	{
        inventory[selectedItem] = null;
        selectedItem = -1;
        MainCanvas.instance.selectItem(-1);
	}

    public void dropSelectedItem()
	{
        spawnItemDrop(player.transform.position, getSelectedItem());

        removeSelectedItem();
    }

    public void sellSelectedItem()
	{
        Coins += getSelectedItem().selling_price;
        MainCanvas.instance.popupTextTrigger("+"+getSelectedItem().selling_price, Camera.main.ScreenToWorldPoint(Input.mousePosition));
        removeSelectedItem();
	}

    public void equipSelectedItem()
	{
        player.GetComponent<PlayerController>().equipClothing(getSelectedItem().Clothing_part, getSelectedItem());
	}




    public void buySelectedItem(SO_Item item, int price)
	{

        if(Coins >= price)
		{

            if (addItem(item))
			{

                Coins -= price;
                MainCanvas.instance.popupTextTrigger("Purchased!", Camera.main.ScreenToWorldPoint(Input.mousePosition));
            }
			else
			{

                // no space
                MainCanvas.instance.popupTextTrigger("No space", Camera.main.ScreenToWorldPoint(Input.mousePosition));
			}

		}
		else
		{
            // no coins
            MainCanvas.instance.popupTextTrigger("Not enough coins", Camera.main.ScreenToWorldPoint(Input.mousePosition));
        }
	}
}
