using Cysharp.Threading.Tasks;
using NUnit.Framework.Constraints;
using System.Collections.Generic;
using System.Threading.Channels;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;

public interface ICombatTextManager
{
    public bool IsTextOn { get; }
    public UniTask ShowNextRoundText(int round);
    public UniTask ShowAttackWarningText(BaseUnit unit);
    public UniTask ShowSelfAttackText(BaseUnit unit);
    public void OnDamage(BaseUnit target, IDamageInfo damage);
}

[CreateAssetMenu(fileName = "TextManager", menuName = "Manager/TextManager", order = 1)]
public class TextManager : ScriptableObject , ICombatTextManager
{
    [SerializeField] private TextCanvas textCanvasPref;
    [SerializeField] private DamageContainer normalDamageContainer;
    [SerializeField] private DamageContainer criticalDamageContainer;

    private TextCanvas textCanvas;
    private InputHandler inputHandler;

    public void Init()
    {
        textCanvas = Instantiate(textCanvasPref);

        ServiceLocator.For(this)
            .Get(out inputHandler);
    }

    #region[Combat Text]
    public bool IsTextOn { get; private set; } = false;

    public async UniTask ShowNextRoundText(int round)
    {
        BaseText nextRoundText = textCanvas.GetComponentInChildren<NextRoundText>(true);

        nextRoundText.textComp.text = nextRoundText.textComp.text.Replace("{Count}", round.ToString());

        IsTextOn = true;
        await nextRoundText.ShowText(inputHandler);
        IsTextOn = false;

        nextRoundText.textComp.text = nextRoundText.textComp.text.Replace(round.ToString(), "{Count}");
    }

    public async UniTask ShowAttackWarningText(BaseUnit unit)
    {
        BaseText attackWarningText = textCanvas.GetComponentInChildren<AttackWarningText>(true);

        attackWarningText.textComp.text = attackWarningText.textComp.text.Replace("{Name}", unit.GetStat().CoreStat.Name);
        string attackType = unit.GetUnitType() == DataEnum.UNIT_TYPE.MELEE ? "[Melee]" : "[Range]";
        attackWarningText.textComp.text = attackWarningText.textComp.text.Replace("{AttackType}", attackType);
        
        await attackWarningText.ShowText(inputHandler);

        attackWarningText.textComp.text = attackWarningText.textComp.text.Replace(unit.GetStat().CoreStat.Name, "{Name}");
        attackWarningText.textComp.text = attackWarningText.textComp.text.Replace(attackType, "{AttackType}");
    }

    public async UniTask ShowSelfAttackText(BaseUnit unit)
    {
        BaseText selfAttackText = textCanvas.GetComponentInChildren<SelfAttackText>(true);

        selfAttackText.textComp.text = selfAttackText.textComp.text.Replace("{Name}", unit.GetStat().CoreStat.Name);

        await selfAttackText.ShowText(inputHandler);

        selfAttackText.textComp.text = selfAttackText.textComp.text.Replace(unit.GetStat().CoreStat.Name, "{Name}");
    }

    public void OnDamage(BaseUnit target, IDamageInfo damage)
    {
        IDamageValueDisplayer displayer;
        DamageContainer container;
        if (!damage.IsCritical)
        {
            displayer = new DamageValueDisplayer_BounceVer1();
            container = normalDamageContainer;
        }
        else
        {
            displayer = new DamageValueDisplayer_BounceVer2();
            container = criticalDamageContainer;
        }

        IDamageValueDisplayInvoker displayInvoker = new DamageValueDisplayInvoker();
        displayInvoker.Invoke(displayer, damage.DamageValue, target.Attachments.GetHitBox().bounds, container);
    }
    #endregion
}
