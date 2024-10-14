using System.Collections.Generic;
using UnityEngine;

public class FreeCameraNav : MonoBehaviour
{
    public Camera MainCamera;
    public Camera TargetCamera;
    public float movementSpeed = 10f;
    public float fastMovementSpeed = 100f;
    public float freeLookSensitivity = 3f;
    public float zoomSensitivity = 10f;
    public float fastZoomSensitivity = 50f;
    private bool mouse_initialed = false;
    private bool looking = false;
    private bool locking = false;

    public bool isMouseInViewPort;
    public bool isOnRotating;
    public bool isOnPenning;
    public float panSensitivity = 0.01f;
    private Vector3 lastPosition;

    private float _rotationY;
    private float _rotationX;

    private Vector3 LastRightClick;

    [SerializeField]
    private Transform _target;
    private float _distanceFromTarget = 3.0f;
    private Vector3 _currentRotation;
    private Vector3 _smoothVelocity = Vector3.zero;

    [SerializeField]
    private float _smoothTime = 0.2f;
    private Vector2 _rotationXMinMax = new Vector2(-40, 40);
    public Vector3 _ceneterScreen;

    [Header("Spots")]
    public List<Transform> CameraSpots;

    private void Start()
    {
        if (MainCamera == null)
        {
            MainCamera = Camera.main;
        }

        _target.gameObject.SetActive(false);
        _ceneterScreen = new Vector3(Screen.width / 2, Screen.height / 2, 0);
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.V))
        {
            ToggleCameraMode();
        }

        if (locking)
        {
            return;
        }

        if (_target != null)
        {
            _distanceFromTarget = Vector3.Distance(MainCamera.transform.position, _target.position);
            _target.rotation = transform.rotation;
        }

        if (TargetCamera != null)
        {
            isMouseInViewPort = mouseOverViewport(MainCamera, TargetCamera);

            if (!isMouseInViewPort)
            {
                return;
            }
        }

        if (MouseInputUIBlocker.BlockedByUI)
        {
            return;
        }

        var fastMode = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        var movementSpeed = fastMode ? this.fastMovementSpeed : this.movementSpeed;

        HandleMovement(movementSpeed);
        HandleRotation();
        HandleZoom(fastMode);

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            selectObjectfromRaycast();
        }

        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            selectObjectfromRaycast();
            _target.gameObject.SetActive(false);
        }

        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            mouse_initialed = true;
        }

        if (Input.GetKeyUp(KeyCode.Mouse1))
        {
            mouse_initialed = false;
        }

        if (Input.GetKey(KeyCode.Mouse1))
        {
            if (mouse_initialed)
            {
                isOnRotating = true;
                RotateAroundObject();
            }
            else
            {
                isOnRotating = false;
            }
        }
        else
        {
            isOnRotating = false;
        }

        HandlePanning();

        if (Input.GetKeyUp(KeyCode.Mouse1))
        {
            StopLooking();
        }
    }

    void HandleMovement(float movementSpeed)
    {
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            transform.position += -transform.right * movementSpeed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            transform.position += transform.right * movementSpeed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            transform.position += transform.forward * movementSpeed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            transform.position += -transform.forward * movementSpeed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.Q))
        {
            transform.position += transform.up * movementSpeed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.E))
        {
            transform.position += -transform.up * movementSpeed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.R) || Input.GetKey(KeyCode.PageUp))
        {
            transform.position += Vector3.up * movementSpeed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.F) || Input.GetKey(KeyCode.PageDown))
        {
            transform.position += -Vector3.up * movementSpeed * Time.deltaTime;
        }
    }

    void HandleRotation()
    {
        if (looking)
        {
            float newRotationX = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * freeLookSensitivity;
            float newRotationY = transform.localEulerAngles.x - Input.GetAxis("Mouse Y") * freeLookSensitivity;
            transform.localEulerAngles = new Vector3(newRotationY, newRotationX, 0f);
        }
    }

    void HandleZoom(bool fastMode)
    {
        float axis = Input.GetAxis("Mouse ScrollWheel");

        if (axis != 0)
        {
            if (MainCamera.orthographic)
            {
                MainCamera.orthographicSize -= axis * (fastMode ? fastZoomSensitivity : zoomSensitivity);
                MainCamera.orthographicSize = Mathf.Clamp(MainCamera.orthographicSize, 1f, 100f);
            }
            else
            {
                var zoomSensitivity = fastMode ? this.fastZoomSensitivity : this.zoomSensitivity;
                transform.position += transform.forward * axis * zoomSensitivity;
            }
        }
    }

    void HandlePanning()
    {
        if (Input.GetMouseButtonDown(2))
        {
            lastPosition = Input.mousePosition;
        }

        if (Input.GetMouseButton(2))
        {
            isOnPenning = true;
            Vector3 delta = Input.mousePosition - lastPosition;
            transform.Translate(-delta.x * panSensitivity, -delta.y * panSensitivity, 0);
            _target.Translate(-delta.x * panSensitivity, -delta.y * panSensitivity, 0);
            lastPosition = Input.mousePosition;
        }
        else
        {
            isOnPenning = false;
        }
    }

    bool mouseOverViewport(Camera main_cam, Camera local_cam)
    {
        Vector3 main_mou = main_cam.ScreenToViewportPoint(Input.mousePosition);
        return local_cam.rect.Contains(main_mou);
    }

    void OnDisable()
    {
        StopLooking();
    }

    public void StartLooking()
    {
        looking = true;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void StopLooking()
    {
        looking = false;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void selectObjectfromRaycast()
    {
        RaycastHit hit;
        Ray ray = MainCamera.ScreenPointToRay(_ceneterScreen);

        if (Physics.Raycast(ray, out hit))
        {
            _target.position = hit.point;
        }
        else
        {
            _distanceFromTarget = Vector3.Distance(MainCamera.transform.position, _target.position);
        }
    }

    public void RotateAroundObject()
    {
        float mouseX = Input.GetAxis("Mouse X") * freeLookSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * freeLookSensitivity;

        if (mouseX != 0 || mouseY != 0)
        {
            _rotationY += mouseX;
            _rotationX -= mouseY;

            _target.gameObject.SetActive(true);

            _rotationX = Mathf.Clamp(_rotationX, -90f, 90f);

            Vector3 nextRotation = new Vector3(_rotationX, _rotationY);

            _currentRotation = Vector3.MoveTowards(_currentRotation, nextRotation, _smoothTime);
            transform.localEulerAngles = _currentRotation;

            transform.position = _target.position - transform.forward * _distanceFromTarget;
        }
    }

    public void OnClick_ChangeCameraSpots(int _index)
    {
        LeanTween.move(gameObject, CameraSpots[_index], 0.5f);
        LeanTween.rotate(gameObject, CameraSpots[_index].rotation.eulerAngles, 0.5f);

        _target.position = CameraSpots[CameraSpots.Count - 1].position;
        _target.rotation = CameraSpots[CameraSpots.Count - 1].rotation;
    }

    public void ToggleCameraMode()
    {
        if (MainCamera.orthographic)
        {
            MainCamera.orthographic = false;
        }
        else
        {
            MainCamera.orthographic = true;
        }
    }

    public void SetTargetPoint(Vector3 _position)
    {
        _target.position = _position;
    }

    public void CenterScreenRayCast()
    {
        RaycastHit hit;
        Ray ray = MainCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        if (Physics.Raycast(ray, out hit))
        {
            Transform objectHit = hit.transform;
            _distanceFromTarget = Vector3.Distance(MainCamera.transform.position, objectHit.position);
            _target.position = hit.point;
        }
        else
        {
            _distanceFromTarget = Vector3.Distance(MainCamera.transform.position, _target.position);
        }
    }

    public void SetRotationTarget(Vector3 _pos)
    {
        _target.position = _pos;
        _currentRotation = MainCamera.transform.rotation.eulerAngles;
    }

    public void SetCameraLookAtExterial(Vector3 _newPos, Vector3 _boundCenter)
    {
    }
}
