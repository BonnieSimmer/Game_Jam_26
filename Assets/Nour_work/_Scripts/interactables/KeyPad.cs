using UnityEngine;

public class KeyPad : Interactable
{
    [SerializeField] private GameObject _door;
    private Animator _doorAnimator;
    private bool _doorOpen;

    void Start()
    {
        if (_door != null)
        {
            _doorAnimator = _door.GetComponent<Animator>();
        }
        else
        {
            Debug.LogWarning($"Door reference missing on {gameObject.name}!");
        }
    }

    protected override void Interact()
    {
        if (!_doorAnimator) return;

        _doorOpen = !_doorOpen;
        _doorAnimator.SetBool("Open", _doorOpen);
        promptMessage = _doorOpen ? "Press to Close Door" : "Press to Open Door";
    }
}