using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    private Camera _camera;
    [SerializeField]
    private float _distance = 3f;
    [SerializeField]
    private LayerMask _layerMask;
    private PlayerUI _playerUI;
    private InputManager _input;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _camera = GetComponent<PlayerLook>().camera;
        _playerUI = GetComponent<PlayerUI>();
        _input = GetComponent<InputManager>();
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = new Ray(_camera.transform.position, _camera.transform.forward);
        bool hitAnything = false;

        if (Physics.Raycast(ray, out RaycastHit hitInfo, _distance, _layerMask))
        {
            if (hitInfo.collider.TryGetComponent(out Interactable interactable))
            {
                hitAnything = true;
                _playerUI.UpdateText(interactable.promptMessage);
                if (_input.OnFootActions.Interact.triggered)
                {
                    interactable.BaseInteract();
                }
            }
        }

        if (!hitAnything)
        {
            _playerUI.UpdateText(string.Empty);
        }
    }
}
