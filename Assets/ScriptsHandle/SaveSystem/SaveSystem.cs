using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using UnityEditor;
using UnityEngine;

public class SaveSystem : MonoBehaviour
{
	private static SaveSystem _instance;
	public static SaveSystem Instance => _instance;

	private void Awake()
	{
		if (_instance == null)
		{
			_instance = this;
			DontDestroyOnLoad(gameObject);
		}
		else
		{
            LoadRecord(0);
            Destroy(gameObject);
		}
	}

	public Save saveLoad;
	public int EventsHandleNumber = 100;
	public int DropItemSave = 100;
    public int EnemySave = 100;

	private void Start()
	{

		if(!CheckSave(0))
		{
			SaveGame(0);
		}	
		StartNewRecord();
	}

	public void StartNewRecord()
	{
        LoadRecord(0);
	}


	public bool CheckSave(int index)
	{
		if(Directory.Exists("Assets/Resources/Save" + index))
		{
			return true;
		}	
		else
		{
			return false;
		}
	}	



	public void LoadRecord(int index)
	{

		if (Directory.Exists("Assets/Resources/Save" + index))
		{
			string saveRecord = File.ReadAllText("Assets/Resources/Save" + index + "/saveRecord.json");
            string saveRecord_questClaim = File.ReadAllText("Assets/Resources/Save" + index + "/saveRecord_questClaim.json");


            JsonUtility.FromJsonOverwrite(saveRecord, saveLoad);
            JsonUtility.FromJsonOverwrite(saveRecord_questClaim, saveLoad.questClaim);


            for (int i = 0; i < saveLoad.team.ClaimedCharacter.Count; i++)
            {
                if(File.Exists("Assets/Resources/Save" + index + "/CharacterData/" + saveLoad.team.ClaimedCharacter[i].name + ".json"))
                {
                    string CharacterSaveRecord = File.ReadAllText("Assets/Resources/Save" + index + "/CharacterData/" + saveLoad.team.ClaimedCharacter[i].name + ".json");
                    JsonUtility.FromJsonOverwrite(CharacterSaveRecord, saveLoad.team.ClaimedCharacter[i]);
                }    
                
            }

			if(File.Exists("Assets/Resources/Save0/QuestDefaultData.json"))
			{
                string QuestDefaultSave = File.ReadAllText("Assets/Resources/Save0/QuestDefaultData.json");
                JsonUtility.FromJsonOverwrite(QuestDefaultSave, saveLoad.questClaim.QuestDefaultData);
                saveLoad.questClaim.QuestDefaultData.GetDefaultData();
            }

            if (File.Exists("Assets/Resources/Save0/CharacterDefaultData.json"))
            {
                string CharacterDefaultSave = File.ReadAllText("Assets/Resources/Save0/CharacterDefaultData.json");
                JsonUtility.FromJsonOverwrite(CharacterDefaultSave, saveLoad.team.CharacterDefaultData);
                saveLoad.team.CharacterDefaultData.GetDefaultData();
            }

            for (int i = 0; i < saveLoad.questClaim.quests.Count; i++)
            {
                string QuestSaveRecord = File.ReadAllText("Assets/Resources/Save" + index + "/QuestData/Quest" + i + ".json");
                JsonUtility.FromJsonOverwrite(QuestSaveRecord, saveLoad.questClaim.quests[i]);
            }

            for (int i = 0; i < saveLoad.team.ClaimedCharacter.Count; i++)
			{
				string CharacterSaveRecord = File.ReadAllText("Assets/Resources/Save" + index + "/CharacterData/" + saveLoad.team.ClaimedCharacter[i].name + ".json");
                JsonUtility.FromJsonOverwrite(CharacterSaveRecord, saveLoad.team.ClaimedCharacter[i]);
            }

            for (int i = 0; i < EventsHandleNumber; i++)
            {
				if(File.Exists("Assets/Resources/Save" + index + "/Event/EventHandle_" + i + ".json"))
				{
                    string ThisEvent = File.ReadAllText("Assets/Resources/Save" + index + "/Event/EventHandle_" + i + ".json");
                    File.WriteAllText("Assets/Resources/Save/Event/EventHandle_" + i + ".json", ThisEvent);
                }
            }

            for (int i = 0; i < DropItemSave; i++)
            {
                if (File.Exists("Assets/Resources/Save" + index + "/DropItem/DropItem_" + i + ".json"))
                {
                    string ThisDropItem = File.ReadAllText("Assets/Resources/Save" + index + "/DropItem/DropItem_" + i + ".json");
                    File.WriteAllText("Assets/Resources/Save/DropItem/DropItem_" + i + ".json", ThisDropItem);
                }
            }

            for (int i = 0; i < EnemySave; i++)
            {
                if (File.Exists("Assets/Resources/Save" + index + "/EnemySave/EnemySave_" + i + ".json"))
                {
                    string ThisEnemy = File.ReadAllText("Assets/Resources/Save" + index + "/EnemySave/EnemySave_" + i + ".json");
                    File.WriteAllText("Assets/Resources/Save/EnemySave/EnemySave_" + i + ".json", ThisEnemy);
                }
            }
        }

        if (index == 0)
        {
            saveLoad.team.CharacterDefaultData.Characters.Clear();
            saveLoad.questClaim.QuestDefaultData.Quests.Clear();
            foreach (Character character in saveLoad.team.Teamate)
            {
                if (character != null)
                    saveLoad.team.AddCharacter(character);
            }
            saveLoad.Time = 0f;
        }
    }





