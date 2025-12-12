using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using System.Collections.Generic;
using System;
using UnityEngine.XR.ARFoundation;

public class Signup : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_InputField emailInput;
    [SerializeField] private TMP_InputField passwordInput;
    [SerializeField] private UnityEngine.UI.Button signupButton;

    [Header("Navigation Options")]
    [SerializeField] private GameObject signupPanel;

    private FirebaseAuth auth;
    private DatabaseReference databaseRef;
    private bool isFirebaseInitialized = false;

    void Start()
    {
        // Disable button until Firebase is ready
        if (signupButton != null)
        {
            signupButton.interactable = false;
        }

        // Initialize Firebase Auth
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Result == DependencyStatus.Available)
            {
                auth = FirebaseAuth.DefaultInstance;
                databaseRef = FirebaseDatabase.DefaultInstance.RootReference;
                isFirebaseInitialized = true;
                
                // Enable button once Firebase is ready
                if (signupButton != null)
                {
                    signupButton.interactable = true;
                }
                
                Debug.Log("Firebase Auth initialized successfully - Ready to sign up!");
            }
            else
            {
                Debug.LogError($"Could not resolve Firebase dependencies: {task.Result}");
                ShowFeedback("Firebase initialization failed!", true);
            }
        });
    }

    public void SignUpButton()
    {
        // Check if Firebase is initialized
        if (!isFirebaseInitialized || auth == null)
        {
            Debug.LogWarning("Please wait for Firebase to initialize...");
            return;
        }

        // Validate inputs
        if (emailInput == null || passwordInput == null)
        {
            Debug.LogError("Input fields are not assigned!");
            ShowFeedback("Configuration error!", true);
            return;
        }

        string email = emailInput.text.Trim();
        string password = passwordInput.text;

        // Validate email and password
        if (string.IsNullOrEmpty(email))
        {
            ShowFeedback("Please enter an email address", true);
            return;
        }

        if (string.IsNullOrEmpty(password))
        {
            ShowFeedback("Please enter a password", true);
            return;
        }

        if (password.Length < 6)
        {
            ShowFeedback("Password must be at least 6 characters", true);
            return;
        }

        if (!IsValidEmail(email))
        {
            ShowFeedback("Please enter a valid email address", true);
            return;
        }

        // Proceed with signup
        ShowFeedback("Creating account...", false);
        CreateUserAccount(email, password);
    }

    private void CreateUserAccount(string email, string password)
    {
        if (auth == null)
        {
            ShowFeedback("Firebase not initialized!", true);
            return;
        }

        auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                ShowFeedback("Account creation was cancelled", true);
                return;
            }

            if (task.IsFaulted)
            {
                HandleAuthError(task.Exception);
                return;
            }

            // Account created successfully
            AuthResult result = task.Result;
            FirebaseUser newUser = result.User;
            
            Debug.Log($"User created successfully: {newUser.Email} (UID: {newUser.UserId})");
            
            // Save user data to database
            SaveUserDataToDatabase(newUser.UserId, email);
        });
    }

    private void SaveUserDataToDatabase(string userId, string email)
    {
        if (databaseRef == null)
        {
            ShowFeedback("Account created but data save failed", true);
            return;
        }

        // Create user data
        Dictionary<string, object> userData = new Dictionary<string, object>
        {
            { "email", email },
            { "createdAt", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss") },
            { "lastLogin", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss") }
        };

        // Save to Firebase Database under users/{userId}
        databaseRef.Child("users").Child(userId).SetValueAsync(userData).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted && !task.IsFaulted)
            {
                Debug.Log("User data saved to database successfully");
                ShowFeedback("Account created successfully!", false);
                
                // Clear input fields
                if (emailInput != null) emailInput.text = "";
                if (passwordInput != null) passwordInput.text = "";
                
                // Perform StartCoaching() from GoalManager
                GoalManager goalManager = FindFirstObjectByType<GoalManager>();
                if (goalManager != null)
                {
                    goalManager.StartCoaching();
                }
            }
            else
            {
                Debug.LogError("Failed to save user data: " + task.Exception);
                ShowFeedback("Account created but profile save failed", true);
            }
        });
    }

    private void HandleAuthError(AggregateException exception)
    {
        FirebaseException firebaseEx = exception.GetBaseException() as FirebaseException;
        
        if (firebaseEx != null)
        {
            AuthError errorCode = (AuthError)firebaseEx.ErrorCode;
            
            switch (errorCode)
            {
                case AuthError.EmailAlreadyInUse:
                    ShowFeedback("Email is already registered", true);
                    break;
                case AuthError.InvalidEmail:
                    ShowFeedback("Invalid email format", true);
                    break;
                case AuthError.WeakPassword:
                    ShowFeedback("Password is too weak", true);
                    break;
                case AuthError.NetworkRequestFailed:
                    ShowFeedback("Network error. Check your connection", true);
                    break;
                default:
                    ShowFeedback($"Error: {errorCode}", true);
                    break;
            }
        }
        else
        {
            ShowFeedback("Account creation failed", true);
        }
        
        Debug.LogError("Auth error: " + exception);
    }

    private bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }

    private void ShowFeedback(string message, bool isError)
    {
        Debug.Log($"{(isError ? "Error" : "Success")}: {message}");
    }
}
