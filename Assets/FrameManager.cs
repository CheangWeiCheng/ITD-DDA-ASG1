using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FrameManager : MonoBehaviour
{
    [SerializeField] private Image woodenFrameImage;
    [SerializeField] private Image floralFrameImage;
    
    void Start()
    {
        NoFrame();
    }
    
    public void ToggleWoodenFrame(bool show)
    {
        if (woodenFrameImage != null)
            woodenFrameImage.gameObject.SetActive(show);
        floralFrameImage.gameObject.SetActive(false);
    }
    
    public void ToggleFloralFrame(bool show)
    {
        if (floralFrameImage != null)
            floralFrameImage.gameObject.SetActive(show);
        woodenFrameImage.gameObject.SetActive(false);
    }
    
    public void NoFrame()
    {
        if (woodenFrameImage != null) woodenFrameImage.gameObject.SetActive(false);
        if (floralFrameImage != null) floralFrameImage.gameObject.SetActive(false);
    }
}