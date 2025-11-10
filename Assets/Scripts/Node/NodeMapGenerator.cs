using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
using DG.Tweening;
using S3MG;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "NodeMapGenerator", menuName = "NodeTree/NodeMapGenerator", order = 1)]
public class NodeMapGenerator : ScriptableObject, IUpdateObserver
{
    [SerializeField] public bool noMapOperation { get; set; } = false;

    [Header("▼Skip Node Processing : If true, node processing is skipped, and only the stage advances")]
    [SerializeField] public bool skipNodeProcessing = false;

    [Header("▼Generate on Execution")]
    [SerializeField] public bool playOnAwake = true;

    [Header("▼Make Start Node")]
    [SerializeField] public bool makeStart = true;

    [Header("▼Offset Nodes in Placement")]
    [SerializeField] public bool isOffset = true;

    [Header("▼Show Text on Nodes")]
    [SerializeField] public bool showText = true;

    [Header("▼Allow Path Intersection")]
    [SerializeField] public bool crossable = false;

    [Header("▼Completely Random Placement : Except for fixed nodes, do not apply the default map placement rules")]
    [SerializeField] public bool allRandom = false;

    [Header("▼Whether to randomize the seed : If true, the value of the variable seed is invalid")]
    [SerializeField] public bool randomizeSeed = true;
    [SerializeField] public int seed = 0;

    [Header("▼Map Basic Settings")]
    [SerializeField, Range(1, 128)] public int floorNum = 15;
    [SerializeField, Range(1, 32)] public int routeNum = 7;
    [SerializeField, Range(1, 256)] public float normalNodeSize = 80;
    [SerializeField, Range(1, 256)] public float startNodeSize = 160;
    [SerializeField, Range(1, 256)] public float finalNodeSize = 160;
    [SerializeField] public float floorDistance = 80;
    [SerializeField] public float routeDistance = 80;

    [Header("▼Active Route Count : Limited to RouteNum if exceeded")]
    [SerializeField, Range(1, 20)] public int activeRouteNum = 4;
    Node[] activeRouteNodeArray;

    private GameObject mapField;
    private GameObject mapParent;

    [Header("▼Node Prefab: ButtonPrefab with Node class")]
    [SerializeField] public Node nodePref;

    [Header("▼Path Settings : Prefab is not required, Custom Prefab with settings like shaders is also acceptable")]
    [SerializeField] public SpriteRenderer pathImagePref;
    [SerializeField] public Color pathColor = Color.gray;
    [SerializeField] public Color passedPathColor = Color.white;
    [SerializeField, Range(0, 16)] public float pathWidth = 4f;
    [SerializeField, Range(0, 256)] public float paddingBetweenNodes = 0;

    [Header("▼Background Prefab : Prefab is not required, Image Prefab with 9-slice scaling")]
    [SerializeField] public GameObject backgroundPref;

    [Header("▼Padding Between Background and Map")]
    [SerializeField, Range(0, 160)] public float backgroundPadding = 80;

    [Header("▼Mouse Drag Sensitivity and Gamepad Movement Sensitivity")]
    [SerializeField, Range(1, 3)] public float mouseSensitive = 1.0f;
    [SerializeField, Range(1, 6)] public float gamepadSensitive = 2.0f;

    [Header("▼Camera Control")]
    [SerializeField] private GameObject cameraPref;
    [SerializeField] private GameObject planetSelectorPref;
    [SerializeField] private float cameraPositionYOffset;
    [SerializeField] private float cameraPositionZOffset;
    [SerializeField] private float camSpeed;
    [SerializeField] private float smoothTime;
    List<Node> showNodes;
    private int showNodesIndex = 0;
    Camera cam;
    GameObject planetSelector;
    InputHandler inputHandler;
    Vector3 direction;
    Vector3 targetPosition;
    Vector3 velocity = Vector3.zero;
    private InputDisposer inputDisposer;

    [Header("▼Data for Each Node : Set ScriptableObject Data")]
    [SerializeField] public NodeData startNodeData;
    [SerializeField] public NodeData[] finalNodeData;
    [SerializeField] public FixedNodeData[] fixedNodeData;
    [SerializeField] public MapNodeData[] mapNodeData;

    [SerializeField] public Node nowNode { get; set; }

