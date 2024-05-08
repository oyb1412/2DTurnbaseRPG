## **📃핵심 기술**

### ・턴 시스템을 상태 패턴으로 구현

🤔**WHY?**

각 플레이어의 공격 순서를 속도에 맞춰 지정 후, 대기, 공격, 스킬 등 각 행동에 맞춰 턴을 진행하는 시스템 구축

🤔**HOW?**

 관련 코드

- BattleManager
    
    ```csharp
    using System.Collections;
    using System.Linq;
    using UnityEngine;
    using UnityEngine.UI;
    using DG.Tweening;
    
    public class BattleManager : MonoBehaviour
    {
        public enum BattleState
        {
            main, skillselect, targetselect, attack, Check, changePlayer, Win, Lose,
            oneTargetSkill, allTargetSkill, Heal, allTargetSkillAttack, oneTargetSkillAttack, useHeal,
            allTargetSkillCheck,SkillList
        }
    
    	   void Update()
      {
          if (!GameManager.instance.isPlaying)
              return;
    
               switch (StateManager.instance.currentState)
              {
    
                  case StateManager.BattleState.StartTurn:
    
                  chars = RealizeChar();
                  battleInfoImage.gameObject.SetActive(true);
                      state = BattleState.main;
                      if (chars[0].GetComponentInChildren<PlayerManager>())
                          StateManager.instance.currentState = StateManager.BattleState.PlayerTurn;
                       else
                          StateManager.instance.currentState = StateManager.BattleState.EnemyTurn;
                  
                  break;
                  case StateManager.BattleState.PlayerTurn:
                  switch (state)
                  {
                      case BattleState.main:
                          players = RealizePlayer();
                          enemys = RealizeEnemy();
    
                          for (int i = 0;i<players.Length; i++)
                          {
                              if (chars[myTurn].charName == gameData[i].PlayerName)
                              {
                                  currentPlayerNumber = i;
                                  break;
                              }
                          }
                          if(!trigger)
                          {
                              battleInfoText.text = "";
                              battleInfoText.DOText(string.Format("{0}'s Turn", chars[myTurn].charName), 1);
                              StartCoroutine(ChangeState(BattleState.skillselect, 1.5f));
                              trigger = true;
                          }
                          break;
                      case BattleState.skillselect:
                          ButtonInfo();
                          if (gameData[currentPlayerNumber].playerCurrenMp < 10)
                          {
                              buttons[1].interactable = false;
                          }
    
                          buttonsParent.gameObject.SetActive(true);
                          arrowImage.gameObject.SetActive(true);
    
                          buttonsParent.transform.position = new Vector3(chars[myTurn].transform.position.x+3.5f,
                              chars[myTurn].transform.position.y+1.5f,
                              chars[myTurn].transform.position.z);
    
                          if (!trigger)
                          {
                              battleInfoText.text = "";
                              battleInfoText.DOText("Select Your Action",1);
                              trigger = true;
                          }
    
                          break;
                      case BattleState.SkillList:
                          SkillButtonInfo();
                          skillButtons.transform.position = new Vector3(chars[myTurn].transform.position.x + 3.5f,
                              chars[myTurn].transform.position.y + 1.5f,
                              chars[myTurn].transform.position.z);
                          if(!trigger)
                          {
                              battleInfoText.text = "";
                              battleInfoText.DOText("Select your Skill", 1);
                              trigger = true;
                          }
                          if (Input.GetMouseButtonDown(1))
                          {
                              AudioManager.instance.PlayerSfx(AudioManager.Sfx.Select);
                              SkillReturn();
                          }
                          break;
                      case BattleState.allTargetSkill:
    
                          if (!trigger)
                          {
                              battleInfoText.text = "";
                              battleInfoText.DOText(string.Format("MP -10 {0} Damage All Enemies", gameData[currentPlayerNumber].playerAttackDamage), 1);
                              StartCoroutine(ChangeState(BattleState.allTargetSkillAttack, 1.5f));
    
                              trigger = true;
                          }
                          break;
                      case BattleState.oneTargetSkill:
                          target = SelectEnemy();
                          if (!trigger)
                          {
                              battleInfoText.text = "";
                              battleInfoText.DOText(string.Format("MP -10 {0} Damage One Enemy", gameData[currentPlayerNumber].playerAttackDamage * 2), 1);
                              trigger = true;
                          }
                          if (target != null)
                          {
                              targetEnemy = target.gameObject.GetComponent<EnemyManager>();
                              trigger = false;
                              state = BattleState.oneTargetSkillAttack;
                          }
                          break;
                      case BattleState.Heal:
                          if(!trigger)
                          {
                              battleInfoText.text = "";
                              battleInfoText.DOText(string.Format("MP -10 {0} HP Self Healing", gameData[currentPlayerNumber].skillDamage), 1);
                              trigger = true;
                          }
                          ChangeState(BattleState.useHeal);
                          break;
                      case BattleState.allTargetSkillAttack:
    
                          if (!trigger)
                          {
                              battleInfoText.text = "";
                              battleInfoText.DOText(string.Format("{0} Damage All Enemies", gameData[currentPlayerNumber].playerAttackDamage), 1);
                              gameData[currentPlayerNumber].playerCurrenMp -= 10;
                              chars[myTurn].SetAttack = true;
                              trigger = true;
                          }
                          chars[myTurn].AllTargetAttack(enemys, 3);
    
                          if (!chars[myTurn].SetAttack && !attackTrigger)
                          {
                              StartCoroutine(ChangeState(BattleState.allTargetSkillCheck, 1.5f));
                              attackTrigger = true;
                          }
    
                          break;
                      case BattleState.allTargetSkillCheck:
                          if (!trigger)
                          {
                              battleInfoText.text = "";
                              battleInfoText.DOText(string.Format("Enemy Dead a {0}'s Attack", chars[myTurn].charName), 1);
                              for (int i = 0; i < enemys.Length; i++)
                              {
                                  if (enemys[i].currentHp <= 0)
                                  {
                                      deadCount++;
                                  }
                              }
                              trigger = true;
                          }
                          if (deadCount > 0)
                          {
                              chars = chars.Where(x => null != x).ToArray(); //null 배열만 지움
    
                              if (GameObject.FindGameObjectsWithTag("Enemy").Length > 0)
                              {
                                  trigger = false;
                                  state = BattleState.changePlayer;
                              }
                              else
                              {
                                  if(trigger)
                                  {
                                      battleInfoText.text = "Win!!!";
                                      trigger = false;
                                  }
    
                                  int enemyType = PlayerPrefs.GetInt("colliderEnemyType");
                                  switch (enemyType)
                                  {
                                      case 0:
                                          gold = Random.Range(60, 80);
                                          break;
                                      case 1:
                                          gold = Random.Range(70, 90);
                                          break;
                                      case 2:
                                          gold = Random.Range(80, 100);
                                          break;
                                  }
    
                                  ChangeState(BattleState.Win);
                               }
                          }
                          else
                          {
                              trigger = false;
                              state = BattleState.changePlayer;
                          }
                          
                          break;
                      case BattleState.oneTargetSkillAttack:
    
                          if (!trigger)
                          {
                              battleInfoText.text = "";
                              battleInfoText.DOText(string.Format("{0} Damage to {1}", chars[myTurn].attackDamage * 2f, targetEnemy.charName),1);
                              gameData[currentPlayerNumber].playerCurrenMp -= 10;
                              chars[myTurn].SetAttack = true;
                              trigger = true;
                          }
                          chars[myTurn].Attack(targetEnemy, 2, 2);
                          if (!chars[myTurn].SetAttack && !attackTrigger)
                          {
                              StartCoroutine(ChangeState(BattleState.Check, 1.5f));
                              attackTrigger = true;
                          }
                          break;
                      case BattleState.useHeal:
                          if (!trigger)
                          {
                              AudioManager.instance.PlayerSfx(AudioManager.Sfx.Heal);
    
                              gameData[currentPlayerNumber].playerCurrenMp -= 10;
                              chars[myTurn].SetHeal = true;
                              battleInfoText.text = "";
                              battleInfoText.DOText(string.Format("{0} HP Recovery", (gameData[currentPlayerNumber].skillDamage)),1);
    
                              gameData[currentPlayerNumber].playerCurrentHp += gameData[currentPlayerNumber].skillDamage;
                              if (gameData[currentPlayerNumber].playerCurrentHp >= gameData[currentPlayerNumber].playerMaxHp)
                                  gameData[currentPlayerNumber].playerCurrentHp = gameData[currentPlayerNumber].playerMaxHp;
    
                                  Transform effect = Instantiate(chars[myTurn].effectPrefabs[0], chars[myTurn].transform).transform;
                              effect.transform.position = new Vector3(chars[myTurn].transform.position.x,
                                  chars[myTurn].transform.position.y+0.5f,
                                  chars[myTurn].transform.position.z);
                              trigger = true;
    
                              StartCoroutine(ChangeState(BattleState.changePlayer, 1.5f));
    
                          }
                          break;
                      case BattleState.targetselect:
                          if(!trigger)
                          {
                              battleInfoText.text = "";
                              battleInfoText.DOText("Select a Target",1);
                              trigger = true;
                          }
                          PlyerInfo();
                          target = SelectEnemy();
                          if (target != null)
                          {
                              targetEnemy = target.gameObject.GetComponent<EnemyManager>();
                              state = BattleState.attack;
                              trigger = false;
                          }
                          break;
                      case BattleState.attack:
    
                          if(!trigger)
                          {
                              battleInfoText.text = "";
                              battleInfoText.DOText(string.Format("{0} Damage to {1}", gameData[currentPlayerNumber].playerAttackDamage, targetEnemy.charName), 1);
                              chars[myTurn].SetAttack = true;
                              trigger = true;
                          }
                          chars[myTurn].Attack(targetEnemy,1, 1);
                          if (!chars[myTurn].SetAttack && !attackTrigger)
                          {
                              attackTrigger = true;
                              StartCoroutine(ChangeState(BattleState.Check, 1.5f));
                          }
    
                          break;
                      case BattleState.Check:
                          if (targetEnemy.currentHp <= 0)
                          {
                              if (GameObject.FindGameObjectsWithTag("Enemy").Length > 0)
                              {
                                  chars = chars.Where(x => null != x).ToArray();
                                  trigger = false;
                                  state = BattleState.changePlayer;
                              }
                              else
                              {
                                  if(!trigger)
                                  {
                                      battleInfoText.text = "";
                                      battleInfoText.DOText("Win!!!", 1);
                                      trigger = true;
                                  }
    
                                  int enemyType = PlayerPrefs.GetInt("colliderEnemyType");
                                  switch (enemyType)
                                  {
                                      case 0:
                                          gold = Random.Range(60, 80);
                                          break;
                                      case 1:
                                          gold = Random.Range(70, 90);
                                          break;
                                      case 2:
                                          gold = Random.Range(80, 100);
                                          break;
                                  }
                                  if (Input.GetMouseButtonDown(0))
                                  {
                                      AudioManager.instance.PlayerSfx(AudioManager.Sfx.Select);
    
                                      GameManager.instance.SetPanel(true);
    
                                      gameData[0].currentGold = gameData[0].currentGold + (gold * gameData[0].currentPlayerNumber);
                                      for (int i = 0; i < players.Length; i++)
                                      {
    
                                          gameData[i].PlayerCurrendExp += gold + 10;
                                          if (gameData[i].PlayerCurrendExp >= gameData[i].PlayerMaxExp)
                                          {
                                              gameData[i].PlayerCurrendExp = gameData[i].PlayerMaxExp - gameData[i].PlayerCurrendExp;
                                              gameData[i].playerLevel++;
                                              gameData[i].playerAttackDamage += 2;
                                              gameData[i].skillDamage += 2;
    
                                          }
                                      }
                                      trigger = false;
                                      state = BattleState.Win;
                                  }
    
                              }
                          }
                          else
                          {
                              trigger = false;
                              state = BattleState.changePlayer;
                          }
                          break;
                      case BattleState.Win:
                          if(!trigger)
                          {
                              battleInfoText.text = "";
                              battleInfoText.DOText(string.Format("Get {0} Gold! Get {1} Exp!", gold * gameData[0].currentPlayerNumber, gold + 10), 1);
                              trigger = true;
                          }
                          if (Input.GetMouseButtonDown(0))
                          {
    
                              AudioManager.instance.PlayerSfx(AudioManager.Sfx.GoScene);
    
                              GameManager.instance.SetPanel(false);
                              AudioManager.instance.PlayerBgm(AudioManager.Bgm.Battle, false);
                              GameManager.instance.ActionFade((int)GameManager.Scenes.FieldScene);
                          }
                          break;
                      case BattleState.changePlayer:
    
                          //순번이 캬라수를 넘어가면 순번0으로 초기화
                          if (myTurn +1 >= chars.Length)
                              myTurn = -1;
    
                              if (chars.Length > myTurn + 1)
                              {
    
                              if (chars[myTurn + 1].GetComponent<EnemyManager>())
                              {
                                      myTurn++;
                                      attackTrigger = true;
                                      state = BattleState.main;
                                      StateManager.instance.currentState = StateManager.BattleState.EnemyTurn;
                              }
                              else
                              {
                                      myTurn++;
                                      attackTrigger = true;
                                      state = BattleState.main;
                              }
                              attackTrigger = false;
    
                          }
                          break;
                  }
                  break;
                  case StateManager.BattleState.EnemyTurn:
                  switch (state)
                  {
                      case BattleState.main:
                          players = RealizePlayer();
                          enemys = RealizeEnemy();
                          if(!trigger)
                          {
                              battleInfoText.text = "";
                              battleInfoText.DOText(string.Format("{0}'s Turn", chars[myTurn].charName), 1);
                              trigger = true;
                              StartCoroutine(ChangeState(BattleState.targetselect, 1.5f));
                          }
                          break;
                      case BattleState.targetselect:
                          players = SelectPlayer(); //데이터 기준으로
                          target = players[0].GetComponent<Collider2D>();
          
                          if (target != null)
                          {
                              targetPlayer = target.gameObject.GetComponent<PlayerManager>();
                              state = BattleState.attack;
                          }
                          break;
                      case BattleState.attack:
    
                          if (!trigger)
                          {
                              battleInfoText.text = "";
                              battleInfoText.DOText(string.Format("{0}'s got {1} Damage", targetPlayer.charName, chars[myTurn].attackDamage), 1);
                              chars[myTurn].SetAttack = true;
                              trigger = true;
                          }
                          chars[myTurn].Attack(targetPlayer);
                          if (!chars[myTurn].SetAttack && !attackTrigger)
                          {
                              attackTrigger = true;
                              StartCoroutine(ChangeState(BattleState.Check, 1.0f));
                          }
    
                          break;
                      case BattleState.Check:
                          if (targetPlayer.gameData.playerCurrentHp <= 0)
                          {
                              chars = chars.Where(x => null != x).ToArray(); //null 배열만 지움
                              //if(!trigger)
                              //{
                              //    battleInfoText.text = "";
                              //    battleInfoText.DOText(string.Format("{0}'s Attack {1}'s Dead", chars[myTurn].charName, targetPlayer.charName), 1);
                              //    trigger = true;
                              //}
                              if (GameObject.FindGameObjectsWithTag("Player").Length > 0)
                              {
                                  trigger = false;
                                  state = BattleState.changePlayer;
                              }
                              else
                              {
                                  if(!trigger)
                                  {
                                      battleInfoText.text = "";
                                      battleInfoText.text = "Lose...";
                                      trigger = true;
                                      StartCoroutine(ChangeState(BattleState.Lose, 1.5f));
                                  }
                              }
                          }
                          else
                          {
                              trigger = false;
                              state = BattleState.changePlayer;
                          }
                          break;
                      case BattleState.Lose:
                          if(!trigger)
                          {
                              battleInfoText.text = "";
                              battleInfoText.DOText("Lost 100 Gold and EXP...", 1);
                              GameManager.instance.SetPanel(false);
                              trigger = true;
                          }
                          if (Input.GetMouseButtonDown(0))
                          {
                              AudioManager.instance.PlayerSfx(AudioManager.Sfx.GoScene);
    
                              if (gameData[0].currentGold > 100)
                              {
                                  gameData[0].currentGold -= 100;
                              }
                              if (gameData[0].currentGold <= 100)
                              {
                                  gameData[0].currentGold = 0;
                              }
                              for(int i = 0; i < gameData.Length; i++)
                              {
                                  gameData[i].PlayerCurrendExp = 0;
                                  gameData[i].playerCurrentHp = gameData[i].playerMaxHp;
                              }
                              gameData[0].playerFieldPosition = new Vector2(-9.3f, -9.5f);
                              AudioManager.instance.PlayerBgm(AudioManager.Bgm.Battle, false);
                              GameManager.instance.ActionFade((int)GameManager.Scenes.FieldScene);
                          }
                          break;
                      case BattleState.changePlayer:
    
                          if (myTurn + 1 >= chars.Length)
                              myTurn = -1;
                          //공격 후 다음순번 플레이어 선택
                          if (chars.Length > myTurn + 1)
                          {
                              if (chars[myTurn + 1].GetComponent<EnemyManager>())
                              {
                                      myTurn++;
                                      trigger = false;
                                      state = BattleState.main; 
                              }
                              else
                              {
                                      myTurn++;
                                      trigger = false;
                                      state = BattleState.main;
                                      StateManager.instance.currentState = StateManager.BattleState.PlayerTurn;
                              }
                          }
                          break;
                  }
                  break;
              }
      }
    }
    ```
    

