using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FrameManager : MonoBehaviour
{
    [SerializeField] private Image woodenFrameImage;
    [SerializeField] private Image floralFrameImage;
    [SerializeField] private AudioClip frameChangeSound;
    
    void Start()
    {
        if (woodenFrameImage != null) woodenFrameImage.gameObject.SetActive(false);
        if (floralFrameImage != null) floralFrameImage.gameObject.SetActive(false);
    }

    public void ToggleWoodenFrame(bool show)
    {
        if (frameChangeSound != null)
        {
            AudioSource.PlayClipAtPoint(frameChangeSound, Camera.main.transform.position);
        }
        if (woodenFrameImage != null)
            woodenFrameImage.gameObject.SetActive(show);
        floralFrameImage.gameObject.SetActive(false);
    }
    
    public void ToggleFloralFrame(bool show)
    {
        if (frameChangeSound != null)
        {
            AudioSource.PlayClipAtPoint(frameChangeSound, Camera.main.transform.position);
        }
        if (floralFrameImage != null)
            floralFrameImage.gameObject.SetActive(show);
        woodenFrameImage.gameObject.SetActive(false);
    }
    
    public void NoFrame()
    {
        if (frameChangeSound != null)
        {
            AudioSource.PlayClipAtPoint(frameChangeSound, Camera.main.transform.position);
        }
        if (woodenFrameImage != null) woodenFrameImage.gameObject.SetActive(false);
        if (floralFrameImage != null) floralFrameImage.gameObject.SetActive(false);
    }
}