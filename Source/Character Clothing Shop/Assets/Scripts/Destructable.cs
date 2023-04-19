using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructable : MonoBehaviour
{

    bool destroyed;

    float respawnTime;

    [SerializeField]
    List<SO_Item> lootTable = new List<SO_Item>();

    [SerializeField]
    ParticleSystem destroyParticles;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
		if (destroyed)
		{
            if(respawnTime > 0)
			{
                respawnTime -= Time.deltaTime;
			}
			else
			{
                respawnTime = 0;
                destroyed = false;
                GetComponent<SpriteRenderer>().enabled = true;
                GetComponent<Collider2D>().enabled = true;
			}
		}
    }



    public void Destroy()
	{
        if (destroyed) return;
        respawnTime = Random.Range(30f,60f);
        destroyed = true;
        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<Collider2D>().enabled = false;
        destroyParticles.Play();
        GameManager.instance.spawnItemDrop(transform.position, lootTable[Random.Range(0, lootTable.Count)]);
    }


}
