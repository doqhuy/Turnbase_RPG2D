using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[CreateAssetMenu(fileName = "New CharacterDefaultData", menuName = "ScriptableObjects/CharacterDefaultData")]
public class CharacterDefaultData : ScriptableObject
{
    public List<Character> Characters;

    bool CheckCharacterExist(Character character)
    {
        if(Characters.Contains(character)) return true;
        else
        return false;
    }    

    public void SaveThisData()
    {
        string SaveDefaultCharacter = JsonUtility.ToJson(this);
        File.WriteAllText("Assets/Resources/Save0/CharacterDefaultData.json", SaveDefaultCharacter);
    }

    public void SetDefaultData(Character character)
    {
        if (!CheckCharacterExist(character))
        {
            Characters.Add(character);
            string saveCharacter = JsonUtility.ToJson(character);
            File.WriteAllText("Assets/Resources/Save0/CharacterDefaultData/" + character.Name + ".json", saveCharacter);
        }
        SaveThisData();
    }
    public void GetDefaultData()
    {
        for (int i = 0; i < Characters.Count; i++)
        {
            string saveQuest = File.ReadAllText("Assets/Resources/Save0/CharacterDefaultData/" + Characters[i].Name + ".json");
            JsonUtility.FromJsonOverwrite(saveQuest, Characters[i]);
        }
    }


}
