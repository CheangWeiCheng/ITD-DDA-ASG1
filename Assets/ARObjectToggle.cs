using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class ARObjectToggle : MonoBehaviour
{
    [SerializeField]
    private MeshRenderer meshRenderer;
    [SerializeField]
    private Canvas uiElement;
    [SerializeField]
    private TMP_Text text;
    [SerializeField]
    private Button toggleButton;
    [SerializeField]
    private TMP_Dropdown flavorDropdown;
    [SerializeField]
    private TMP_Dropdown sizeDropdown;
    [SerializeField]
    private TMP_Dropdown donutTypeDropdown;
    [SerializeField]
    private Material[] flavorMaterials;
    [SerializeField]
    private Mesh[] donutMeshes;
    [SerializeField]
    private XRGrabInteractable xrGrabInteractable;

    [SerializeField]
    private GameObject donutStuff;
    [SerializeField]
    private bool isCoffee = true;
    [SerializeField]
    private bool hasAttachedDonut = false;

    void Start()
    {
        text.gameObject.SetActive(false);
        uiElement.enabled = false;
        xrGrabInteractable.enabled = false;

        if (isCoffee && donutStuff != null)
        {
            donutStuff.SetActive(false);
        }
    }

    public void ToggleMeshRenderer()
    {
        meshRenderer.enabled = !meshRenderer.enabled;
        uiElement.gameObject.SetActive(false);
        text.gameObject.SetActive(true);
    }
    
    public void ToggleUIElement()
    {
        uiElement.enabled = !uiElement.enabled; // Toggle Canvas enabled state
        toggleButton.gameObject.SetActive(!uiElement.enabled);
        xrGrabInteractable.enabled = !xrGrabInteractable.enabled;
    }

    public void UpdateFlavor()
    {
        int selectedFlavorIndex = flavorDropdown.value;
        if (selectedFlavorIndex >= 0 && selectedFlavorIndex < flavorMaterials.Length)
        {
            meshRenderer.material = flavorMaterials[selectedFlavorIndex];
        }
    }

    public void UpdateSize()
    {
        int selectedSizeIndex = sizeDropdown.value;
        Vector3 coffeeScale = Vector3.one;
        Vector3 donutScale = Vector3.one;

        switch (selectedSizeIndex)
        {
            case 0: // Small
                coffeeScale = Vector3.one * 0.5f;
                donutScale = Vector3.one * (0.5f / 0.5f); // 1.0x
                break;
            case 1: // Medium
                coffeeScale = Vector3.one * 0.6f;
                donutScale = Vector3.one * (0.5f / 0.6f); // 0.833x
                break;
            case 2: // Large
                coffeeScale = Vector3.one * 0.7f;
                donutScale = Vector3.one * (0.5f / 0.7f); // 0.714x
                break;
        }

        // Scale coffee mesh
        meshRenderer.transform.localScale = coffeeScale;
        
        // Scale attached donut inversely
        if (hasAttachedDonut && donutStuff != null)
        {
            // Get the donut mesh renderer from donutStuff
            MeshRenderer donutMesh = donutStuff.GetComponentInChildren<MeshRenderer>();
            if (donutMesh != null)
            {
                donutMesh.transform.localScale = donutScale;
            }
        }
    }

    public void UpdateDonutType()
    {
        int selectedTypeIndex = donutTypeDropdown.value;
        
        if (donutMeshes != null && selectedTypeIndex >= 0 && selectedTypeIndex < donutMeshes.Length)
        {
            MeshFilter meshFilter = GetComponent<MeshFilter>();
            if (meshFilter != null)
            {
                meshFilter.mesh = donutMeshes[selectedTypeIndex];
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (isCoffee && !hasAttachedDonut && other.CompareTag("Donut"))
        {
            donutStuff.SetActive(true);
            hasAttachedDonut = true;
            // Destroy the donut object
            Destroy(other.gameObject);
        }
    }

    public void ResetEverything()
    {
        meshRenderer.enabled = true;
        uiElement.enabled = false;
        toggleButton.gameObject.SetActive(true);
        text.gameObject.SetActive(false);
        xrGrabInteractable.enabled = false;

        // Reset flavor
        if (flavorDropdown != null)
        {
            flavorDropdown.value = 0;
            UpdateFlavor();
        }

        // Reset size
        if (sizeDropdown != null)
        {
            sizeDropdown.value = 1; // Default to Medium
            UpdateSize();
        }

        // Reset donut type
        if (donutTypeDropdown != null)
        {
            donutTypeDropdown.value = 0;
            UpdateDonutType();
        }
    }
}
