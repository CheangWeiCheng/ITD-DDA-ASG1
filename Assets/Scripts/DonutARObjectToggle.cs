using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using Firebase.Extensions;
using Firebase.Database;
using System;
using System.Collections.Generic;

public class DonutARObjectToggle : MonoBehaviour
{
    [SerializeField]
    public MeshRenderer meshRenderer;
    [SerializeField]
    private Canvas uiElement;
    [SerializeField]
    private TMP_Text text;
    [SerializeField]
    private Button toggleButton;
    [SerializeField]
    private AudioClip toggleSound;
    [SerializeField]
    public TMP_Dropdown flavorDropdown;
    [SerializeField]
    public TMP_Dropdown donutTypeDropdown;
    [SerializeField]
    public Material[] flavorMaterials;
    [SerializeField]
    public Mesh[] donutMeshes;
    [SerializeField]
    private XRGrabInteractable xrGrabInteractable;

    void Start()
    {
        if (text == null || uiElement == null || xrGrabInteractable == null)
        {
            Debug.Log("DonutARObjectToggle: missing references");
            return;
        }
        text.gameObject.SetActive(false);
        uiElement.enabled = false;
        xrGrabInteractable.enabled = false;
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

    public void SaveOrderToFirebase()
    {
        if (flavorDropdown == null) return;
        
        Dictionary<string, object> orderData = new Dictionary<string, object>
        {
            { "itemType", "Donut" },
            { "donutFlavor", flavorDropdown.options[flavorDropdown.value].text },
            { "timestamp", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss") }
        };
        
        if (donutTypeDropdown != null)
        {
            orderData["donutType"] = donutTypeDropdown.options[donutTypeDropdown.value].text;
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