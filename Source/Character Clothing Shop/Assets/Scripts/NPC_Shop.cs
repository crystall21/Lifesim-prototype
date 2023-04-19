using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_Shop : Interactable
{
    public SO_Shop shop;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	public override void Execute()
	{
		MainCanvas.instance.openShop(shop);
    }
}
