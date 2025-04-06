using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour {
    [SerializeField] private float speed = 15.0f;
    [SerializeField] private float sens = 1.0f;
    [SerializeField] private float gravity = -9.8f;
    [SerializeField] private LayerMask excludePlayerLayer;
    [SerializeField] private GameObject body;
    private Camera pCamera;
    private Rigidbody rb;
    private Vector2 eulerAngles;
    
    void Awake() {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        pCamera = Camera.main;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
    
    void Start() {
        
    }

    void Update() {
        // Look
        eulerAngles += Mouse.current.delta.value * sens;
        eulerAngles.y = Mathf.Clamp(eulerAngles.y, -90f, 90f);
        Quaternion newRotation = Quaternion.AngleAxis(eulerAngles.x, Vector3.up) * Quaternion.AngleAxis(eulerAngles.y, -Vector3.right);
        body.transform.localEulerAngles = new Vector3(0f, newRotation.eulerAngles.y, 0f);
        pCamera.transform.localEulerAngles = new Vector3(newRotation.eulerAngles.x, 0f, 0f);
        
        var ray = new Ray(transform.position, -transform.up);
        if (!Physics.Raycast(ray, out var hit, float.PositiveInfinity, excludePlayerLayer.value)) {
            transform.position = Vector3.zero;
            transform.rotation = Quaternion.identity;
            eulerAngles = Vector2.zero;
        }
        
        // Gravity
        Physics.gravity = (transform.up = hit.normal) * gravity;

        // Move
        bool
            w = Keyboard.current.wKey.isPressed,
            a = Keyboard.current.aKey.isPressed,
            s = Keyboard.current.sKey.isPressed,
            d = Keyboard.current.dKey.isPressed;

        var movementDirection = new Vector3( // manual input handling
            a || d ? a ? -1.0f : 1.0f : 0.0f,
            0.0f,
            w || s ? s ? -1.0f : 1.0f : 0.0f
        );

        float mps = speed * Time.deltaTime;
        rb.linearVelocity += body.transform.forward * (movementDirection.z * mps);
        rb.linearVelocity += body.transform.right * (movementDirection.x * mps);
    }
}