    Node startNode;
    Node finalNode;
    Node[,] map;
    bool isCompleted = false;
    List<Transform> pathsTransform;

    /*------------------------------------------------------------
    Executed when the value in the inspector is changed:Implemented to initialize when added because serializable classes like FixedNodeData and MapNodeData cannot use constructors
    ------------------------------------------------------------*/
    void OnValidate()
    {
        if (!Application.isPlaying && fixedNodeData != null)
        {
            foreach (FixedNodeData node in fixedNodeData)
            {
                if (node.nodeData == null) node.init();
            }
        }
        if (!Application.isPlaying && mapNodeData != null)
        {
            foreach (MapNodeData node in mapNodeData)
            {
                if (node.nodeData == null) node.init();
            }
        }
    }

    /*------------------------------------------------------------
    Initialization
    ------------------------------------------------------------*/
    public void init()
    {
        if (dataCheckBeforeGeneration()) return;
        UpdatePublisher.SubscribeObserver(this);
        cam = Instantiate(cameraPref).GetComponent<Camera>();
        planetSelector = Instantiate(planetSelectorPref);
        pathsTransform = new List<Transform>();
        showNodes = new List<Node>();
        ServiceLocator.ForSceneOf(this).Get(out inputHandler);
        ServiceLocator.ForSceneOf(this).Register(cam);
        inputDisposer = new InputDisposer(inputHandler, InputHandler.InputState.SelectPlanet);
        inputHandler.OnMoveToPlanetAction += ShowActiveNode;
        inputHandler.OnControlSpaceshipAction += (_direction) => direction = _direction;

        if (randomizeSeed) seed = Mathf.FloorToInt(Random.value * int.MaxValue);
        Random.InitState(seed);

        preGenerated();
        createMap();
        selectFirstNode();
        for (int i = 0; i < activeRouteNodeArray.Length; i++)
        {
            connectingNodes(activeRouteNodeArray[i]);
        }

        hideEmpty();
        setNode();
        activeNodeSelect();
        isCompleted = true;
    }

    public void ObserverUpdate(float dt)
    {
        if (!isCompleted) return;
        if (noMapOperation) return;

        if(cam != null)
        {
            targetPosition += direction * camSpeed * Time.deltaTime;
            cam.transform.position = Vector3.SmoothDamp(cam.transform.position, targetPosition, ref velocity,smoothTime);
        }
    }

    /*------------------------------------------------------------
    Pre-generation data check
    ------------------------------------------------------------*/
    bool dataCheckBeforeGeneration()
    {
        bool check = false;

        if (nodePref == null)
        {
            Debug.Log($"Please register the Node Prefab.");
            check = true;
        }
        if (makeStart && startNodeData == null)
        {
            Debug.Log($"Please register the Start Node data.");
            check = true;
        }
        if (finalNodeData.Length == 0 || mapNodeData.Length == 0)
        {
            Debug.Log($"Please register all map node data except for fixedNodeData.");
            check = true;
        }

        for (int i = 0; i < finalNodeData.Length; i++)
        {
            if (finalNodeData[i] == null)
            {
                Debug.Log($"One or more finalNodeData is null.");
                check = true;
                break;
            }
        }

        for (int i = 0; i < mapNodeData.Length; i++)
        {
            if (mapNodeData[i].nodeData == null)
            {
                Debug.Log($"One or more mapNodeData is null.");
                check = true;
                break;
            }
        }

        for (int i = 0; i < fixedNodeData.Length; i++)
        {
            if (fixedNodeData[i].nodeData == null)
            {
                Debug.Log($"One or more fixedNodeData is null.");
                check = true;
                break;
            }
        }

        bool anySerializable = false;
        for (int i = 0; i < mapNodeData.Length; i++)
        {
            if (mapNodeData[i].serializable)
            {
                anySerializable = true;
                break;
            }
        }
        if (!anySerializable)
        {
            Debug.Log($"Please register at least one continuous node in mapNodeData.");
            check = true;
        }

        bool appearedAfter1 = false;
        for (int i = 0; i < mapNodeData.Length; i++)
        {
            if (!mapNodeData[i].reverseAppearedAfter && mapNodeData[i].appearedAfter == 1)
            {
                appearedAfter1 = true;
                break;
            }
        }
        if (!appearedAfter1)
        {
            Debug.Log($"Please register at least one node that can start from the first floor in mapNodeData.");
            check = true;
        }

        return check;
    }

