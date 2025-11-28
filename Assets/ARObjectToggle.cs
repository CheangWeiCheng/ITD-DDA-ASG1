using UnityEngine;
using TMPro;

public class ARObjectToggle : MonoBehaviour
{
    [SerializeField]
    private MeshRenderer meshRenderer;
    [SerializeField]
    private Canvas uiElement;
    [SerializeField]
    private TMP_Text text;

    void Start()
    {
        text.gameObject.SetActive(false);
    }

    public void ToggleMeshRenderer()
    {
        meshRenderer.enabled = !meshRenderer.enabled;
        uiElement.gameObject.SetActive(false);
        text.gameObject.SetActive(true);
    }
}
