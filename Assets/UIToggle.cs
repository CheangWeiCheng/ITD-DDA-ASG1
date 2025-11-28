using UnityEngine;
using UnityEngine.UI;

public class UIToggle : MonoBehaviour
{
    [SerializeField]
    private Canvas uiElement;
    [SerializeField]
    private Button toggleButton;
    
    void Start()
    {
        uiElement.enabled = false; // Use .enabled for Canvas component
    }
    
    public void ToggleUIElement()
    {
        uiElement.enabled = !uiElement.enabled; // Toggle Canvas enabled state
        toggleButton.gameObject.SetActive(!uiElement.enabled);
    }
}