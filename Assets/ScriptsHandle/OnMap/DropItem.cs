using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropItem : MonoBehaviour
{
    public Item Item;
    public Equipment Equipment;
    public Recipe Recipe;
    public int Coin = 0;



    SaveSystem SaveSystem = SaveSystem.Instance;



    // Update is called once per frame
    void Update()
    {
        
        Transform player = GameObject.Find("Player").transform;
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        if (distanceToPlayer <= 1.2f && Input.GetKeyDown(KeyCode.J))
        {
            if(Item != null)
            {
                SaveSystem.saveLoad.inventory.AddItem(Item);
            }
            if(Equipment != null)
            {
                SaveSystem.saveLoad.equipmentInventory.AddEquipment(Equipment);
            }
            if(Recipe != null)
            {
                SaveSystem.saveLoad.inventory.AddRecipe(Recipe);
            }
            SaveSystem.saveLoad.inventory.Coin += Coin;
            GameObject DropItemSaveObj = transform.parent.gameObject;
            this.gameObject.SetActive(false);
            DropItemSave DropItemSave = DropItemSaveObj.GetComponent<DropItemSave>();
            DropItemSave.SaveDropItem();
            Debug.Log("Call Method Save");

        }
    }
}