    /*------------------------------------------------------------
    Pre-generation
    ------------------------------------------------------------*/
    void preGenerated()
    {
        mapField = new GameObject("Map Field");
        mapField.transform.position = new Vector3 (0, 0, 0);

        mapParent = new GameObject("Parent");
        mapParent.layer = LayerMask.NameToLayer("UI");
        mapParent.transform.SetParent(mapField.transform);
        mapParent.transform.SetAsFirstSibling();

        if (pathImagePref == null)
        {
            GameObject pathObject = new GameObject("Path");
            Image image = pathObject.AddComponent<Image>();
            image.raycastTarget = false;
            pathObject.layer = LayerMask.NameToLayer("UI");
            pathImagePref = pathObject.GetComponent<SpriteRenderer>();
        }
    }

    /*------------------------------------------------------------
    Create map
    ------------------------------------------------------------*/
    void createMap()
    {
        map = new Node[floorNum, routeNum];

        if (makeStart)
        {
            startNode = Instantiate(nodePref, mapParent.transform);
            startNode.Init(this);

            setNodeSize(startNode, startNodeSize);
            startNode.transform.position = new Vector3(0, 0,0);
            startNode.gameObject.name = $"Start Node";
            startNode.connected = true;
        }

        for (int i = 0; i < floorNum; i++)
        {
            for (int j = 0; j < routeNum; j++)
            {
                Node node = Instantiate(nodePref, mapParent.transform);
                node.Init(this);

                setNodeSize(node, normalNodeSize);
                node.floor = i;
                node.route = j;
                node.xPos = (routeDistance * j) + (normalNodeSize * j) - (routeDistance * (routeNum - 1) / 2 + (normalNodeSize * (routeNum - 1) / 2));
                //node.yPos = (floorDistance * i) + (normalNodeSize * i) + ((makeStart) ? floorDistance + (startNodeSize / 2) + (normalNodeSize / 2) : 0);
                node.zPos = (floorDistance * i) + (normalNodeSize * i) + ((makeStart) ? floorDistance + (startNodeSize / 2) + (normalNodeSize / 2) : 0);
                if (isOffset)
                {
                    node.xPos += Random.Range(-routeDistance * 0.9f / 2, routeDistance * 0.9f / 2 + 1);
                    //node.yPos += Random.Range(-floorDistance * 0.9f / 2, floorDistance * 0.9f / 2 + 1);
                    node.zPos += Random.Range(-floorDistance * 0.9f / 2, floorDistance * 0.9f / 2 + 1);
                }
                //node.transform.position = new Vector3(node.xPos, node.yPos,0);
                node.transform.position = new Vector3(node.xPos, node.yPos,node.zPos);
                map[i, j] = node;
                map[i, j].gameObject.name = $"{i},{j}";
            }
        }

        finalNode = Instantiate(nodePref, mapParent.transform);
        finalNode.Init(this);

        setNodeSize(finalNode, finalNodeSize);
        //finalNode.transform.position = new Vector3(0, (floorDistance * floorNum) + (normalNodeSize * floorNum) + (finalNodeSize / 2) + (normalNodeSize / 2) + ((makeStart) ? 10 : 0),0);
        finalNode.transform.position = new Vector3(0, 0, (floorDistance * floorNum) + (normalNodeSize * floorNum) + (finalNodeSize / 2) + (normalNodeSize / 2) + ((makeStart) ? 10 : 0));
        finalNode.gameObject.name = $"Final Node";
        finalNode.connected = true;

        //mapWidth = (routeNum * normalNodeSize) + (routeNum * routeDistance);
        //mapHeight = finalNode.transform.position.y + ((makeStart) ? startNodeSize / 2 : normalNodeSize / 2) + finalNodeSize / 2;
    }

    /*------------------------------------------------------------
    Set node size
    ------------------------------------------------------------*/
    void setNodeSize(Node target, float size)
    {
        target.transform.localScale = new Vector2 (size, size);
    }

