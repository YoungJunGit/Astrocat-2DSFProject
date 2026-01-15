using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;

public interface ICombatTextManager
{
    public bool IsTextOn { get; }
    public UniTask ShowCombatText(IUnitActionProperty unitAction, BaseUnit unit);
    public UniTask ShowNextRoundText(int round);
    public UniTask ShowTauntText(BaseUnit unit);
    public void OnDamage(BaseUnit target, IDamageInfo damage);
}

[CreateAssetMenu(fileName = "TextManager", menuName = "Manager/TextManager", order = 1)]
public class TextManager : ScriptableObject , ICombatTextManager
{
    private BaseCanvas _textCanvas;
    private WarningText _warningText;
    private InputHandler _inputHandler;

    [SerializeField] private BaseCanvas textCanvasPref;
    [SerializeField] private DamageContainer normalDamageContainer;
    [SerializeField] private DamageContainer criticalDamageContainer;

    [SerializeField] BaseTextSetting nextRoundTextSetting;
    [SerializeField] BaseTextSetting attackWarningTextSetting;
    [SerializeField] BaseTextSetting selfAttackTextSetting;
    [SerializeField] BaseTextSetting skillWarningTextSetting;
    [SerializeField] BaseTextSetting tauntTextSetting;

    // Temporary - String DT가 완성되기 전까지 사용
    [SerializeField, TextArea(4, 10)] string nextRoundText;
    [SerializeField, TextArea(4, 10)] string attackWarningText;
    [SerializeField, TextArea(4, 10)] string selfAttackText;
    [SerializeField, TextArea(4, 10)] string skillWarningText;
    [SerializeField, TextArea(4, 10)] string tauntText;

    public void Init()
    {
        _textCanvas = Instantiate(textCanvasPref);
        _textCanvas.Init();

        _warningText = _textCanvas.GetComponentInChildren<WarningText>(true);

        ServiceLocator.For(this).Get(out _inputHandler);
    }

    #region[Combat Text]

    public async UniTask ShowCombatText(IUnitActionProperty unitAction, BaseUnit unit)
    {
        switch(unitAction)
        {
            case BaseAttackAction:
                await ShowAttackWarningText(unit);
                break;
            case SelfAttackAction:
                await ShowSelfAttackText(unit);
                break;
            case ISkillAction skill:
                await ShowSkillWarningText(unit, skill.Data.Skill_Name);
                break;
        }
    }

    public bool IsTextOn { get; private set; } = false;
    public async UniTask ShowNextRoundText(int round)
    {
        _warningText.SetText(nextRoundText);
        _warningText.ReplaceText("{Count}", round.ToString());

        IsTextOn = true;
        await _warningText.ShowTextWith(nextRoundTextSetting, _inputHandler);
        IsTextOn = false;
    }

    public async UniTask ShowTauntText(BaseUnit unit)
    {
        _warningText.SetText(tauntText);
        _warningText.ReplaceText("{Name}", unit.GetStat().CoreStat.Name);

        await _warningText.ShowTextWith(tauntTextSetting, _inputHandler);
    }

    private async UniTask ShowAttackWarningText(BaseUnit unit)
    {
        _warningText.SetText(attackWarningText);
        _warningText.ReplaceText("{Name}", unit.GetStat().CoreStat.Name);

        await _warningText.ShowTextWith(attackWarningTextSetting, _inputHandler);
    }

    private async UniTask ShowSelfAttackText(BaseUnit unit)
    {
        _warningText.SetText(selfAttackText);
        _warningText.ReplaceText("{Name}", unit.GetStat().CoreStat.Name);

        await _warningText.ShowTextWith(selfAttackTextSetting, _inputHandler);
    }

    private async UniTask ShowSkillWarningText(BaseUnit unit, string skillName)
    {
        _warningText.SetText(skillWarningText);
        _warningText.ReplaceText("{Name}", unit.GetStat().CoreStat.Name);
        _warningText.ReplaceText("{SkillName}", skillName);

        await _warningText.ShowTextWith(skillWarningTextSetting, _inputHandler);
    }
    #endregion

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

        IDamageHealValueDisplayInvoker displayInvoker = new DamageHealValueDisplayInvoker();
        displayInvoker.Invoke(displayer, damage.DamageValue, target.Attachments.GetHitBox().bounds, container);
    }
}
