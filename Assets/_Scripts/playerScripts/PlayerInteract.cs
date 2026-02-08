using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    [SerializeField] private float distance = 5f;
    [SerializeField] private LayerMask layerMask;
    
    private Camera _cam;
    private PlayerUI _playerUI;
    private InputManager _input;

    void Start()
    {
        _playerUI = GetComponent<PlayerUI>();
        _input = GetComponent<InputManager>();
        
        PlayerLook lookScript = GetComponent<PlayerLook>();
        if (lookScript != null)
        {
            _cam = lookScript.playerCamera;
        }

        _input.OnInteract += AttemptInteract;
    }

    void Update()
    {
        if (!_cam) return;

        Ray ray = new Ray(_cam.transform.position, _cam.transform.forward);
        
        if (Physics.Raycast(ray, out RaycastHit hit, distance, layerMask) && 
            hit.collider.TryGetComponent(out Interactable interactable))
        {
            _playerUI.UpdateText(interactable.promptMessage);
        }
        else
        {
            _playerUI.UpdateText(string.Empty);
        }
    }

    private void AttemptInteract()
    {
        if (!_cam) return;

        Ray ray = new Ray(_cam.transform.position, _cam.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, distance, layerMask))
        {
            if (hit.collider.TryGetComponent(out Interactable interactable))
                interactable.BaseInteract();
        }
    }
}