    /*------------------------------------------------------------
    Randomly select active route nodes and store them in an array : To prevent random selection from messing up the order, ensure the randomly selected active route nodes are stored in ascending order
    ------------------------------------------------------------*/
    void selectFirstNode()
    {
        List<Node> tmp = new List<Node>();
        for (int i = 0; i < routeNum; i++)
        {
            tmp.Add(map[0, i]);
        }

        if (activeRouteNum < 1) activeRouteNum = 1;
        if (activeRouteNum > routeNum) activeRouteNum = routeNum;

        for (int i = 0; i < routeNum - activeRouteNum; i++)
        {
            tmp.RemoveAt(Random.Range(0, tmp.Count));
        }

        activeRouteNodeArray = tmp.ToArray();
    }

    /*------------------------------------------------------------
    Connect nodes
    ------------------------------------------------------------*/
    void connectingNodes(Node node)
    {
        if (!node.connected) node.connected = true;
        if (node.floor < floorNum - 1)
        {
            int connectDirection = Random.Range(-1, 2);
            if (node.route == 0 && connectDirection == -1) connectDirection++;
            if (node.route == routeNum - 1 && connectDirection == 1) connectDirection--;

            if (!crossable)
            {
                if (node.route != 0 && connectDirection == -1 && map[node.floor, node.route - 1].nextNodes != null)
                {
                    for (int i = 0; i < map[node.floor, node.route - 1].nextNodes.Count; i++)
                    {
                        if (map[node.floor, node.route - 1].nextNodes[i] == map[node.floor + 1, node.route])
                        {
                            connectDirection++;
                            break;
                        }
                    }
                }
                if (node.route != routeNum - 1 && connectDirection == 1 && map[node.floor, node.route + 1].nextNodes != null)
                {
                    for (int i = 0; i < map[node.floor, node.route + 1].nextNodes.Count; i++)
                    {
                        if (map[node.floor, node.route + 1].nextNodes[i] == map[node.floor + 1, node.route])
                        {
                            connectDirection--;
                            break;
                        }
                    }
                }
            }

            if (makeStart)
            {
                if (node.floor == 0)
                {
                    startNode.nextNodes.Add(node);
                    node.prevNodes.Add(startNode);
                    drawPath(startNode.transform, node.transform);
                }
            }

            Node nextNode = map[node.floor + 1, node.route + connectDirection];

            bool haveNode = false;
            for (int i = 0; i < node.nextNodes.Count; i++)
            {
                if (node.nextNodes[i] == nextNode) haveNode = true;
            }
            if (!haveNode) drawPath(node.transform, nextNode.transform);

            node.nextNodes.Add(nextNode);
            nextNode.prevNodes.Add(node);

            connectingNodes(nextNode);
        }
        else
        {
            bool haveFinalNode = false;
            for (int i = 0; i < node.nextNodes.Count; i++)
            {
                if (node.nextNodes[i] == finalNode) haveFinalNode = true;
            }
            if (!haveFinalNode) drawPath(node.transform, finalNode.transform);

            node.nextNodes.Add(finalNode);
            finalNode.prevNodes.Add(node);

            if (floorNum == 1 && makeStart)
            {
                startNode.nextNodes.Add(node);
                node.prevNodes.Add(startNode);
                drawPath(startNode.transform, node.transform);
            }
        }
    }

    /*------------------------------------------------------------
    Draw paths
    ------------------------------------------------------------*/
    void drawPath(Transform start, Transform end)
    {
        SpriteRenderer path = Instantiate(pathImagePref, mapParent.transform);
        path.color = pathColor;

        float distance = Vector3.Distance(start.transform.position, end.transform.position);
        //distance -= (distance > paddingBetweenNodes * 2) ? paddingBetweenNodes * 2 : distance;
        path.size = new Vector2(path.size.x, distance*3);
        //path.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, distance);

        path.transform.position = (start.position + end.position) / 2;

        float angle = Mathf.Atan2(end.transform.position.z - start.transform.position.z, end.transform.position.x - start.transform.position.x) * Mathf.Rad2Deg - 90;
        path.transform.rotation = Quaternion.Euler(90, 0, angle);

        path.transform.SetAsFirstSibling();

        pathsTransform.Add(path.transform);
    }

    /*------------------------------------------------------------
	Hide unvisited nodes
	------------------------------------------------------------*/
    void hideEmpty()
    {
        for (int i = 0; i < floorNum; i++)
        {
            for (int j = 0; j < routeNum; j++)
            {
                if (!map[i, j].connected) map[i, j].gameObject.SetActive(false);
            }
        }
    }

