using UnityEngine;
using UnityEngine.EventSystems;

public class DialogueFocusEnforcer : MonoBehaviour
{
    private GameObject lastSelectedObject;
    private EventSystem eventSystem;

    void Start()
    {
        eventSystem = EventSystem.current;
    }

    void Update()
    {
        // 1. If the Event System is missing, abort (Safety)
        if (eventSystem == null)
        {
            eventSystem = EventSystem.current;
            return;
        }

        // 2. Monitor Selection
        if (eventSystem.currentSelectedGameObject != null)
        {
            // If something is selected, remember it!
            lastSelectedObject = eventSystem.currentSelectedGameObject;
        }
        else
        {
            // 3. THE FIX: If nothing is selected (user clicked empty space)...
            // Force the selection back to the last known button.
            if (lastSelectedObject != null && lastSelectedObject.activeInHierarchy)
            {
                eventSystem.SetSelectedGameObject(lastSelectedObject);
            }
        }
    }

    // Call this when Dialogue Starts to unlock mouse
    public void EnableDialogueMode()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Optional: Force select the first button found in children if nothing is selected
        // var firstButton = GetComponentInChildren<Button>();
        // if(firstButton) eventSystem.SetSelectedGameObject(firstButton.gameObject);
    }

    // Call this when Dialogue Ends to lock mouse back
    public void DisableDialogueMode()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        lastSelectedObject = null;
        eventSystem.SetSelectedGameObject(null);
    }
}
