using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GISCamera : MonoBehaviour
{
    [SerializeField]
    private float _mouseRotationSensitivity = 3.0f;

    [SerializeField]
    private float _rotationY;

    [SerializeField]
    private float _rotationX;

    [SerializeField]
    private Vector3 _currentRotation;

    [SerializeField]
    private float _distanceFromTarget = 3.0f;

    [SerializeField]
    private float _smoothTime = 0.2f;

    public bool isOnRotating;
    public Transform _target;

    private bool mouse_initialed = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        _distanceFromTarget = Vector3.Distance(transform.position, _target.position);
        _target.rotation = transform.rotation;

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
                RotateAround();
            }
            else
            {
                isOnRotating = false;
            }
        }
    }

    public void OnPositionInit()
    {
        _distanceFromTarget = Vector3.Distance(transform.position, _target.position);
        _currentRotation = transform.rotation.eulerAngles;
        _rotationY = 180;
        _rotationX = 90;
    }

    public void RotateAround()
    {
        float mouseX = Input.GetAxis("Mouse X") * _mouseRotationSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * _mouseRotationSensitivity;

        if (mouseX != 0 || mouseY != 0)
        {
            _rotationY += mouseX;
            _rotationX -= mouseY;

            _target.gameObject.SetActive(true);


            if (_rotationX > 90f)
            {
                _rotationX = 90f;
            }
            else
            if (_rotationX < -90f)
            {
                _rotationX = -90;
            }


            // Apply clamping for x rotation 
            //_rotationX = Mathf.Clamp(_rotationX, _rotationXMinMax.x, _rotationXMinMax.y);

            Vector3 nextRotation = new Vector3(_rotationX, _rotationY);

            // Apply damping between rotation changes
            _currentRotation = Vector3.MoveTowards(_currentRotation, nextRotation, _smoothTime);
            transform.localEulerAngles = _currentRotation;

            // Substract forward vector of the GameObject to point its forward vector to the target
            transform.position = _target.position - transform.forward * _distanceFromTarget;
        }
    }


}
