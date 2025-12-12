using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;
using UnityEngine;

/// <summary>
/// The GoalManager cycles through a list of Goals, each representing
/// </summary>
public class GoalManager : MonoBehaviour
{

    [Tooltip("The greeting prompt Game Object to show when onboarding begins.")]
    [SerializeField]
    GameObject m_GreetingPrompt;

    [SerializeField]
    GameObject loginPanel;

    /// <summary>
    /// The greeting prompt Game Object to show when onboarding begins.
    /// </summary>
    public GameObject greetingPrompt
    {
        get => m_GreetingPrompt;
        set => m_GreetingPrompt = value;
    }

    /// <summary>
    /// The Back button to enable once the greeting prompt is dismissed.
    /// </summary>
    [SerializeField]
    GameObject m_BackButton;

    [SerializeField]
    AudioClip m_SoundEffectClip;

    const int k_NumberOfSurfacesTappedToCompleteGoal = 1;

    Coroutine m_CurrentCoroutine;

    /// <summary>
    /// Triggers a restart of the onboarding/coaching process.
    /// </summary>
    public void StartCoaching()
    {
        if (m_SoundEffectClip != null)
        {
            AudioSource.PlayClipAtPoint(m_SoundEffectClip, Camera.main.transform.position);
        }

        m_GreetingPrompt.SetActive(false);
        loginPanel.SetActive(false);
        m_BackButton.SetActive(true);
    }

    /// <summary>
    /// Brings the player back to the greeting prompt and resets the onboarding process.
    /// </summary>
    public void ReturnToGreetingPrompt()
    {
        // Play sound effect
        if (m_SoundEffectClip != null)
        {
            AudioSource.PlayClipAtPoint(m_SoundEffectClip, Camera.main.transform.position);
        }

        if (m_CurrentCoroutine != null)
        {
            StopCoroutine(m_CurrentCoroutine);
        }

        m_GreetingPrompt.SetActive(true);
        m_BackButton.SetActive(false);
    }
}