using Cysharp.Threading.Tasks;
using NUnit.Framework.Constraints;
using System.Collections.Generic;
using System.Threading.Channels;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;

public interface ICombatTextManager
{
    public UniTask<bool> ShowAttackWarningText(BaseUnit unit);
    public void OnDamage(BaseUnit target, IDamage damage);
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

        ServiceLocator.For(this).Get(out inputHandler);
    }

    #region[Combat Text]
    public async UniTask<bool> ShowAttackWarningText(BaseUnit unit)
    {
        Dialogue attackWarningText = textCanvas.GetComponentInChildren<AttackWarningDialogue>(true);

        attackWarningText.textComp.text = attackWarningText.textComp.text.Replace("{Name}", unit.GetStat().coreStat.Name);
        string attackType = unit.GetUnitType() == DataEnum.UNIT_TYPE.MELEE ? "[Melee]" : "[Range]";
        attackWarningText.textComp.text = attackWarningText.textComp.text.Replace("{AttackType}", attackType);
        
        await attackWarningText.ShowDialogue();

        attackWarningText.textComp.text = attackWarningText.textComp.text.Replace(unit.GetStat().coreStat.Name, "{Name}");
        attackWarningText.textComp.text = attackWarningText.textComp.text.Replace(attackType, "{AttackType}");

        return true;
    }
    public void OnDamage(BaseUnit target, IDamage damage)
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
        displayInvoker.Invoke(displayer, damage.Value, target.attachments.GetHitBox().bounds, container);
    }
    #endregion
}
