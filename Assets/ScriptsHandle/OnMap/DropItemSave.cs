using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DropItemSave : MonoBehaviour
{
    public string DropSaveName = "DropItem_[index]";
    public ListDropItem ListDropItem;

    private void Start()
    {
        if(!File.Exists("Assets/Resources/Save0/DropItem/" + DropSaveName + ".json"))
        {
            ListDropItem newLDI = new ListDropItem();
            for (int i = 0; i < transform.childCount; i++)
            {
                GameObject thisItem = transform.GetChild(i).gameObject;
                if (!thisItem.activeSelf)
                {
                    newLDI.IsNotPicked.Add(false);
                }
                else
                {
                    newLDI.IsNotPicked.Add(true);
                }
            }
            string saveRecord = JsonUtility.ToJson(newLDI);
            File.WriteAllText("Assets/Resources/Save0/DropItem/" + DropSaveName + ".json", saveRecord);
        }
        
        LoadDropItem();
    }

    private void OnEnable()
    {
        LoadDropItem();
    }

    private void Update()
    {
        
    }

    public void SaveDropItem()
    {
        ListDropItem newLDI = new ListDropItem();
        for(int i = 0; i<transform.childCount; i++)
        {
            GameObject thisItem = transform.GetChild(i).gameObject;
            if(!thisItem.activeSelf)
            {
                newLDI.IsNotPicked.Add(false);
            }    
            else
            {
                newLDI.IsNotPicked.Add(true);
            }
        }
        string saveRecord = JsonUtility.ToJson(newLDI);
        File.WriteAllText("Assets/Resources/Save/DropItem/" + DropSaveName + ".json", saveRecord);
    }

    void LoadDropItem()
    {
        if(File.Exists("Assets/Resources/Save/DropItem/" + DropSaveName + ".json"))
        {
            string saveRecord = File.ReadAllText("Assets/Resources/Save/DropItem/" + DropSaveName + ".json");
            ListDropItem = JsonUtility.FromJson<ListDropItem>(saveRecord);
            for (int i = 0; i < transform.childCount; i++)
            {
                GameObject thisItem = transform.GetChild(i).gameObject;
                thisItem.SetActive(ListDropItem.IsNotPicked[i]);
            }
        }    
    }
}
[System.Serializable]
public class ListDropItem
{
    public List<bool> IsNotPicked = new List<bool>();
}

