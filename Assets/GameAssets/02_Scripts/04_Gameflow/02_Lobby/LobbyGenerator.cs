using UnityEngine;

[CreateAssetMenu(fileName = "LobbyGenerator", menuName = "LobbyGenerator")]
public class LobbyGenerator : ScriptableObject
{
    [Header("Background")]
    [SerializeField]
    private GameObject cameraRigPref;
    [SerializeField]
    private GameObject spaceshipRootPref;
    [SerializeField]
    private GameObject dialogueSystemPref;
    [SerializeField]
    private ParticleSystem starParticle;

    private CameraMove camMove;

    public void Init()
    {
        camMove = Instantiate(cameraRigPref).GetComponent<CameraMove>();
        Instantiate(spaceshipRootPref);
        Instantiate(dialogueSystemPref);
        Instantiate(starParticle);

        if(camMove != null)
        {
            camMove.StartCameraMove();
            UpdatePublisher.SubscribeObserver(camMove);
            ServiceLocator.ForSceneOf(this)
                .Register(camMove as ICameraMove); 
        }
    }

    public void DisableCamera()
    {
        UpdatePublisher.DiscribeObserver(camMove);
    }
}
