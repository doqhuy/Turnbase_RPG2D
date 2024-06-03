﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BattleHandle : MonoBehaviour
{
    public Button Attack;

    public Button Protect;
	public Button SkillLB;
	public Button ItemLB;
    public Button Cancel;
	public GameObject tooltip; 

    public GameObject Round;
    public GameObject Action;


	public GameObject slotButtonPrefab;
    public GameObject SkillsList;
    
    public GameObject ItemsList;
	

    public GameObject WinObj;
	public GameObject LoseObj;

    public GameObject[] selectEnemies;
    public GameObject[] selectTeamate;

    public List<Toggle> EnemyToggle;
	public List<Toggle> TeamateToggle;

	public List<Sprite> BackGrounds;
	public GameObject CurrentBG;

    public GeneralInformation GeneralInformation;
    public SaveSystem SaveSystem;

	private List<ItemWithQuantity> items;
	

    public List<TargetOnTurn> Target;
	private List<Enemy> enemies;
	private Enemy TargetEnemy;
	private int TargetEnemyIndex;
	private Enemy SelfEnemy;
	private int SelfEnemyIndex = 0;

	private List<Character> teamates;
	private Character TargetCharacter;
	private int TargetCharacterIndex;
	private Character SelfCharacter;
	private int SekfCharacterIndex = 0;

	public int turn = 0;
	private void Start()
	{

		GeneralInformation = GeneralInformation.Instance;
        SaveSystem = SaveSystem.Instance;
        //items = SaveSystem.saveLoad.inventory.Items;
        items = SaveSystem.saveLoad.inventory.Items;
		int index = GeneralInformation.Zone;
		Image BG = CurrentBG.GetComponent<Image>();
		BG.sprite = BackGrounds[index];
        if (Target == null)
		{
			Target = new List<TargetOnTurn>();
		}
		InitialBattle();
		

        turn--;
		NextTurn();
		ActionHanle();

	}

	//Kiểm tra kẻ thù chết chưa
	public bool CheckEnemy()
	{
		for(int i=0; i<enemies.Count; i++)
		{
			if (enemies[i].HP > 0) return true;
		}	
		return false;
	}	

	//Kiểm tra team còn sống
	public bool CheckTeam()
	{
		for (int i = 0; i < 4; i++)
		{
			Character thisChar = teamates[i];
			if(thisChar != null) 
			{
				if (thisChar.HP > 0) return true;
			}
		}
		return false;
	}	

	public void CheckWinLose()
	{
		if (!CheckTeam())
		{
			for (int i = 0; i < 4; i++)
			{
				Character thisChar = teamates[i];
				if (thisChar != null)
				{
					thisChar.ResetAllBuffCount();
				}
			}
			LoseObj.SetActive(true); 
			this.gameObject.SetActive(false);
		}

		if (!CheckEnemy())
		{
			for (int i = 0; i < 4; i++)
			{
				Character thisChar = teamates[i];
				if (thisChar != null)
				{
					thisChar.ResetAllBuffCount();
				}
			}
			WinObj.SetActive(true); 
			this.gameObject.SetActive(false);
		}
	}	

	public void NextTurn()
	{
		CheckWinLose();
		Target = Target.OrderBy(t => t.Speed).ToList();
		turn++;
		if (turn >= Target.Count) turn = 0;

		
		RoundHandle();


		if (Target[turn].isDead)
		{
			NextTurn();
		}
		else if (Target[turn].isEnemy)
		{
			EnemyAction();
		}
	}

	public void EnemyAction()
	{
		int RandomFunc = UnityEngine.Random.Range(0, 3);
		if (RandomFunc == 0) AttackCast_Enemy();
		if (RandomFunc == 1) ProtectCast();
		if (RandomFunc >= 2) SkillCast_Enemy(Target[turn].enemy);
		StartCoroutine(WaitingForNextAction());
	}

	public void DisableUi(bool choose)
	{
        Cancel.gameObject.SetActive(choose);
        Attack.enabled = choose;
        Protect.enabled = choose;
        SkillLB.enabled = choose;
        ItemLB.enabled = choose;
        SkillsList.gameObject.SetActive(choose);
        ItemsList.gameObject.SetActive(choose);
    }
	IEnumerator WaitingForNextAction()
	{
		CheckWinLose();
		if (!this.gameObject.activeSelf) yield return null;
		DisableUi(false);		
        SetEnemiesActive(false);
        SetTeamateActive(false);
        yield return new WaitForSecondsRealtime(1.5f); // Độ trễ 1 giây
		NextTurn();
	}

	void ActionHanle()
    {

        Attack.onClick.RemoveAllListeners();
        Attack.onClick.AddListener(() => 
		{
            Cancel.gameObject.SetActive(true);
            SkillsList.gameObject.SetActive(false);
            ItemsList.gameObject.SetActive(false);
            ShowTargetSelectionUIForAttack();

        });
	Protect.onClick.RemoveAllListeners();
		Protect.onClick.AddListener(() => 
		{
			ProtectCast();
			StartCoroutine(WaitingForNextAction());
		});

		SkillLB.onClick.RemoveAllListeners();
		SkillLB.onClick.AddListener(() =>
		{
            ItemsList.gameObject.SetActive(false);
            SkillDisplay();            
            SkillsList.gameObject.SetActive(true);
		});

        ItemLB.onClick.RemoveAllListeners();
        ItemLB.onClick.AddListener(() =>
        {
            SkillsList.gameObject.SetActive(false);
            ItemDisplay();
            ItemsList.gameObject.SetActive(true);
        });
        Cancel.onClick.RemoveAllListeners();
        Cancel.onClick.AddListener(() =>
        {
            Cancel.gameObject.SetActive(false);
            SetTeamateActive(false);
			SetEnemiesActive(false);
        });
    }

	void ProtectCast()
	{
		if (Target[turn].character != null)
		{
			Target[turn].character.SetProtect();
		}
		if (Target[turn].enemy != null)
		{
			Target[turn].enemy.SetProtect();
		}
	}	

	//Lượt tiếp theo
	

	

	void SkillDisplay()
	{
        foreach (Transform child in SkillsList.transform)
        {
            Destroy(child.gameObject);
        }
        for (int i = 0; i < Target[turn].character.learnedSkills.Count; i++)
        {
            
            
            if (Target[turn].character.learnedSkills[i] != null )
            {
                // Instantiate a new skill button
                GameObject skillButton = Instantiate(slotButtonPrefab, SkillsList.transform);
                GameObject Header = skillButton.transform.Find("Header").gameObject;
                GameObject Content = skillButton.transform.Find("Content").gameObject;

                Button button = skillButton.GetComponent<Button>();
                Image image = skillButton.GetComponentInChildren<Image>();
                TMP_Text HeaderText = Header.GetComponentInChildren<TMP_Text>();
                TMP_Text contentText = Content.GetComponentInChildren<TMP_Text>();


                // Set the skill button's image and text
                image.sprite = Target[turn].character.learnedSkills[i].Image;
				HeaderText.text = Target[turn].character.learnedSkills[i].name;
				contentText.text = Target[turn].character.learnedSkills[i].Description;

                // Add a listener to the skill button to cast the skill when clicked
                int skillIndex = i; // To avoid closure issues
                button.onClick.AddListener(() =>
                {
                    tooltip.gameObject.SetActive(false);
                    Cancel.gameObject.SetActive(true);
					SkillsList.gameObject.SetActive(false);
					ShowTargetSelectionUIForSkill(skillIndex);
                });
            }
        }
    }
    
	void ItemDisplay()
	{
        foreach (Transform child in ItemsList.transform)
        {
            Destroy(child.gameObject);
        }

        
        for (int i = 0; i < items.Count; i++)
        {
            
            if (items[i] != null && items[i].Item.Usable == true)
            {
                
                GameObject itemButton = Instantiate(slotButtonPrefab, ItemsList.transform);

                
                GameObject headerObj = itemButton.transform.Find("Header").gameObject;
                GameObject contentObj = itemButton.transform.Find("Content").gameObject;

                
                Button button = itemButton.GetComponent<Button>();
                Image image = itemButton.GetComponentInChildren<Image>();
                TMP_Text headerText = headerObj.GetComponentInChildren<TMP_Text>();
                TMP_Text contentText = contentObj.GetComponentInChildren<TMP_Text>();

                
                image.sprite = items[i].Item.Image;
                headerText.text = items[i].Item.Name;
                contentText.text = items[i].Item.Description;

                
                int itemIndex = i; 
                button.onClick.RemoveAllListeners(); 
                button.onClick.AddListener(() =>
                {
                    
                    tooltip.gameObject.SetActive(false);
                    Cancel.gameObject.SetActive(true);
                    ItemsList.gameObject.SetActive(false);
                    ShowTargetSelectionUIForItem(itemIndex);
                });
            }
        }
    }  
	 
	

    //Xử lý vòng
    void RoundHandle()
    {
		for (int i = 0; i < Round.transform.childCount; i++)
        {
			GameObject thisObj = Round.transform.GetChild(i).gameObject;
            GameObject thisTurnAnimated = thisObj.transform.Find("Turn").gameObject;

			int index = i;

			//Hiển thị các nhân vật
			Image thisImage = thisObj.GetComponent<Image>();
			if (index < Target.Count)
			{
				if (Target[index].enemy != null)
				{
					thisImage.sprite = Target[i].enemy.imageEnemy;
					if(Target[index].enemy.HP <= 0)
					{
						Target[index].isDead = true;
						thisObj.SetActive(false);
					}	
				}
				else if (Target[index].character != null)
				{
					thisImage.sprite = Target[index].character.Avatar;
					if (Target[index].character.HP <= 0)
					{
						Target[index].isDead = true;
						thisObj.SetActive(false);
					}
				}
				else
				{
					thisObj.SetActive(false);
				}
			}
			else
			{
				thisObj.SetActive(false); 
			}

			// Nhân vật đến lượt
			if (turn == index)
            {
				Toggle toggle = thisObj.GetComponent<Toggle>();
                toggle.isOn = true;
                thisTurnAnimated.SetActive(true);
				
				if (turn < Target.Count)
				{
					if (Target[turn].isEnemy)
					{
						DisableUi(false);
                    }
					else
					{
						Attack.enabled = true;
						Protect.enabled = true;
                        SkillLB.enabled = true;
                        ItemLB.enabled = true;
                        
                    }
				}	
			}
            else
            {
				thisTurnAnimated.SetActive(false);
			}
		}
    }

	//Hàm tấn công của kẻ thù
	public void AttackCast_Enemy()
	{
        
        Enemy enemyTarget = Target[turn].enemy;
        Character characterTarget;
		while (true)
		{
            int index = UnityEngine.Random.Range(0, 3);
            characterTarget = teamates[index];

            if (characterTarget != null && characterTarget.HP > 0)
			{
				enemyTarget.NormalAttack(characterTarget);
				GameObject ActionedObj = TeamateToggle[index].transform.Find("Actioned").gameObject;
				Animator ActionedAnimator = ActionedObj.GetComponent<Animator>();
				ActionedAnimator.Play(enemyTarget.AttackAnimationType);
				return;
			}
		
        }

    }	


	//Hàm tấn công của người chơi
	public void AttackCast()
	{
		//SelectTargetHandle();
		if (!Target[turn].isEnemy) 
		{
			Character targetCharacter = Target[turn].character;			
					GameObject ActionedObj = EnemyToggle[TargetEnemyIndex].transform.Find("Actioned").gameObject;
					if(ActionedObj.activeSelf)
					{
						Animator ActionedAnimator = ActionedObj.GetComponent<Animator>();
						ActionedAnimator.Play(targetCharacter.AttackAnimationType);
					}								
			targetCharacter.NormalAttack(TargetEnemy);

		}
	}	

	void SkillCast(int skillNumber)
	{
		if (!Target[turn].isEnemy)
		{
			
			if (!Target[turn].character.learnedSkills[skillNumber].IsMultiple)
			{
				SkillCastAnimation(Target[turn].character.learnedSkills[skillNumber], TargetCharacterIndex, TargetEnemyIndex);
				Target[turn].character.CastSkill(TargetEnemy, TargetCharacter, Target[turn].character.learnedSkills[skillNumber]);
			}
			else
			{
				if (Target[turn].character.learnedSkills[skillNumber].Target == "Enemy")
				{
					for (int i = 0; i < enemies.Count; i++)
					{
						if(enemies[i]!=null)
						{
							SkillCastAnimation(Target[turn].character.learnedSkills[skillNumber], TargetCharacterIndex, i);
							Target[turn].character.CastSkill(enemies[i], TargetCharacter, Target[turn].character.learnedSkills[skillNumber]);
						}	
						
					}
				}
				else if (Target[turn].character.learnedSkills[skillNumber].Target == "Team")
				{
					for (int i = 0; i < teamates.Count; i++)
					{
						if (teamates[i]!=null)
						{
							SkillCastAnimation(Target[turn].character.learnedSkills[skillNumber], i, TargetEnemyIndex);
							Target[turn].character.CastSkill(TargetEnemy, teamates[i], Target[turn].character.learnedSkills[skillNumber]);
						}	
					}
				}
			}
		}
	}

	public void SkillCastAnimation(Skill skill, int CharIndex, int EIndex)
	{
		if(skill.Target == "Enemy")
		{
			GameObject ActionedObj = EnemyToggle[EIndex].transform.Find("Actioned").gameObject;
			if (ActionedObj.activeSelf && enemies[EIndex].HP > 0)
			{
				Animator ActionedAnimator = ActionedObj.GetComponent<Animator>();
				ActionedAnimator.Play(skill.AnimationType);
			}
		}
		else if (skill.Target == "Team")
		{
			GameObject ActionedObj = TeamateToggle[CharIndex].transform.Find("Actioned").gameObject;
			if (ActionedObj.activeSelf && teamates[CharIndex].HP > 0)
			{
				Animator ActionedAnimator = ActionedObj.GetComponent<Animator>();
				ActionedAnimator.Play(skill.AnimationType);
			}
		}	
		else if (skill.Target == "Self")
		{
			if (!Target[turn].isEnemy)
			{
				GameObject ActionedObj = TeamateToggle[Target[turn].SelfIndex].transform.Find("Actioned").gameObject;
				if (ActionedObj.activeSelf)
				{
					Animator ActionedAnimator = ActionedObj.GetComponent<Animator>();
					ActionedAnimator.Play(skill.AnimationType);
				}
			}	
			else
			{
				GameObject ActionedObj = EnemyToggle[Target[turn].SelfIndex].transform.Find("Actioned").gameObject;
				if (ActionedObj.activeSelf)
				{
					Animator ActionedAnimator = ActionedObj.GetComponent<Animator>();
					ActionedAnimator.Play(skill.AnimationType);
				}
			}	
		}	
	}



	void SkillCast_Enemy(Enemy enemy)
	{
		int randomSkill = UnityEngine.Random.Range(0, enemy.skill.Count);
		int ranE = GetRandomEnemy();
		int ranC = GetRandomCharacter();
		if (!enemy.skill[randomSkill].IsMultiple)
		{
			SkillCastAnimation(enemy.skill[randomSkill], ranC, ranE);
			enemy.CastSkill(enemies[ranE], teamates[ranC], enemy.skill[randomSkill]);
		}
		else
		{
			if (enemy.skill[randomSkill].Target == "Enemy")
			{
				for(int i=0; i<enemies.Count; i++)
				{
					if (enemies[i]!=null)
					{
						SkillCastAnimation(enemy.skill[randomSkill], ranC, i);
						enemy.CastSkill(enemies[i], teamates[ranC], enemy.skill[randomSkill]);
					}	
				}
			}	
			else if(enemy.skill[randomSkill].Target == "Team")
			{
				for (int i = 0; i < teamates.Count; i++)
				{
					if (teamates[i]!=null)
					{
						SkillCastAnimation(enemy.skill[randomSkill], i, ranE);
						enemy.CastSkill(enemy, teamates[i], enemy.skill[randomSkill]);
					}
				}
			}	
		}	
	}

	public int GetRandomCharacter()
	{
		int ranCharIndex = UnityEngine.Random.Range(0, 10);
		while (ranCharIndex >= 0)
		{
			for(int  i = 0; i < teamates.Count; i++)
			{
				int index = i;
				if (teamates[index]!=null)
				{
					if (teamates[index].HP > 0)
					{
						if (ranCharIndex == 0)
						{
							return index;
						}
						ranCharIndex--;
					}
				}
			}	
		}
		return 0;
	}	

	public int GetRandomEnemy()
	{
		int ranEnemIndex = UnityEngine.Random.Range(0, 10);
		while (ranEnemIndex >= 0)
		{
			for (int i = 0; i < enemies.Count; i++)
			{
				int index = i;
				if (enemies[index].HP > 0)
				{
					if (ranEnemIndex == 0)
					{
						return index;
					}
					ranEnemIndex--;
				}
			}
		}
		return 0;
	}



    //Khởi tạo trận đấu
    public void InitialBattle()
	{
		enemies = GeneralInformation.EnemyInBattle;
		teamates = SaveSystem.saveLoad.team.Teamate;


		for (int i=0; i<GeneralInformation.EnemyInBattle.Count; i++)
		{
			Enemy newenemy = enemies[i];
			TargetOnTurn newEnemyTarget = new TargetOnTurn(newenemy,null, newenemy.Speed, true, i);
			Target.Add(newEnemyTarget);
		}

		for(int i=0; i<4; i++)
		{
			if (SaveSystem.saveLoad.team.Teamate[i] != null)
			{
				Character thisChar = teamates[i];
				TargetOnTurn newCharTarget = new TargetOnTurn(null, thisChar, thisChar.Speed, false, i);
				Target.Add(newCharTarget);
			}
		}
	}

    public void SetEnemiesActive(bool isActive)
    {
        // Loop through each GameObject in the array
		for(int i=0; i < selectEnemies.Length;i++)
		{
			// Set the active state of the GameObject
			selectEnemies[i].SetActive(isActive);
           
        }
        
    }

    public void SetTeamateActive(bool isActive)
    {
        // Loop through each GameObject in the array
        for (int i = 0; i < selectTeamate.Length; i++)
        {
            // Set the active state of the GameObject
            selectTeamate[i].SetActive(isActive);

        }

    }
    
	 void ShowTargetSelectionUIForItem(int ItemIndex)
    {
		
			SetTeamateActive(true);
			for (int i = 0; i < selectTeamate.Length; i++)
			{
				// Set the active state of the GameObject           
				GameObject currentObj = selectTeamate[i];
				Button button = selectTeamate[i].GetComponent<Button>();
				int index = i;
				button.onClick.RemoveAllListeners();

				button.onClick.AddListener(() =>
				{
                    
                    
					TargetEnemy = enemies[index];
					TargetEnemyIndex = index;
					TargetCharacter = teamates[index];
					TargetCharacterIndex = index;
					StartCoroutine(SaveSystem.saveLoad.inventory.UseItem(TargetCharacter, teamates, ItemIndex));
					StartCoroutine(WaitingForNextAction());
				});
			}
		
		
    }
	 
    void ShowTargetSelectionUIForSkill(int skillIndex)
    {
		if (Target[turn].character.learnedSkills[skillIndex].Target == "Team")
		{
			SetTeamateActive(true);
			for (int i = 0; i < selectTeamate.Length; i++)
			{
				// Set the active state of the GameObject           
				GameObject currentObj = selectTeamate[i];
				Button button = selectTeamate[i].GetComponent<Button>();
				int index = i;
				button.onClick.RemoveAllListeners();

				button.onClick.AddListener(() =>
				{
                    
                    TargetEnemy = enemies[index];
					TargetEnemyIndex = index;
					TargetCharacter = teamates[index];
					TargetCharacterIndex = index;
					SkillCast(skillIndex);
					StartCoroutine(WaitingForNextAction());
				});
			}
		}
		else
		{
			SetEnemiesActive(true);
			for (int i = 0; i < selectEnemies.Length; i++)
			{
				// Set the active state of the GameObject           
				GameObject currentObj = selectEnemies[i];
				Button button = selectEnemies[i].GetComponent<Button>();
				int index = i;
				button.onClick.RemoveAllListeners();

				button.onClick.AddListener(() =>
				{
                    
                    TargetEnemy = enemies[index];
					TargetEnemyIndex = index;
                    SkillCast(skillIndex);
                    StartCoroutine(WaitingForNextAction());

                });
			}
		}
    }

    void ShowTargetSelectionUIForAttack()
    {
        
            SetEnemiesActive(true);
            for (int i = 0; i < selectEnemies.Length; i++)
            {
                // Set the active state of the GameObject           
                GameObject currentObj = selectEnemies[i];
                Button button = selectEnemies[i].GetComponent<Button>();
                int index = i;
                button.onClick.RemoveAllListeners();

                button.onClick.AddListener(() =>
                {

                    TargetEnemy = enemies[index];
                    TargetEnemyIndex = index;
                    AttackCast();
                    StartCoroutine(WaitingForNextAction());
                    
                });
            }
        
    }
}

public class TargetOnTurn
{
	public int SelfIndex = 0;
	public Enemy enemy;
	public Character character;
	public float Speed = 0;
	public int turn = 0;
	public bool isEnemy = false;
	public bool isProtect = false;
	public int isBuffAtk = 0;
	public bool isDead = false;

	// Hàm khởi tạo để tạo đối tượng với thông tin cụ thể
	public TargetOnTurn() { }
	public TargetOnTurn(Enemy enemy, Character character, float speed, bool isEnemy, int selfindex)
	{
		this.SelfIndex = selfindex;
		this.enemy = enemy;
		this.character = character;
		this.Speed = speed;
		this.isEnemy = isEnemy;
		this.isDead	= false;
	}
}