    /*------------------------------------------------------------
    Set each node
    ------------------------------------------------------------*/
    void setNode()
    {
        if (makeStart) startNode.setNodeData(startNodeData.sprite, startNodeData.type, (showText) ? startNodeData.nodeName : "");

        int finalNodeNum = Random.Range(0, finalNodeData.Length);
        finalNode.setNodeData(finalNodeData[finalNodeNum].sprite, finalNodeData[finalNodeNum].type, (showText) ? finalNodeData[finalNodeNum].nodeName : "");

        List<Node> tmp = new List<Node>();
        tmp = getUnassignedConnectedNodes(tmp);
        float totalChance = 0;
        for (int i = 0; i < mapNodeData.Length; i++)
        {
            totalChance += mapNodeData[i].chance;
        }

        if (allRandom)
        {
            randomlyAssignNode(tmp, totalChance);
        }
        else
        {
            int loopCount = 0;
            while (true)
            {
                loopCount++;
                applyRulesAndAssignNode(tmp, totalChance);
                duplicateBranchBackToNull();
                tmp.Clear();
                tmp = getUnassignedConnectedNodes(tmp);
                if (tmp.Count > 0 && loopCount > 100)
                {
                    randomlyAssignNode(tmp, totalChance);
                    Debug.Log($"The outer loop exceeded 100 iterations, so assigning a random node.");
                }
                if (tmp.Count == 0 || loopCount > 100) break;
            }
            // Debug.Log($"Outer loop：{loopCount}");
        }

        for (int i = 0; i < fixedNodeData.Length; i++)
        {
            if (fixedNodeData[i].appearedOn >= floorNum + 1) continue;
            for (int j = 0; j < routeNum; j++)
            {
                if (map[fixedNodeData[i].appearedOn - 1, j].connected) map[fixedNodeData[i].appearedOn - 1, j].setNodeData(fixedNodeData[i].nodeData.sprite, fixedNodeData[i].nodeData.type, (showText) ? fixedNodeData[i].nodeData.nodeName : "");
            }
        }
    }

    /*------------------------------------------------------------
    Place nodes applying rules
    ------------------------------------------------------------*/
    void applyRulesAndAssignNode(List<Node> tmp, float totalChance)
    {
        int loopCount = 0;
        while (true)
        {
            loopCount++;
            for (int i = 0; i < tmp.Count; i++)
            {
                MapNodeData selectedNodeData = selectRandomNodeData(mapNodeData, totalChance);
                if (selectedNodeData != null)
                {
                    if (meetsCondition(tmp[i], selectedNodeData))
                    {
                        tmp[i].setNodeData(selectedNodeData.nodeData.sprite, selectedNodeData.nodeData.type, (showText) ? selectedNodeData.nodeData.nodeName : "");
                    }
                }
            }
            tmp.Clear();
            tmp = getUnassignedConnectedNodes(tmp);
            if (tmp.Count > 0 && loopCount > 100)
            {
                randomlyAssignNode(tmp, totalChance);
                Debug.Log($"The inner loop exceeded 100 iterations, so assigning a random node.");
            }
            if (tmp.Count == 0 || loopCount > 100) break;
        }
        // Debug.Log($"Inner Loop：{loopCount}");
    }

    /*------------------------------------------------------------
    Return a list of unset nodes
    ------------------------------------------------------------*/
    List<Node> getUnassignedConnectedNodes(List<Node> tmp)
    {
        for (int i = 0; i < floorNum; i++)
        {
            for (int j = 0; j < routeNum; j++)
            {
                if (map[i, j].connected && map[i, j].nodeType == null) tmp.Add(map[i, j]);
            }
        }
        return tmp;
    }

    /*------------------------------------------------------------
    Return nodes based on spawn probability
    ------------------------------------------------------------*/
    MapNodeData selectRandomNodeData(MapNodeData[] mapNodeData, float totalChance)
    {
        float randNum = Random.Range(0f, totalChance);
        float cumulativeChance = 0f;

        for (int j = 0; j < mapNodeData.Length; j++)
        {
            cumulativeChance += mapNodeData[j].chance;
            if (randNum < cumulativeChance)
            {
                return mapNodeData[j];
            }
        }
        return null;
    }

