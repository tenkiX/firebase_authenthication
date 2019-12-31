using UnityEngine;
using System;
using Firebase;
using Firebase.Auth;

public class FirebaseManager : MonoBehaviour
{
    FirebaseAuth auth;
    FirebaseUser user;

    void Start()
    {
        // Set auth instance
        auth = FirebaseAuth.DefaultInstance;
        auth.StateChanged += AuthStateChanged;
        AuthStateChanged(this, null);
    }

    void OnDestroy()
    {
        if (auth != null)
        {
            auth.StateChanged -= AuthStateChanged;
            auth = null;
        }
    }

    // Track state changes of the auth object.
    void AuthStateChanged(object sender, EventArgs eventArgs)
    {
        if (auth.CurrentUser != user)
        {
            bool signedIn = user != auth.CurrentUser && auth.CurrentUser != null;
            if (!signedIn && user != null)
            {
                Debug.LogWarning(user.UserId + " is not authenticated.");
            }
            user = auth.CurrentUser;
            if (signedIn)
            {
                Debug.Log(user.UserId + " is authenticated.");
            }
        }
    }

    public async void RegisterUserAsync(string email, string password, Action OnSuccess, Action<string> OnError)
    {
        AggregateException exception = null;
        await auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWith(task =>
        {
            if (task.IsCanceled || task.IsFaulted)
            {
                //if we would call OnError here, we would not be able to use unity main-thread functions there like Instantiate
                exception = task.Exception;
            }
        });
        if (exception != null)
        {
            OnError(GetAuthErrorMessage(exception));
        }
        else
        {
            OnSuccess();
        }
    }

    public async void LogIn(string email, string password, Action OnSuccess, Action<string> OnError)
    {
        AggregateException exception = null;
        await auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWith(task =>
        {
            if (task.IsCanceled || task.IsFaulted)
            {
                exception = task.Exception;
            }
        });
        if (exception != null)
        {
            OnError(GetAuthErrorMessage(exception));
        }
        else
        {
            OnSuccess();
        }
    }
    
    public void LogOut()
    {
        auth.SignOut();
        Debug.Log("Logged out");
    }

    /// <summary> Convert Firebase Authentication error messages to a readable format </summary>
    string GetAuthErrorMessage(AggregateException exception)
    {
        FirebaseException ex = null;
        foreach (Exception e in exception.Flatten().InnerExceptions)
        {
            ex = e as FirebaseException;
            if (ex != null) { break; }
        }
        var message = "";
        switch ((AuthError)ex.ErrorCode)
        {
            case AuthError.AccountExistsWithDifferentCredentials:
                message = "This username is already taken.";
                break;
            case AuthError.MissingPassword:
                message = "Please enter a password.";
                break;
            case AuthError.WeakPassword:
                message = "The password is too weak (min. 6 character needed).";
                break;
            case AuthError.WrongPassword:
                message = "Wrong password.";
                break;
            case AuthError.EmailAlreadyInUse:
                message = "This username is already taken.";
                break;
            case AuthError.InvalidEmail:
                message = "Invalid username.";
                break;
            case AuthError.MissingEmail:
                message = "Username is missing";
                break;
            case AuthError.TooManyRequests:
                message = "Too many requests";
                break;
            case AuthError.Cancelled:
                message = "Request cancelled.";
                break;
            case AuthError.UserNotFound:
                message = "Username does not exists.";
                break;
            case AuthError.QuotaExceeded:
                message = "Served quota reached for this month. \n Need more money for poor developer to buy better server. :(";
                break;
            default:
                message = "Unhandled error (" + (AuthError)ex.ErrorCode + ")";
                break;
        }
        return message;
    }
}