🤓**Result**

유닛의 행동들을 상태로 나눈 후, 상태를 변경해가며 상태에 맞는 로직을 실행하는 상태 패턴을 구현해 상태의 추가, 제거, 수정 등의 유지보수의 용이성 증가

### ・다형성을 활용한 플레이어 턴 순서 지정

🤔**WHY?**

모든 유닛의 공격 속도를 비교해 플레이어의 턴 순서를 정해야 하지만, 플레이어 유닛/ 적 유닛의 별개의 클래스로 나누어져 있어 이들을 묶어 비교하는 것이 불가능했음.

🤔**HOW?**

 관련 코드

- BattleManager
    
    ```csharp
    using System.Collections;
    using System.Linq;
    using UnityEngine;
    using UnityEngine.UI;
    using DG.Tweening;
    
    public class BattleManager : MonoBehaviour
    {
       CharBase[] RealizeChar()
    	{
        
        CharBase[] charObj = FindObjectsOfType<CharBase>();
    
        
        for (int j = 0; j < charObj.Length; j++)
        {
            for (int h = 0; h < charObj.Length - 1; h++)
            {
    
                if (charObj[h].attackOrder < charObj[h + 1].attackOrder)
                {
                    CharBase save = new CharBase();
                    save = charObj[h + 1];
                    charObj[h + 1] = charObj[h];
                    charObj[h] = save;
                }
            }
        }
        return charObj;
    	}
    }
    ```
    

🤓**Result**

플레이어 유닛 / 적 유닛 클래스에 동일 클래스를 부모 클래스로 지정해, 부모 클래스 인스턴스 변수로 플레이어 / 적 유닛 모두를 통합해 공격 속도를 계산, 턴 순서를 지정하는 것이 가능해짐

### ・타일맵 충돌 판정

🤔**WHY?**

산, 나무, 강 등 다양한 형태의 타일맵들과 플레이어간의 충돌 판정 구현

🤔**HOW?**

 관련 이미지

- BattleManager
    
    ![turn.png](https://prod-files-secure.s3.us-west-2.amazonaws.com/7becc665-a2c0-47ed-8a12-3582bd47e71e/392b4708-d694-49e1-b6fe-82191638f973/turn.png)
    

🤓**Result**

충돌이 필요한 타일맵 및 강/다리 등 곂쳐있는 타일맵과 플레이어 캐릭터간의 충돌판정을 구현
