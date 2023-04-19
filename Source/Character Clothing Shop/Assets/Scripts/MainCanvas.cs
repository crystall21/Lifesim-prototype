using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;
using DG.Tweening;
public class MainCanvas : MonoBehaviour
{

    [SerializeField]
    List<GameObject> inventorySlots = new List<GameObject>();

    [SerializeField]
    TextMeshProUGUI popupText,coinsText;

    public GameObject item_info_panel,item_bag;

    bool bag_opened, can_open_bag,can_open_shop;
    public bool can_sell;

    public GameObject interact_indicator;
    PlayerController player;

    [Header("Shop")]
    [SerializeField]
    GameObject shop_panel;

    [SerializeField]
    GameObject shop_content,shop_item_prefab,shopitem_info_panel;

    SO_Shop opened_shop;
    int selected_shop_item;

    public static MainCanvas instance;
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        can_open_bag = true;
        can_open_shop = true;
        player = FindObjectOfType<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
		#region INTERFACE_INPUT
		if (Input.GetKeyDown(KeyCode.B))
		{
            toggleBag();
		}
        if (Input.GetKeyDown(KeyCode.Escape))
        {
			if (isBagOpened())
			{
                toggleBag();
			}
            if(isShopOpened())
			{
                closeShop();
			}
        }

        #endregion

        if (interact_indicator.activeInHierarchy)
		{
            interact_indicator.transform.position = player.transform.position - new Vector3(0, 1, 0);

        }
	}

    public void updateCoinsText()
	{
        coinsText.text = GameManager.instance.Coins + " <sprite=0>";

    }
	public void updateItemInfoPanel()
	{
		if (GameManager.instance.getSelectedItem())
		{
            item_info_panel.SetActive(true);
            item_info_panel.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = GameManager.instance.getSelectedItem().item_name;
            item_info_panel.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = GameManager.instance.getSelectedItem().description;
            item_info_panel.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = GameManager.instance.getSelectedItem().selling_price + " <sprite=0>";
            item_info_panel.transform.GetChild(4).gameObject.SetActive(can_sell);
            item_info_panel.transform.GetChild(5).gameObject.SetActive(GameManager.instance.getSelectedItem().type == item_type.Clothing);

        }
		else
		{
            item_info_panel.SetActive(false);
		}
	}

    public void updateInventory()
	{
        for(int i = 0; i < 20; i++)
		{
            if(GameManager.instance.inventory[i] != null)
			{
                inventorySlots[i].transform.GetChild(0).GetComponent<Image>().enabled = true;
                inventorySlots[i].transform.GetChild(0).GetComponent<Image>().sprite = GameManager.instance.inventory[i].icon;
			}
			else
			{
                inventorySlots[i].transform.GetChild(0).GetComponent<Image>().enabled = false;

            }
		}
	}

    public void popupTextTrigger(string text, Vector2 position)
	{
        popupText.color = new Color(1, 1, 1, 1);
        popupText.text = text;
        popupText.transform.position = position;
        popupText.transform.DOKill();
        popupText.DOKill();
        popupText.transform.DOMoveY(popupText.transform.position.y + .5f, 1).OnComplete(()=> popupText.DOFade(0, .5f));
	}
    

    public void toggleBag()
	{
        if (can_open_bag)
		{

            StartCoroutine(toggleBagCoroutine());
			if (bag_opened)
			{

                item_bag.GetComponent<Animation>().Play("bag-menu-disappear");

			}
			else
			{

                item_bag.GetComponent<Animation>().Play("bag-menu-appear");
                item_bag.SetActive(true);
            }
            bag_opened = !bag_opened;
		}
	}

    IEnumerator toggleBagCoroutine()
	{
        selectItem(-1);
        can_open_bag = false;
        yield return new WaitForSeconds(.5f);
        can_open_bag = true;
		if (!bag_opened)
		{
            item_bag.SetActive(false);
		}
	}
    IEnumerator closeShopCoroutine()
    {
        selected_shop_item = -1;
        updateShopItemInfoPanel();
        can_open_shop = false;
        shop_panel.GetComponent<Animation>().Play("shop-menu-disappear");
        yield return new WaitForSeconds(.5f);
        can_sell = false;
        opened_shop = null;
        selected_shop_item = -1;
        shop_panel.SetActive(false);
        for (int i = 0; i < shop_content.transform.childCount; i++)
        {
            Destroy(shop_content.transform.GetChild(i).gameObject);
        }
        can_open_shop = true;

    }

    public void selectItem(int index)
	{
        // turning off all selection borders
        inventorySlots.ForEach(o => o.transform.GetChild(1).gameObject.SetActive(false));

        if (index > -1 && index != GameManager.instance.selectedItem && GameManager.instance.inventory[index] != null)
        {
            // turning on the selection border of the newly selected item
            inventorySlots[index].transform.GetChild(1).gameObject.SetActive(true);
            GameManager.instance.selectedItem = index;
		}
		else
		{
            GameManager.instance.selectedItem = -1;

        }
        updateItemInfoPanel();
    }

    public void openShop(SO_Shop shop)
	{
        if (!can_open_shop) return;
        if (isShopOpened()) return;

        // updating the shop details
        shop_panel.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = shop.shop_Name;
        for (int i = 0; i < shop.shop_items.Count; i++)
		{
            var shopItem = Instantiate(shop_item_prefab, shop_content.transform);
            shopItem.transform.GetChild(0).GetComponent<Image>().sprite = shop.shop_items[i].item.icon;
            shopItem.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = shop.shop_items[i].item.item_name;
            shopItem.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = shop.shop_items[i].price + " <sprite=0>";
            int index = i;
            shopItem.GetComponent<Button>().onClick.AddListener(() => selectShopItem(index));

        }

        if (!bag_opened)
        {
            toggleBag();
        }

        can_sell = true;
        opened_shop = shop;
        selected_shop_item = -1;
        shop_panel.SetActive(true);
        shop_panel.GetComponent<Animation>().Play("shop-menu-appear");
    }

    public void selectShopItem(int id)
    {
        // turning off all selection borders of shop items
        for (int i = 0; i < opened_shop.shop_items.Count; i++)
        {
            shop_content.transform.GetChild(i).GetChild(3).gameObject.SetActive(false);

        }

        // is there a selected shop item?
        if (selected_shop_item != id)
        {
            // no, so lets select one
            selected_shop_item = id;
            shop_content.transform.GetChild(id).GetChild(3).gameObject.SetActive(true);
        }
        else
        {
            // lets deselect it
            selected_shop_item = -1;
            shop_content.transform.GetChild(id).GetChild(3).gameObject.SetActive(false);

        }
        updateShopItemInfoPanel();

    }

    public bool isShopOpened()
	{
        return opened_shop != null;
	}
    public bool isBagOpened()
    {
        return bag_opened;
    }

    public void closeShop()
	{
        StartCoroutine(closeShopCoroutine());
	}

    public void updateShopItemInfoPanel()
    {
        if (selected_shop_item >=0)
        {
            shopitem_info_panel.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = opened_shop.shop_items[selected_shop_item].item.item_name;
            shopitem_info_panel.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = opened_shop.shop_items[selected_shop_item].item.description;
            shopitem_info_panel.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = opened_shop.shop_items[selected_shop_item].price + " <sprite=0>";
            shopitem_info_panel.SetActive(true);

        }
        else
        {
            shopitem_info_panel.SetActive(false);
        }
    }

    public void buyItemButton()
	{
        GameManager.instance.buySelectedItem(opened_shop.shop_items[selected_shop_item].item, opened_shop.shop_items[selected_shop_item].price);
	}
}
