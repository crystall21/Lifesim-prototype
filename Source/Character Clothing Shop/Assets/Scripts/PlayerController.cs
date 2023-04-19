using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D rb;
    bool FacingRight = false;
    Vector3 movingVector;
    
    [SerializeField]
    bool canMove;

    [SerializeField]
    float movementSpeed;

    [SerializeField]
    GameObject spritesParent;

    Interactable target;

    float attackCD;
    [SerializeField]
    GameObject attack_effect;

    [SerializeField]
    SO_Item Head, Torso, Pants;



    [SerializeField]
    AnimatorOverrideController animator_override_controller;
    // Start is called before the first frame update
    public virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator_override_controller["Head_Default_Idle"] = Head.clothing_animations[0];
        animator_override_controller["Head_Default_Run"] = Head.clothing_animations[1];
        animator_override_controller["Torso_Default_Idle"] = Torso.clothing_animations[0];
        animator_override_controller["Torso_Default_Run"] = Torso.clothing_animations[1];
        animator_override_controller["Pants_Default_Idle"] = Pants.clothing_animations[0];
        animator_override_controller["Pants_Default_Run"] = Pants.clothing_animations[1];
    }

    // Update is called once per frame
    void Update()
    {
        #region MOVEMENT_INPUT
        if (canMove)
        {
            float X = Input.GetAxisRaw("Horizontal");
            float Y = Input.GetAxisRaw("Vertical");

            movingVector = new Vector3(X, Y).normalized;

            if ((X > 0 && !FacingRight) || (X < 0 && FacingRight))
            {
                Flip();
            }
        }
        spritesParent.GetComponent<Animator>().SetBool("Run", movingVector.magnitude > 0);
		#endregion

		#region INTERRACT_INPUT

        if(target != null && Input.GetKeyDown(KeyCode.E))
		{
            target.Execute();
		}

        if(attackCD > 0)
		{
            attackCD -= Time.deltaTime;
        }
        
        if(attackCD <= 0)
		{
			if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
			{
                Vector3 dir = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
                dir.z = transform.position.z;
                var hitPos = (dir - transform.position).normalized;

                var hits = new Collider2D[4];
                var colliders = Physics2D.OverlapCircleNonAlloc(transform.position + hitPos, 0.5f, hits, 1 << LayerMask.NameToLayer("Destructable"));

                if (colliders > 0)
                {
                    for(int i =0;i< colliders; i++)
					{
                        if(hits[i].TryGetComponent<Destructable>(out Destructable destructable))
						{
                            destructable.Destroy();
						}
					}
                }




                float angle = Mathf.Atan2(hitPos.y, hitPos.x) * Mathf.Rad2Deg;

                attack_effect.transform.parent.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
                attack_effect.GetComponent<Animation>().Play();
                attackCD = 1;
            }
		}

		#endregion
	}

	void FixedUpdate()
	{
        if (canMove) rb.velocity = movingVector * movementSpeed * Time.fixedDeltaTime;
        if (movingVector.magnitude == 0) return;
        var hits = new Collider2D[1];
        var colliders = Physics2D.OverlapCircleNonAlloc(transform.position+ (movingVector / 2), 0.25f, hits, 1 << LayerMask.NameToLayer("Interactable"));
       
        if (colliders > 0)
        {
            target = hits[0].GetComponent<Interactable>();
		}
		else
		{
            target = null;
            if (MainCanvas.instance.isShopOpened()) { 
                MainCanvas.instance.closeShop();
                if (MainCanvas.instance.isBagOpened()) MainCanvas.instance.toggleBag();
            
            }
        }
        MainCanvas.instance.interact_indicator.SetActive(target != null);

    }
    void LateUpdate()
    {
        // Change sorting order of the sprites
        spritesParent.GetComponentsInChildren<SpriteRenderer>().ToList().ForEach(o => o.sortingOrder = (int)Camera.main.WorldToScreenPoint(GetComponent<CapsuleCollider2D>().bounds.min).y * -1);
    }
    private void Flip()
    {
        FacingRight = !FacingRight;

        Vector3 theScale = spritesParent.transform.localScale;
        theScale.x *= -1;
        spritesParent.transform.localScale = theScale;
    }


    public void equipClothing(item_clothing_part part, SO_Item item)
    {
        switch (part)
        {
            case item_clothing_part.Head:
                if(Head != null)
				{
                    GameManager.instance.addItem(Head);
				}
                Head = item;
                animator_override_controller["Head_Default_Idle"] = Head.clothing_animations[0];
                animator_override_controller["Head_Default_Run"] = Head.clothing_animations[1];

                break;
            case item_clothing_part.Torso:
                if (Torso != null)
                {
                    GameManager.instance.addItem(Torso);
                }
                Torso = item;
                animator_override_controller["Torso_Default_Idle"] = Torso.clothing_animations[0];
                animator_override_controller["Torso_Default_Run"] = Torso.clothing_animations[1];

                break;
            case item_clothing_part.Pants:
                if (Pants != null)
                {
                    GameManager.instance.addItem(Pants);
                }
                Pants = item;
                animator_override_controller["Pants_Default_Idle"] = Pants.clothing_animations[0];
                animator_override_controller["Pants_Default_Run"] = Pants.clothing_animations[1];
                break;
        }

        GameManager.instance.removeSelectedItem();
    }



    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        if(movingVector.magnitude > 0)
        Gizmos.DrawWireSphere(transform.position + (movingVector/2), .25f);
    }


}
