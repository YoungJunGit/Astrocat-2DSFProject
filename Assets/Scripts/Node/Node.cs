using DG.Tweening;
using S3MG;
using System;
using System.Collections.Generic;
using TMPro;
using Tymski;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Node : MonoBehaviour, IUpdateObserver
{

    [SerializeField] public int floor { get; set; }
    [SerializeField] public int route { get; set; }

    [SerializeField] public List<Node> prevNodes { get; set; } = new List<Node>();
    [SerializeField] public List<int> prevNodesIdx { get; set; } = new List<int>();
    [SerializeField] public List<Node> nextNodes { get; set; } = new List<Node>();
    [SerializeField] public List<int> nextNodesIdx { get; set; } = new List<int>();

    [SerializeField] public bool connected { get; set; } = false;
    [SerializeField] public bool visited { get; set; } = false;
    [SerializeField] public bool isActive { get; set; } = false;
    [SerializeField] public int idx { get; set; } = 0;
    [SerializeField] public float xPos { get; set; }
    [SerializeField] public float yPos { get; set; }
    [SerializeField] public float zPos { get; set; }

    [SerializeField] public NodeType nodeType { get; set; }
    [SerializeField] public string nodeName { get; set; }

    [SerializeField] SpriteRenderer nodeImage;
    [SerializeField] SpriteRenderer visitImage;
    [SerializeField] TextMesh nodeText;
    [SerializeField] AudioSource AS;
    [SerializeField] SceneReference nextScene;

    [SerializeField] float lerpDuration = 0.8f;
    [SerializeField] Color lerpColor = Color.gray;
    Color defaultColor = Color.white;
    Color currentColor;
    bool isMousePointerOn = false;
    bool isForward = true;
    float lerpStartTime;

    [SerializeField] public bool onButton { get; set; } = false;
    float fillTime = 0.4f;
    float elapsedTime = 0f;

    NodeMapGenerator _mapGenerator;
    Camera _3DCamera;
    AbstractScene _scene;

    public TextMesh NodeText => nodeText;

    /*------------------------------------------------------------
    Executed only once when MonoBehaviour is created, Will work if the GameObject is active even if the component is disabled
    ------------------------------------------------------------*/
    public void Init(NodeMapGenerator mapGenerator)
    {
        _mapGenerator = mapGenerator;
        currentColor = defaultColor;
        ServiceLocator.ForSceneOf(this)
            .Get(out _3DCamera)
            .Get(out _scene);
        UpdatePublisher.SubscribeObserver(this);
    }

    private void OnMouseEnter()
    {
        if (!isActive || visited) return;
        isMousePointerOn = true;
        nodeImage.color = Color.red;   
    }

    private void OnMouseExit()
    {
        if (!isActive || visited) return;
        isMousePointerOn = false;
        OnDefaultColor();
    }

    private void OnMouseDown()
    {
        if (!isActive || visited) return;
        isMousePointerOn = false;
        onButton = true;
    }

    /*------------------------------------------------------------
    Called once per frame, Executed when the GameObject and component are enabled
    ------------------------------------------------------------*/
    public void ObserverUpdate(float dt)
    {
        if(_3DCamera != null)
        {
            transform.LookAt(_3DCamera.transform.position);
        }
        if(!isActive || visited || isMousePointerOn) return;

        float t = (Time.time - lerpStartTime) / lerpDuration;
        if (t > 1f)
        {
            t = 0f;
            lerpStartTime = Time.time;
            isForward = !isForward;
            currentColor = isForward ? defaultColor : lerpColor;
        }
        nodeImage.color = Color.Lerp(currentColor, isForward ? lerpColor : defaultColor, t);

        if (onButton) fillUp();
    }

    /*------------------------------------------------------------
    Set image, type, and text on a node
    ------------------------------------------------------------*/
    public void setNodeData(Sprite sprite, NodeType type, string text = "")
    {
        nodeImage.sprite = sprite;
        nodeType = type;
        nodeText.text = text;
    }

    /*------------------------------------------------------------
    Enable the Button component
    ------------------------------------------------------------*/
    public void enableButton()
    {
        isActive = true;
    }

    /*------------------------------------------------------------
    Disable the Button component
    ------------------------------------------------------------*/
    public void disableButton()
    {
        isActive = false;
    }

    /*------------------------------------------------------------
    Mark as passed
    ------------------------------------------------------------*/
    public void passedNode()
    {
        //disableButton();
        if (visited)
        {
            nodeImage.color = defaultColor;
        }
        else
        {
            nodeImage.color = Color.gray;
            onButton = false;
            fillReset();
        }
    }

    public void OnDefaultColor()
    {
        nodeImage.color = defaultColor;
    }

    /*------------------------------------------------------------
    Fill up
    ------------------------------------------------------------*/
    void fillUp()
    {
        Color color = visitImage.color;
        if (elapsedTime < fillTime)
        {
            float amountValue = Mathf.Lerp(0f, 1.0f, elapsedTime / fillTime);
            color.a = amountValue;
            visitImage.color = color;
            elapsedTime += Time.deltaTime;
        }
        else
        {
            AS.Play();
            entryStart();
        }
    }

    /*------------------------------------------------------------
    Fill reset
    ------------------------------------------------------------*/
    void fillReset()
    {
        Color color = visitImage.color;
        color.a = 0.0f;
        visitImage.color = color;
        elapsedTime = 0f;
    }

    /*------------------------------------------------------------
    Branch events based on node type
    ------------------------------------------------------------*/
    void entryStart()
    {
        visited = true;

        if (nodeType != NodeType.Start) _mapGenerator.paintPath(this);

        if (nodeType != NodeType.Start && nodeType != NodeType.Final)
        {
            _mapGenerator.passedSameFloor(this);
        }
        else
        {
            passedNode();
        }

        _mapGenerator.nowNode = this;
        _mapGenerator.nowNodeIdx = idx;

        if (_mapGenerator.skipNodeProcessing)
        {
            _mapGenerator.toNextNode();
        }
        else
        {
            switch (nodeType)
            {
                case NodeType.Start:
                    handleStart();
                    break;
                case NodeType.Camp:
                    handleCamp();
                    break;
                case NodeType.Shop:
                    handleShop();
                    break;
                case NodeType.Event:
                    handleEvent();
                    break;
                case NodeType.Treasure:
                    handleTreasure();
                    break;
                case NodeType.Trap:
                    handleTrap();
                    break;
                case NodeType.Enemy:
                    handleEnemy();
                    break;
                case NodeType.Middle:
                    handleMiddle();
                    break;
                case NodeType.Final:
                    handleFinal();
                    break;
                case NodeType.Random:
                    handleRandom();
                    break;
                default:
                    Debug.Log($"Nothing is set");
                    break;
            }
        }

        // 노드 클릭 시, 맵 데이터 저장
        SaveLoadManager.SaveMap(_mapGenerator);

    }

    /*------------------------------------------------------------
    Processing each node
    ------------------------------------------------------------*/
    void handleStart()
    {
        Debug.Log("게임 시작");
        _mapGenerator.toNextNode();
    }

    void handleCamp()
    {
        Debug.Log("Open a camp");
        _mapGenerator.toNextNode();
    }

    void handleShop()
    {
        Debug.Log("Open a shop");
        _mapGenerator.toNextNode();
    }

    void handleEvent()
    {
        Debug.Log("Execute event");
        _mapGenerator.toNextNode();
    }

    void handleTreasure()
    {
        Debug.Log("Obtain treasure");
        _mapGenerator.toNextNode();
    }

    void handleTrap()
    {
        Debug.Log("Execute trap");
        _mapGenerator.toNextNode();
    }

    async void handleEnemy()
    {
        Debug.Log("적 스테이지로 진입");
        await _scene.SceneHandler.FadeScreen();
        _scene.SceneHandler.LoadingScreen(nextScene);
    }

    void handleMiddle()
    {
        Debug.Log("Midpoint");
        _mapGenerator.toNextNode();
    }

    void handleFinal()
    {
        switch (nodeText.text)
        {
            case "Boss01":
                Debug.Log("Fight the Boss 01");
                break;
            case "Boss02":
                Debug.Log("Fight the Boss 02");
                break;
            default:
                Debug.Log("Fight the Boss 03");
                break;
        }
    }

    void handleRandom()
    {
        Debug.Log("Execute random process");
        _mapGenerator.toNextNode();
    }

    private void OnDestroy()
    {
        UpdatePublisher.DiscribeObserver(this);
    }
}
