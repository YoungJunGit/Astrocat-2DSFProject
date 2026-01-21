using Cysharp.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "QTEManager", menuName = "GameScene/QTEManager")]
public class QTEManager : ScriptableObject, IUpdateObserver
{
    [SerializeField] private QTEUI qteUIPrefab;
    [SerializeField] private InputHandler inputHandler;
    private QTEUI _qteUI;
    
    private bool _onQTEInteracted = false;
    private float _time = 0;
    
    public enum QTEResult
    {
        Normal,
        Perfect,
        Failure
    }

    public void Init()
    {
        _qteUI = Instantiate(qteUIPrefab);
        _qteUI.Init();
    }

    public async UniTask<QTEResult> StartSingleQTE(float time = 1f, Vector2 position = new Vector2())
    {
        QTEResult result = QTEResult.Normal; 
        
        using (var inputDisposer = new InputDisposer(inputHandler, InputHandler.InputState.QTE))
            using (var qteuiDisposer = new QteuiDisposer(_qteUI, time, position))
                using (var updateDisposer = new UpdateDispoer(this))
        {
            _time = time;
            
            UniTask onQTEInteracted = UniTask.WaitUntil(() => _onQTEInteracted);
            UniTask onTimeEnd = UniTask.WaitUntil(() => _time <= 0);
            
            await UniTask.WhenAny(onQTEInteracted, onTimeEnd);
            
            // TODO : Set result by _time
        }
        
        return result;
    }

    public void ObserverUpdate(float dt)
    {
        _time -= dt;
        _qteUI.UpdateQteGage(dt);
    }
}