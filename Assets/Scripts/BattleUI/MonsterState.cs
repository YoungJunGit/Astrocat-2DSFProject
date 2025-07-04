using UnityEngine;
using UnityEngine.UI;
using DataEntity;
using System.Collections;
using System.Collections.Generic;

public class MonsterState : MonoBehaviour
{
	public Text hp;
	public Slider hpSlider;

	[SerializeField] private GameObject fill;

	[SerializeField]
	private List<MonsterDataEntity> MonsterData;

	public int basicDamage;
	public int skillDamage;

	public int maxHP;
	public int currentHP;

	public void Initialize(MonsterDataEntity data)
	{
		basicDamage = (int)data.Mob_Default_Attack;
		maxHP = (int)data.Mob_Default_HP;
		currentHP = maxHP;

		hpSlider.maxValue = maxHP;
		hpSlider.value = currentHP;
		hp.text = $"HP: {currentHP}";
	}


	/// <summary>
	/// 받아온 dmg를 현재의 hp에서 빼고, 뺀 다음의 hp를 체크해서 0보다 작거나 같으면 죽었다는 의미이니 true 반환
	/// 0보다 크면, 아직 죽지 않았다는 의미이미 false 반환
	/// BattleSystem스크립트의 PlayAttack()에서 죽었는지의 여부로 BattleState의 state상태 결정 (Won이냐 Lost냐)
	/// </summary>
	/// <param name="dmg"></param>
	/// <returns></returns>
	public bool TakeDamage(int dmg)
	{
		currentHP -= dmg;

		if (currentHP <= 0)
		{
			hpSlider.value = 0;
			hp.text = "HP: 0";
			fill.SetActive(false);
			return true;
		}
		else
		{
			SetHP();
			return false;
		}
	}

	/// <summary>
	/// 힐링시 호출되는 함수
	/// 받아온 힐링 양을 현재 체력에 더하기.
	/// 체력이 아무리 많이 회복된다고 해도, 최대 체력 넘는 것은 안되기 때문에
	/// if구문으로 최대 체력 제한
	/// </summary>
	/// <param name="amount"></param>
	public void Heal(int amount)
	{
		currentHP += amount;
		if (currentHP > maxHP)
			currentHP = maxHP;
	}

	public void SetHP()
	{
		hpSlider.value = currentHP;
		hp.text = $"HP: {currentHP}";
	}

}
