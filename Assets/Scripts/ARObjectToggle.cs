using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using Firebase.Extensions;
using Firebase.Database;
using System;
using System.Collections.Generic;

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
    private AudioClip toggleSound;
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

    public void SaveOrderToFirebase()
    {
        if (isCoffee)
        {
            SaveCoffeeOrder();
        }
        else
        {
            SaveDonutOrder();
        }
    }

    private void SaveCoffeeOrder()
    {
        if (flavorDropdown == null || sizeDropdown == null) return;
        
        Dictionary<string, object> orderData = new Dictionary<string, object>
        {
            { "itemType", "Coffee" },
            { "flavor", flavorDropdown.options[flavorDropdown.value].text },
            { "size", sizeDropdown.options[sizeDropdown.value].text },
            { "timestamp", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss") }
        };

        // ADD donut info only if attached
        if (donutStuff != null)
        {
            orderData["hasAttachedDonut"] = true;
        }
        else
        {
            orderData["hasAttachedDonut"] = false;
        }
        
        SaveToFirebase(orderData);
    }

    private void SaveDonutOrder()
    {
        if (flavorDropdown == null || donutTypeDropdown != null) 
        {
            // For donuts, flavorDropdown is required, donutTypeDropdown is optional
            Dictionary<string, object> orderData = new Dictionary<string, object>
            {
                { "itemType", "Donut" },
                { "flavor", flavorDropdown.options[flavorDropdown.value].text },
                { "timestamp", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss") }
            };
            
            if (donutTypeDropdown != null)
            {
                orderData["donutType"] = donutTypeDropdown.options[donutTypeDropdown.value].text;
            }
            
            SaveToFirebase(orderData);
        }
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
