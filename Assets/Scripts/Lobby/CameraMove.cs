using UnityEngine;

public class CameraMove : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera cam;

    [Header("Pan")]
    [SerializeField] private bool windowDragFeel = true; // true = '창 끌기' 느낌
    [SerializeField] private float panSpeed = 1f;

    [Header("Zoom (Mouse Wheel)")]
    [SerializeField] private bool enableZoom = true;
    [SerializeField] private float zoomSpeed = 2.5f;      // 휠 감도 (2~5 사이 추천)
    [SerializeField] private float minOrthoSize = 5.0f;
    [SerializeField] private float maxOrthoSize = 30.0f;
    [SerializeField] private float zoomSmoothTime = 0.12f; // 0.08~0.18 추천


    [Header("Clamp (Optional)")]
    [SerializeField] private bool useClamp = false;
    [SerializeField] private Vector2 clampMin = new Vector2(-10f, -6f);
    [SerializeField] private Vector2 clampMax = new Vector2(10f, 6f);

    //대화 UI 팝업시 사용 못하게
    [SerializeField] private bool blockInput = false;
    public void SetBlockInput(bool value) => blockInput = value;

    private bool _dragging;
    private Vector3 _lastMousePos;

    private float _targetOrtho;
    private float _zoomVelocity;

    private void Reset()
    {
        cam = Camera.main;
    }

    private void Awake()
    {
        if (cam == null) cam = Camera.main;
        cam.orthographic = true;
        _targetOrtho = cam.orthographicSize;

    }

    private void Update()
    {
        if (blockInput) return;

        HandleZoom();
        HandlePan();

        // 줌/이동 후 카메라가 영역 밖으로 나가면 다시 제한
        if (useClamp) ApplyClamp();
    }

    private void HandlePan()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _dragging = true;
            _lastMousePos = Input.mousePosition;
        }

        if (_dragging && Input.GetMouseButton(0))
        {
            Vector3 mousePos = Input.mousePosition;
            Vector3 mouseDelta = mousePos - _lastMousePos;
            _lastMousePos = mousePos;

            // "픽셀 이동량 -> 월드 이동량" 변환 (Ortho 기준)
            float unitsPerPixelY = (cam.orthographicSize * 2f) / Screen.height;
            float unitsPerPixelX = unitsPerPixelY * cam.aspect;

            float sign = windowDragFeel ? +1f : -1f;

            Vector3 move = new Vector3(
                mouseDelta.x * unitsPerPixelX * sign,
                mouseDelta.y * unitsPerPixelY * sign,
                0f
            ) * panSpeed;

            transform.position += move;
        }

        if (Input.GetMouseButtonUp(0))
        {
            _dragging = false;
        }
    }

    private void HandleZoom()
    {
        if (!enableZoom) return;

        float scroll = Input.mouseScrollDelta.y;
        if (!Mathf.Approximately(scroll, 0f))
        {
            // 휠 위=줌인(사이즈 감소), 아래=줌아웃(사이즈 증가)
            _targetOrtho -= scroll * zoomSpeed;
            _targetOrtho = Mathf.Clamp(_targetOrtho, minOrthoSize, maxOrthoSize);
        }

        // 실제 카메라 값은 부드럽게 따라감
        cam.orthographicSize = Mathf.SmoothDamp(
            cam.orthographicSize,
            _targetOrtho,
            ref _zoomVelocity,
            zoomSmoothTime
        );
    }

    private void ApplyClamp()
    {
        float halfH = cam.orthographicSize;
        float halfW = halfH * cam.aspect;

        Vector3 pos = transform.position;

        float minX = clampMin.x + halfW;
        float maxX = clampMax.x - halfW;
        float minY = clampMin.y + halfH;
        float maxY = clampMax.y - halfH;

        if (minX > maxX) pos.x = (clampMin.x + clampMax.x) * 0.5f;
        else pos.x = Mathf.Clamp(pos.x, minX, maxX);

        if (minY > maxY) pos.y = (clampMin.y + clampMax.y) * 0.5f;
        else pos.y = Mathf.Clamp(pos.y, minY, maxY);

        transform.position = pos;
    }
}
