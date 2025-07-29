using UnityEngine;

[CreateAssetMenu(fileName = "NodeState", menuName = "SO/NodeState", order = 5)]

public class NodeState : ScriptableObject
{
    public enum Type
    {
        Combat,
        Shop,
        Event,
        Chest,
        Rest,
        Boss
    }

    public int layer;
    public Type type;
    public bool is_accesible;

    public string node_id;
    public string event_id;
    public string battle_id;
    public string shop_id;
    public string reward_id;
    public string[] rest_option;
    public string[] connected_node_ids;

    public void init(int layer, Type type, bool accessible) {
        this.layer = layer;
        this.type = type;
        this.is_accesible = accessible;
    }
}
