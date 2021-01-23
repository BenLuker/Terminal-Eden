using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopDownCameraController : MonoBehaviour
{

    public Transform cameraTransform;
    public Transform craneTransform;
    public float cameraSmoothingSpeed = 10f;

    [Header("Keyboard Input")]
    public float keyboardMovementSpeedSlow = 0.04f;
    public float keyboardMovementSpeedFast = 0.1f;
    public float keyboardRotationSpeedSlow = 0.3f;
    public float keyboardRotationSpeedFast = 0.3f;
    public float keyboardZoomSpeedSlow = 5f;
    public float keyboardZoomSpeedFast = 5f;

    float keyboardMovementSpeed;
    float keyboardRotationSpeed;
    float keyboardZoomSpeed;

    [Header("Mouse Input")]
    public float mouseRotationSpeed = 0.2f;
    public float mouseZoomSpeed = 2f;

    [Header("Camera Limits")]
    public float craneAngleMin = 15f;
    public float craneAngleMax = 60f;
    public float cameraDistanceMin = 50f;
    public float cameraDistanceMax = 100f;

    // Desired position, rotation, and zoom
    Vector3 newPos;
    Quaternion newRotX;
    Quaternion newRotY;
    Vector3 newZoom;

    // Mouse Positions
    Vector3 dragStartPosition;
    Vector3 dragCurrentPosition;
    Vector3 rotateStartPosition;
    Vector3 rotateCurrentPosition;

    #region Events

    void Reset()
    {
        craneTransform = transform.GetChild(0);
        cameraTransform = craneTransform.GetChild(0);
    }

    void Start()
    {
        newPos = transform.position;
        newRotX = transform.rotation;
        newRotY = craneTransform.localRotation;
        newZoom = cameraTransform.localPosition;
    }

    void Update()
    {
        HandleMouseInput();
        HandleKeyboardInput();
    }

    void FixedUpdate()
    {
        // Clamp to limits
        if (newRotY.eulerAngles.x < craneAngleMin)
        {
            newRotY = Quaternion.Euler(Vector3.right * craneAngleMin);
        }
        if (newRotY.eulerAngles.x > craneAngleMax)
        {
            newRotY = Quaternion.Euler(Vector3.right * craneAngleMax);
        }
        if (newZoom.z > -cameraDistanceMin)
        {
            newZoom = Vector3.forward * -cameraDistanceMin;
        }
        if (newZoom.z < -cameraDistanceMax)
        {
            newZoom = Vector3.forward * -cameraDistanceMax;
        }

        // Lerp transformations
        transform.position = Vector3.Lerp(transform.position, newPos, Time.deltaTime * cameraSmoothingSpeed);
        transform.rotation = Quaternion.Lerp(transform.rotation, newRotX, Time.deltaTime * cameraSmoothingSpeed);
        craneTransform.localRotation = Quaternion.Lerp(craneTransform.localRotation, newRotY, Time.deltaTime * cameraSmoothingSpeed);
        cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, newZoom, Time.deltaTime * cameraSmoothingSpeed);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(cameraTransform.position, transform.position);

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(newPos, transform.position);
        Gizmos.DrawSphere(newPos, 0.075f);

        Gizmos.color = Color.gray;
        Gizmos.DrawSphere(transform.position, 0.15f);
    }

    #endregion

    #region Public Methods

    public void SetPosition(Vector3 position, bool smooth = true)
    {
        newPos = position;
        if (!smooth)
        {
            transform.position = newPos;
        }
    }

    public void SetXRotation(float angle, bool smooth = true)
    {
        newRotX = Quaternion.Euler(Vector3.up * angle);
        if (!smooth)
        {
            transform.rotation = newRotX;
        }
    }

    public void SetYRotation(float angle, bool smooth = true)
    {
        newRotY = Quaternion.Euler(Vector3.right * angle);
        if (!smooth)
        {
            craneTransform.localRotation = newRotY;
        }
    }

    public void SetZoom(float distance, bool smooth = true)
    {
        newZoom = Vector3.forward * distance;
        if (!smooth)
        {
            cameraTransform.localPosition = newZoom;
        }
    }

    #endregion

    void HandleMouseInput()
    {
        // Translate
        if (Input.GetMouseButtonDown(0))
        {
            Plane plane = new Plane(Vector3.up, Vector3.zero);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            float entry;

            if (plane.Raycast(ray, out entry))
            {
                dragStartPosition = ray.GetPoint(entry);
            }
        }
        if (Input.GetMouseButton(0))
        {
            Plane plane = new Plane(Vector3.up, Vector3.zero);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            float entry;

            if (plane.Raycast(ray, out entry))
            {
                dragCurrentPosition = ray.GetPoint(entry);

                newPos = transform.position + dragStartPosition - dragCurrentPosition;
            }
        }

        // Rotate
        if (Input.GetMouseButtonDown(1))
        {
            rotateStartPosition = Input.mousePosition;
        }
        if (Input.GetMouseButton(1))
        {
            rotateCurrentPosition = Input.mousePosition;
            Vector3 difference = rotateCurrentPosition - rotateStartPosition;

            newRotX *= Quaternion.Euler(Vector3.up * (difference.x * mouseRotationSpeed));
            newRotY *= Quaternion.Euler(Vector3.right * (difference.y * -mouseRotationSpeed));

            rotateStartPosition = rotateCurrentPosition;
        }

        // Zoom
        if (Input.mouseScrollDelta.y != 0)
        {
            newZoom += Vector3.forward * (Input.mouseScrollDelta.y * mouseZoomSpeed);
        }
    }

    void HandleKeyboardInput()
    {
        // Speed up keyboard controls if holding shift
        if (Input.GetKey(KeyCode.LeftShift))
        {
            keyboardMovementSpeed = keyboardMovementSpeedFast;
            keyboardRotationSpeed = keyboardRotationSpeedFast;
            keyboardZoomSpeed = keyboardZoomSpeedFast;
        }
        else
        {
            keyboardMovementSpeed = keyboardMovementSpeedSlow;
            keyboardRotationSpeed = keyboardRotationSpeedSlow;
            keyboardZoomSpeed = keyboardZoomSpeedSlow;
        }

        // Translate
        Vector2 keyboardMove = Vector2.zero;
        keyboardMove.y += Input.GetKey(KeyCode.W) ? 1 : 0;
        keyboardMove.y -= Input.GetKey(KeyCode.S) ? 1 : 0;
        keyboardMove.x += Input.GetKey(KeyCode.D) ? 1 : 0;
        keyboardMove.x -= Input.GetKey(KeyCode.A) ? 1 : 0;
        keyboardMove.Normalize();

        newPos += (transform.forward * keyboardMove.y * keyboardMovementSpeed);
        newPos += (transform.right * keyboardMove.x * keyboardMovementSpeed);

        // Rotate
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            newRotX *= Quaternion.Euler(Vector3.up * keyboardRotationSpeed);
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            newRotX *= Quaternion.Euler(Vector3.up * -keyboardRotationSpeed);
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            newRotY *= Quaternion.Euler(Vector3.right * keyboardRotationSpeed);
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            newRotY *= Quaternion.Euler(Vector3.right * -keyboardRotationSpeed);
        }

        // Zoom
        if (Input.GetKey(KeyCode.E))
        {
            newZoom += (Vector3.forward * keyboardZoomSpeed);
        }
        if (Input.GetKey(KeyCode.Q))
        {
            newZoom += (Vector3.forward * -keyboardZoomSpeed);
        }
    }
}
