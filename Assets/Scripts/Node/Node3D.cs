using Cysharp.Threading.Tasks;
using S3MG;
using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(Collider2D), typeof(AudioSource))]
public class Node3D : MonoBehaviour, IUpdateObserver
{
    [Header("Node Properties")]
    public int floor { get; set; }
    public int route { get; set; }
    public List<Node3D> prevNodes { get; set; } = new List<Node3D>();
    public List<Node3D> nextNodes { get; set; } = new List<Node3D>();

    public bool connected { get; set; } = false;
    public bool visited { get; set; } = false;
    public NodeData.Type? nodeType { get; set; } = null;

    [Header("Visuals")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private SpriteRenderer highlightRenderer;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private Color highlightColor = Color.gray;
    [SerializeField] private Color defaultColor = Color.white;

    private NodeMapGenerator3D _mapGenerator;
    private bool isHovered = false;
    private float lerpTimer = 0f;
    private float lerpDuration = 0.8f;
    private bool isForward = true;

    private float fillTime = 0.4f;
    private float elapsedTime = 0f;
    private bool onButton = false;

    private void Awake()
    {
        if (!spriteRenderer) spriteRenderer = GetComponent<SpriteRenderer>();
        if (!audioSource) audioSource = GetComponent<AudioSource>();
        if (!highlightRenderer)
        {
            GameObject highlight = new GameObject("Highlight");
            highlight.transform.SetParent(transform);
            highlight.transform.localPosition = Vector3.zero;
            highlightRenderer = highlight.AddComponent<SpriteRenderer>();
            highlightRenderer.sprite = spriteRenderer.sprite;
            highlightRenderer.color = new Color(0, 0, 0, 0);
            highlightRenderer.sortingOrder = spriteRenderer.sortingOrder - 1;
        }
    }

    public void Init(NodeMapGenerator3D mapGenerator)
    {
        _mapGenerator = mapGenerator;
        UpdatePublisher.SubscribeObserver(this);
    }

    public void ObserverUpdate(float dt)
    {
        if (visited) return;

        float t = (Time.time - lerpTimer) / lerpDuration;
        if (t > 1f)
        {
            t = 0f;
            lerpTimer = Time.time;
            isForward = !isForward;
        }

        if (isHovered)
        {
            spriteRenderer.color = Color.Lerp(defaultColor, highlightColor, Mathf.PingPong(Time.time, 1));
        }

        if (onButton) FillUp();
    }

    public void SetNodeData(Sprite sprite, NodeData.Type? type, string name = "")
    {
        spriteRenderer.sprite = sprite;
        nodeType = type;
        gameObject.name = name;
    }

    private void OnMouseEnter()
    {
        if (!visited)
        {
            isHovered = true;
        }
    }

    private void OnMouseExit()
    {
        isHovered = false;
        spriteRenderer.color = defaultColor;
    }

    private void OnMouseDown()
    {
        if (visited) return;
        onButton = true;
        elapsedTime = 0f;
    }

    private void FillUp()
    {
        if (elapsedTime < fillTime)
        {
            elapsedTime += Time.deltaTime;
        }
        else
        {
            audioSource.Play();
            EntryStart();
        }
    }

    private void EntryStart()
    {
        visited = true;
        onButton = false;
        spriteRenderer.color = Color.gray;

        if (nodeType != NodeData.Type.Start) _mapGenerator.PaintPath(this);

        _mapGenerator.nowNode = this;

        switch (nodeType)
        {
            case NodeData.Type.Start: HandleStart(); break;
            case NodeData.Type.Camp: HandleCamp(); break;
            case NodeData.Type.Shop: HandleShop(); break;
            case NodeData.Type.Event: HandleEvent(); break;
            case NodeData.Type.Treasure: HandleTreasure(); break;
            case NodeData.Type.Trap: HandleTrap(); break;
            case NodeData.Type.Enemy: HandleEnemy(); break;
            case NodeData.Type.Middle: HandleMiddle(); break;
            case NodeData.Type.Final: HandleFinal(); break;
            case NodeData.Type.Random: HandleRandom(); break;
            default: Debug.Log("No node type set"); break;
        }
    }

    #region Node Type Handlers
    private void HandleStart() => _mapGenerator.ToNextNode();
    private void HandleCamp() => Debug.Log("Open a camp");
    private void HandleShop() => Debug.Log("Open a shop");
    private void HandleEvent() => Debug.Log("Execute event");
    private void HandleTreasure() => Debug.Log("Obtain treasure");
    private void HandleTrap() => Debug.Log("Execute trap");
    private void HandleEnemy() => ChangeScene(3);
    private void HandleMiddle() => Debug.Log("Midpoint");
    private void HandleFinal() => Debug.Log("Fight the final boss");
    private void HandleRandom() => Debug.Log("Execute random process");
    #endregion

    private void ChangeScene(int sceneNum)
    {
        UpdatePublisher.DiscribeObserver(this);
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneNum);
    }

    private void OnDestroy()
    {
        UpdatePublisher.DiscribeObserver(this);
    }
}