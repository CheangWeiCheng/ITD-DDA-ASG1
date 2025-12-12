using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using System;

public class Login : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_InputField emailInput;
    [SerializeField] private TMP_InputField passwordInput;
    [SerializeField] private UnityEngine.UI.Button loginButton;

    [Header("Navigation Options")]
    [SerializeField] private GameObject loginPanel;

    private FirebaseAuth auth;
    private DatabaseReference databaseRef;
    private bool isFirebaseInitialized = false;

    void Start()
    {
        // Disable button until Firebase is ready
        if (loginButton != null)
        {
            loginButton.interactable = false;
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
                if (loginButton != null)
                {
                    loginButton.interactable = true;
                }
                
                Debug.Log("Firebase Auth initialized - Ready to login!");
            }
            else
            {
                Debug.LogError($"Could not resolve Firebase dependencies: {task.Result}");
                ShowFeedback("Firebase initialization failed!", true);
            }
        });
    }

    public void LoginButton()
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
            ShowFeedback("Please enter your email", true);
            return;
        }

        if (string.IsNullOrEmpty(password))
        {
            ShowFeedback("Please enter your password", true);
            return;
        }

        // Proceed with login
        ShowFeedback("Logging in...", false);
        LoginUser(email, password);
    }

    private void LoginUser(string email, string password)
    {
        if (auth == null)
        {
            ShowFeedback("Firebase not initialized!", true);
            return;
        }

        auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                ShowFeedback("Login was cancelled", true);
                return;
            }

            if (task.IsFaulted)
            {
                HandleAuthError(task.Exception);
                return;
            }

            // Login successful
            AuthResult result = task.Result;
            FirebaseUser user = result.User;
            
            Debug.Log($"User logged in successfully: {user.Email} (UID: {user.UserId})");
            
            // Update last login time
            UpdateLastLoginTime(user.UserId);
            
            ShowFeedback("Login successful!", false);
            
            // Clear password field for security
            if (passwordInput != null) passwordInput.text = "";

            // Perform StartCoaching() from GoalManager
            GoalManager goalManager = FindFirstObjectByType<GoalManager>();
            if (goalManager != null)
            {
                goalManager.StartCoaching();
            }
        });
    }

    private void UpdateLastLoginTime(string userId)
    {
        if (databaseRef == null) return;

        databaseRef.Child("users").Child(userId).Child("lastLogin")
            .SetValueAsync(DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"))
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted && !task.IsFaulted)
                {
                    Debug.Log("Last login time updated");
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
                case AuthError.UserNotFound:
                    ShowFeedback("No account found with this email", true);
                    break;
                case AuthError.WrongPassword:
                    ShowFeedback("Incorrect password", true);
                    break;
                case AuthError.InvalidEmail:
                    ShowFeedback("Invalid email format", true);
                    break;
                case AuthError.NetworkRequestFailed:
                    ShowFeedback("Network error. Check your connection", true);
                    break;
                case AuthError.UserDisabled:
                    ShowFeedback("This account has been disabled", true);
                    break;
                default:
                    ShowFeedback($"Error: {errorCode}", true);
                    break;
            }
        }
        else
        {
            ShowFeedback("Login failed", true);
        }
        
        Debug.LogError("Auth error: " + exception);
    }

    private void ShowFeedback(string message, bool isError)
    {
        Debug.Log($"{(isError ? "Error" : "Success")}: {message}");
    }

    // Optional: Get current logged in user
    public FirebaseUser GetCurrentUser()
    {
        return auth?.CurrentUser;
    }

    // Optional: Logout function
    public void LogoutButton()
    {
        if (auth != null)
        {
            auth.SignOut();
            Debug.Log("User logged out");
            ShowFeedback("Logged out successfully", false);
            
            // Clear input fields
            if (emailInput != null) emailInput.text = "";
            if (passwordInput != null) passwordInput.text = "";
        }
    }
}
