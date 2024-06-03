using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Windows;

public class GeneralInformation : MonoBehaviour
{
	//Singleton Declare
	private static GeneralInformation _instance;
	public static GeneralInformation Instance => _instance;
	private void Awake()
	{
		if (_instance == null)
		{
			_instance = this;
			DontDestroyOnLoad(gameObject);
		}
		else
		{
			Destroy(gameObject);
		}
	}

	//Talking, Shopping, Playing, Eventing
	public string Actioning = "Playing";
	public string currentScene;
    public int charSelectionNumber = 0;
	public bool ToSavePointNow = false;
	public bool IsWarp = false;

	public string TargetMap = "StartVillage";
	public string TargetPortal = "HouseA_Door";

    public List<Enemy> EnemyInBattle;
	public List<Equipment> EquipmentDrop;
	public List<Item> ItemDrop;
	public int Zone;
	public bool WinLose = false;


    private void Start()
    {
		StartCoroutine( TimeCount());
    }

	IEnumerator TimeCount()
	{
		while (true)
		{
			SaveSystem.Instance.saveLoad.Time++;
            yield return new WaitForSeconds(1f);
        }
    }	

}