    /*------------------------------------------------------------
    Check map placement rules and return true if rules are met
    ------------------------------------------------------------*/
    bool meetsCondition(Node node, MapNodeData selectedNodeData)
    {
        bool flag = false;

        if (!selectedNodeData.reverseAppearedAfter)
        {
            if (node.floor >= selectedNodeData.appearedAfter - 1) flag = true;
        }

        if (selectedNodeData.reverseAppearedAfter)
        {
            if (node.floor < selectedNodeData.appearedAfter - 1) flag = true;
        }

        if (!selectedNodeData.serializable)
        {
            for (int i = 0; i < node.nextNodes.Count; i++)
            {
                if (node.nextNodes[i].nodeType == selectedNodeData.nodeData.type)
                {
                    flag = false;
                    break;
                }
            }
            for (int i = 0; i < node.prevNodes.Count; i++)
            {
                if (node.prevNodes[i].nodeType == selectedNodeData.nodeData.type)
                {
                    flag = false;
                    break;
                }
            }
        }

        return flag;
    }

    /*------------------------------------------------------------
    Set duplicate nodes of overlapping branches to null
    ------------------------------------------------------------*/
    void duplicateBranchBackToNull()
    {
        List<Node> tmp = new List<Node>();
        for (int i = 0; i < floorNum; i++)
        {
            for (int j = 0; j < routeNum; j++)
            {
                if (map[i, j].nodeType != null) tmp.Add(map[i, j]);
            }
        }
        for (int i = 0; i < tmp.Count; i++)
        {
            if (tmp[i].nextNodes.Count == 2 && tmp[i].nextNodes[0].nodeType == tmp[i].nextNodes[1].nodeType && tmp[i].nextNodes[0].xPos != tmp[i].nextNodes[1].xPos)
            {
                tmp[i].nextNodes[0].setNodeData(null, null);
            }
            if (tmp[i].nextNodes.Count == 3)
            {
                Node first = tmp[i].nextNodes[0];
                Node second = tmp[i].nextNodes[1];
                Node third = tmp[i].nextNodes[2];
                if (first.nodeType == second.nodeType && second.nodeType == third.nodeType && first.xPos != second.xPos && second.xPos != third.xPos)
                {
                    first.setNodeData(null, null);
                    second.setNodeData(null, null);
                }
                if (first.nodeType == second.nodeType && first.xPos != second.xPos)
                {
                    first.setNodeData(null, null);
                }
                if (first.nodeType == third.nodeType && first.xPos != third.xPos)
                {
                    first.setNodeData(null, null);
                }
                if (second.nodeType == third.nodeType && second.xPos != third.xPos)
                {
                    second.setNodeData(null, null);
                }
            }
        }
    }

    /*------------------------------------------------------------
    Freely place all nodes according to spawn probability except fixed nodes
    ------------------------------------------------------------*/
    void randomlyAssignNode(List<Node> tmp, float totalChance)
    {
        for (int i = 0; i < tmp.Count; i++)
        {
            MapNodeData selectedNodeData = selectRandomNodeData(mapNodeData, totalChance);
            if (selectedNodeData != null)
            {
                tmp[i].setNodeData(selectedNodeData.nodeData.sprite, selectedNodeData.nodeData.type, (showText) ? selectedNodeData.nodeData.nodeName : "");
            }
        }
        Debug.Log($"Placing nodes randomly.");
    }

    /*------------------------------------------------------------
    Select the node to start
    ------------------------------------------------------------*/
    void activeNodeSelect()
    {
        finalNode.disableButton();
        for (int i = 0; i < floorNum; i++)
        {
            for (int j = 0; j < routeNum; j++)
            {
                if (map[i, j].connected && map[i, j].nodeType != null) map[i, j].disableButton();
            }
        }

        if (makeStart)
        {
            startNode.enableButton();
            showNodes.Add(startNode);
            if (Gamepad.current != null) EventSystem.current.SetSelectedGameObject(startNode.gameObject);
        }
        else
        {
            for (int i = 0; i < activeRouteNodeArray.Length; i++)
            {
                activeRouteNodeArray[i].enableButton();
                showNodes.Add(activeRouteNodeArray[i]);
            }
            if (Gamepad.current != null) EventSystem.current.SetSelectedGameObject(activeRouteNodeArray[0].gameObject);
        }

        ShowActiveNode(0);
    }

