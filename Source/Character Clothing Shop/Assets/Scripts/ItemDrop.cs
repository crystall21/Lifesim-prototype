using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class ItemDrop : MonoBehaviour
{
    public bool collectable;
    public SO_Item item;
    // Start is called before the first frame update
    void Start()
    {
        collectable = false;
        GetComponent<SpriteRenderer>().sprite = item.icon;
        transform.DOJump(transform.position + new Vector3(Random.Range(-.25f, .25f), 0, 0), 2, 1, .5f).SetEase(Ease.Linear).OnComplete(() => collectable = true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	void OnTriggerStay2D(Collider2D collision)
	{
        if (!collectable) return;
		if(collision.TryGetComponent(out PlayerController player))
		{
			if (GameManager.instance.addItem(item))
			{
                Destroy(gameObject);
			}
		}
	}
}
