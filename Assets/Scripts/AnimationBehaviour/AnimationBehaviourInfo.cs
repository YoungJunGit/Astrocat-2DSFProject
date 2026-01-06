public class AnimationBehaviourInfo
{
    public bool Fired { get; set; }
    public int FullPathHash { get; }
    
    public AnimationBehaviourInfo(bool fired, int fullPathHash)
    {
        Fired = fired;
        FullPathHash = fullPathHash;
    }
}