	public void DeleteRecord(int index)
	{
		if (Directory.Exists("Assets/Resources/Save" + index))
		{
			File.Delete("Assets/Resources/Save" + index + ".meta");
			Directory.Delete("Assets/Resources/Save" + index, true);
		}
	}

	public void SaveGame(int index)
	{

		if (!Directory.Exists("Assets/Resources/Save" + index))
		{
			Directory.CreateDirectory("Assets/Resources/Save" + index);
			Directory.CreateDirectory("Assets/Resources/Save" + index + "/CharacterData");
            Directory.CreateDirectory("Assets/Resources/Save" + index + "/CharacterDefaultData");
			Directory.CreateDirectory("Assets/Resources/Save" + index + "/Event");
            Directory.CreateDirectory("Assets/Resources/Save" + index + "/DropItem");


            Directory.CreateDirectory("Assets/Resources/Save" + index + "/QuestData");
            Directory.CreateDirectory("Assets/Resources/Save" + index + "/QuestDefaultData");

            Directory.CreateDirectory("Assets/Resources/Save" + index + "/EnemySave");

        }



        string saveRecord = JsonUtility.ToJson(saveLoad);
        string saveRecord_questClaim = JsonUtility.ToJson(saveLoad.questClaim);

        File.WriteAllText("Assets/Resources/Save" + index + "/saveRecord.json", saveRecord);
        File.WriteAllText("Assets/Resources/Save" + index + "/saveRecord_questClaim.json", saveRecord_questClaim);

        for (int i = 0; i < saveLoad.team.ClaimedCharacter.Count; i++)
		{
			string CharacterSaveRecord = JsonUtility.ToJson(saveLoad.team.ClaimedCharacter[i]);
			File.WriteAllText("Assets/Resources/Save" + index + "/CharacterData/"+ saveLoad.team.ClaimedCharacter[i].name+ ".json", CharacterSaveRecord);
		}

        for (int i = 0; i < saveLoad.questClaim.quests.Count; i++)
        {
			string QuestSaveRecord = JsonUtility.ToJson(saveLoad.questClaim.quests[i]);
            File.WriteAllText("Assets/Resources/Save" + index + "/QuestData/Quest" + i + ".json", QuestSaveRecord);
        }

		for(int i = 0; i < EventsHandleNumber; i++)
		{
			if(File.Exists("Assets/Resources/Save/Event/EventHandle_" + i + ".json"))
			{
                string ThisEvent = File.ReadAllText("Assets/Resources/Save/Event/EventHandle_" + i + ".json");
                File.WriteAllText("Assets/Resources/Save" + index + "/Event/EventHandle_" + i + ".json", ThisEvent);
            }
        }

        for (int i = 0; i < DropItemSave; i++)
        {
            if (File.Exists("Assets/Resources/Save/DropItem/DropItem_" + i + ".json"))
            {
                string ThisDropItem = File.ReadAllText("Assets/Resources/Save/DropItem/DropItem_" + i + ".json");
                File.WriteAllText("Assets/Resources/Save" + index + "/DropItem/DropItem_" + i + ".json", ThisDropItem);
            }
        }

        for (int i = 0; i < EnemySave; i++)
        {
            if (File.Exists("Assets/Resources/Save/EnemySave/EnemySave_" + i + ".json"))
            {
                string ThisEnemy = File.ReadAllText("Assets/Resources/Save/EnemySave/EnemySave_" + i + ".json");
                File.WriteAllText("Assets/Resources/Save" + index + "/EnemySave/EnemySave_" + i + ".json", ThisEnemy);
            }
        }

        

    }

}
