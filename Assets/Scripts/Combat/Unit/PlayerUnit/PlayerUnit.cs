using Cysharp.Threading.Tasks;
using DataEntity;
using DataEnum;
using DG.Tweening;
using UnityEngine;

public class PlayerUnit : BaseUnit
{
    public async override UniTask OnDie()
    {
        attachments.GetSpriteRenderer().sortingLayerName = "Actor";

        using (var eventDisposer = new EventDisposer(new CombatEvent("DeathEvent")))
        {
            bool isFinishedEvent = false;

            base.OnDie();
            combatInfo.actionList.Add("OnFinishedDeath", () =>
                {
                    gameObject.SetActive(false);
                    m_FinishedDying.Invoke(this);
                    isFinishedEvent = true;
                }
            );

            await UniTask.WaitUntil(() => isFinishedEvent);
        }

        attachments.GetSpriteRenderer().sortingLayerName = "Character";
    }
}
