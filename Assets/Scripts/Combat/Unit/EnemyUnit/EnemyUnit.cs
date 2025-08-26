using UnityEngine;
using DataEntity;
using DataEnum;
using DG.Tweening;
using Cysharp.Threading.Tasks;

public class EnemyUnit : BaseUnit
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
                    attachments.GetSpriteRenderer().DOFade(0, 0.5f).OnComplete(() =>
                    {
                        gameObject.SetActive(false);
                        m_FinishedDying.Invoke(this);
                        isFinishedEvent = true;
                    });
                }
            );

            await UniTask.WaitUntil(() => isFinishedEvent);
        }

        attachments.GetSpriteRenderer().sortingLayerName = "Character";
    }
}
