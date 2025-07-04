using UnityEngine;
using UnityEngine.UI;
using DataEntity;
using System.Collections;
using System.Collections.Generic;

public class StateUnit : MonoBehaviour
{
	public Text unitName;
	public Text hp;
	public Slider hpSlider;

	[SerializeField] private GameObject fill;

	[SerializeField]
	private List<CharacterDataEntity> PlayerData;

	public int basicDamage;
	public int skillDamage;

	public int maxHP;
	public int currentHP;

	public void Initialize(CharacterDataEntity data)
	{
		unitName.text = data.Char_Name;

		basicDamage = (int)data.Char_Default_Attack;
		maxHP = (int)data.Char_Default_HP;
		currentHP = maxHP;

		hpSlider.maxValue = maxHP;
		hpSlider.value = currentHP;
		hp.text = $"HP: {currentHP}";
	}


	/// <summary>
	/// �޾ƿ� dmg�� ������ hp���� ����, �� ������ hp�� üũ�ؼ� 0���� �۰ų� ������ �׾��ٴ� �ǹ��̴� true ��ȯ
	/// 0���� ũ��, ���� ���� �ʾҴٴ� �ǹ��̹� false ��ȯ
	/// BattleSystem��ũ��Ʈ�� PlayAttack()���� �׾������� ���η� BattleState�� state���� ���� (Won�̳� Lost��)
	/// </summary>
	/// <param name="dmg"></param>
	/// <returns></returns>
	public bool TakeDamage(int dmg)
	{
		currentHP -= dmg;

		if (currentHP <= 0)
		{
			hpSlider.value = 0;
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
	/// ������ ȣ��Ǵ� �Լ�
	/// �޾ƿ� ���� ���� ���� ü�¿� ���ϱ�.
	/// ü���� �ƹ��� ���� ȸ���ȴٰ� �ص�, �ִ� ü�� �Ѵ� ���� �ȵǱ� ������
	/// if�������� �ִ� ü�� ����
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