    /*------------------------------------------------------------
    Paint the paths that have been passed
    ------------------------------------------------------------*/
    public void paintPath(Node node)
    {
        if (!makeStart && node.floor == 0) return;

        foreach (Transform path in pathsTransform)
        {
            float distance = Vector2.Distance(path.position, (nowNode.transform.position + node.transform.position) / 2);
            if (Mathf.Approximately(distance, 0) || distance < 0.01f)
            {
                path.GetComponent<SpriteRenderer>().color = passedPathColor;
                break;
            }
        }
    }

    /*------------------------------------------------------------
    Set nodes on the same floor as the argument to passed state, making them unclickable
    ------------------------------------------------------------*/
    public void passedSameFloor(Node node)
    {
        for (int i = 0; i < floorNum; i++)
        {
            for (int j = 0; j < routeNum; j++)
            {
                if (map[i, j].floor == node.floor)
                {
                    map[i, j].passedNode();
                }
            }
        }
    }

    /*------------------------------------------------------------
    Move to the next node: for operations from external classes
    ------------------------------------------------------------*/
    public void toNextNode()
    {
        if (nowNode == finalNode) return;
        showNodes.Clear();
        for (int i = 0; i < nowNode.nextNodes.Count; i++)
        {
            showNodes.Add(nowNode.nextNodes[i]);
        }
        ShowActiveNode(0);
        if (Gamepad.current != null) EventSystem.current.SetSelectedGameObject(nowNode.nextNodes[0].gameObject);
    }

    /*------------------------------------------------------------
    Activate map : for operations from external classes
    ------------------------------------------------------------*/
    public void activeMap()
    {
        noMapOperation = false;
        mapField.SetActive(true);
        UpdatePublisher.SubscribeObserver(this);
    }

    /*------------------------------------------------------------
    Deactivate map : for operations from external classes
    ------------------------------------------------------------*/
    public void inactiveMap()
    {
        noMapOperation = true;
        mapField.SetActive(false);
        UpdatePublisher.DiscribeObserver(this);
    }

    /*------------------------------------------------------------
    ReGenarate : for operations from external classes
    ------------------------------------------------------------*/
    public void reGenarate()
    {
        if (mapParent != null)
        {
            Destroy(mapParent.gameObject);
        }
        if (activeRouteNodeArray != null)
        {
            for (int i = 0; i < activeRouteNodeArray.Length; i++)
            {
                activeRouteNodeArray[i] = null;
            }
        }
        if (map != null)
        {
            for (int i = 0; i < map.GetLength(0); i++)
            {
                for (int j = 0; j < map.GetLength(1); j++)
                {
                    map[i, j] = null;
                }
            }
        }
        if (pathsTransform != null)
        {
            for (int i = 0; i < pathsTransform.Count; i++)
            {
                pathsTransform[i] = null;
            }
        }
        mapParent = null;
        activeRouteNodeArray = null;
        startNode = null;
        finalNode = null;
        map = null;
        pathsTransform = new List<Transform>();
        nowNode = null;
        isCompleted = false;
        noMapOperation = false;
        init();
    }

    /*--------------------------------
    Show Active Nodes
    ---------------------------------*/
    private void ShowActiveNode(float index)
    {
        foreach (var nodes in showNodes)
            nodes.disableButton();

        Node node;
        if (index < 0)
        {
            showNodesIndex = (showNodesIndex + (int)index + showNodes.Count) % showNodes.Count;
            node = showNodes[showNodesIndex];
        }
        else
        {
            showNodesIndex = (showNodesIndex + (int)index) % showNodes.Count;
            node = showNodes[showNodesIndex];
        }
        node.enableButton();
        Vector3 targetPos = new Vector3(node.transform.position.x, cameraPositionYOffset, node.transform.position.z - cameraPositionZOffset);
        cam.transform.DOMove(targetPos, 1.0f).SetEase(Ease.OutCirc);
        planetSelector.transform.position = new Vector3(node.transform.position.x, node.transform.position.y+3, node.transform.position.z);
        targetPosition = targetPos;
    }

    private void OnDestroy()
    {
        inputDisposer.Dispose();
        UpdatePublisher.DiscribeObserver(this);
    }
    // TODO : Map local save
}