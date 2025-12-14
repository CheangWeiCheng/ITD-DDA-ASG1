using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using Firebase.Extensions;
using Firebase.Database;
using System;
using System.Collections.Generic;

public class CoffeeARObjectToggle : MonoBehaviour
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
    private AudioClip toggleSound;
    [SerializeField]
    private TMP_Dropdown flavorDropdown;
    [SerializeField]
    private TMP_Dropdown sizeDropdown;
    [SerializeField]
    private Material[] flavorMaterials;
    [SerializeField]
    private XRGrabInteractable xrGrabInteractable;

    [SerializeField]
    private GameObject donutStuff;
    [SerializeField]
    private bool hasAttachedDonut = false;

    // Cache for donut settings when attached
    private int cachedDonutFlavorIndex = 0;
    private int cachedDonutTypeIndex = 0;

    void Start()
    {
        text.gameObject.SetActive(false);
        uiElement.enabled = false;
        xrGrabInteractable.enabled = false;

        if (donutStuff != null)
        {
            donutStuff.SetActive(false);
        }
    }

    public void ToggleMeshRenderer()
    {
        if (toggleSound != null)
        {
            AudioSource.PlayClipAtPoint(toggleSound, Camera.main.transform.position);
        }

        if (meshRenderer != null)
            meshRenderer.enabled = !meshRenderer.enabled;
        
        if (uiElement != null)
            uiElement.gameObject.SetActive(false);
        
        if (text != null)
            text.gameObject.SetActive(true);
    }
    
    public void ToggleUIElement()
    {
        if (toggleSound != null)
        {
            AudioSource.PlayClipAtPoint(toggleSound, Camera.main.transform.position);
        }
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

    void OnTriggerEnter(Collider other)
    {
        if (!hasAttachedDonut && other.CompareTag("Donut"))
        {
            // Get the donut's settings before destroying it
            DonutARObjectToggle donutScript = other.GetComponent<DonutARObjectToggle>();
            if (donutScript != null)
            {
                // Cache the donut's current settings
                if (donutScript.flavorDropdown != null)
                {
                    cachedDonutFlavorIndex = donutScript.flavorDropdown.value;
                }
                
                if (donutScript.donutTypeDropdown != null)
                {
                    cachedDonutTypeIndex = donutScript.donutTypeDropdown.value;
                }
            }
            
            // Activate the attached donut and apply settings
            donutStuff.SetActive(true);
            hasAttachedDonut = true;
            
            // Apply the cached settings to the attached donut
            ApplyDonutSettingsToChild();
            
            // Scale donut based on coffee size (small is default)
            if (sizeDropdown != null)
            {
                int selectedSizeIndex = sizeDropdown.value;
                Vector3 donutScale = Vector3.one;
                
                switch (selectedSizeIndex)
                {
                    case 1: // Medium
                        donutScale = Vector3.one * (0.5f / 0.6f); // 0.833x
                        break;
                    case 2: // Large
                        donutScale = Vector3.one * (0.5f / 0.7f); // 0.714x
                        break;
                    // Small (case 0) doesn't need scaling
                }
                
                // Apply scale to donut if it's not small
                if (selectedSizeIndex > 0)
                {
                    MeshRenderer donutMesh = donutStuff.GetComponentInChildren<MeshRenderer>();
                    if (donutMesh != null)
                    {
                        donutMesh.transform.localScale = donutScale;
                    }
                }
            }
            
            // Destroy the donut object
            Destroy(other.gameObject);
            
            Debug.Log($"Donut attached! Flavor: {cachedDonutFlavorIndex}, Type: {cachedDonutTypeIndex}");
        }
    }

    // Apply cached donut settings to the child donut object
    private void ApplyDonutSettingsToChild()
    {
        if (!hasAttachedDonut || donutStuff == null) return;
        
        // Get the child donut's ARObjectToggle script
        DonutARObjectToggle childDonutScript = donutStuff.GetComponent<DonutARObjectToggle>();
        if (childDonutScript == null) 
        {
            // If the child doesn't have the script, add it or find it in children
            childDonutScript = donutStuff.GetComponentInChildren<DonutARObjectToggle>(true);
        }
        
        if (childDonutScript != null)
        {
            // Apply flavor
            if (childDonutScript.flavorDropdown != null && 
                childDonutScript.flavorMaterials != null && 
                cachedDonutFlavorIndex < childDonutScript.flavorMaterials.Length)
            {
                childDonutScript.flavorDropdown.value = cachedDonutFlavorIndex;
                childDonutScript.meshRenderer.material = childDonutScript.flavorMaterials[cachedDonutFlavorIndex];
            }
            
            // Apply donut type
            if (childDonutScript.donutTypeDropdown != null && 
                childDonutScript.donutMeshes != null && 
                cachedDonutTypeIndex < childDonutScript.donutMeshes.Length)
            {
                childDonutScript.donutTypeDropdown.value = cachedDonutTypeIndex;
                MeshFilter meshFilter = childDonutScript.GetComponent<MeshFilter>();
                if (meshFilter != null)
                {
                    meshFilter.mesh = childDonutScript.donutMeshes[cachedDonutTypeIndex];
                }
            }
        }
    }

    public void SaveOrderToFirebase()
    {
        if (flavorDropdown == null || sizeDropdown == null) return;
        
        Dictionary<string, object> orderData = new Dictionary<string, object>
        {
            { "itemType", "Coffee" },
            { "coffeeFlavor", flavorDropdown.options[flavorDropdown.value].text },
            { "coffeeSize", sizeDropdown.options[sizeDropdown.value].text },
            { "timestamp", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss") }
        };

        // ADD donut info only if attached
        if (donutStuff != null && donutStuff.activeSelf)
        {
            // Get the CHILD DONUT'S script and dropdowns
            DonutARObjectToggle donutChildScript = donutStuff.GetComponent<DonutARObjectToggle>();
            
            if (donutChildScript != null)
            {
                // Get flavor from child donut (might be different from coffee flavor)
                if (donutChildScript.flavorDropdown != null)
                {
                    orderData["donutFlavor"] = donutChildScript.flavorDropdown.options[donutChildScript.flavorDropdown.value].text;
                }
                
                // Get type from child donut
                if (donutChildScript.donutTypeDropdown != null)
                {
                    orderData["donutType"] = donutChildScript.donutTypeDropdown.options[donutChildScript.donutTypeDropdown.value].text;
                }
                
                orderData["itemType"] = "Coffee + Donut Set";
            }
            else
            {
                // Fallback: just mark as having donut
                orderData["hasDonut"] = true;
            }
        }
        else
        {
            orderData["itemType"] = "Coffee";
        }
        
        SaveToFirebase(orderData);
    }

    private void SaveToFirebase(Dictionary<string, object> orderData)
    {
        string orderId = DateTime.UtcNow.ToString("yyyyMMddHHmmssfff");
        
        FirebaseManager.DBref.Child("orders").Child(orderId).SetValueAsync(orderData).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted && !task.IsFaulted)
            {
                Debug.Log($"✅ Order saved: {orderId}");
                
                // Show what was saved
                string orderSummary = "Order Summary:\n";
                foreach (var item in orderData)
                {
                    orderSummary += $"{item.Key}: {item.Value}\n";
                }
                Debug.Log(orderSummary);
            }
            else
            {
                Debug.LogError($"❌ Save failed: {task.Exception}");
            }
        });
    }
}