using System;
using System.Linq;
using System.IO;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
 

namespace TodoApp.ViewModels;

public partial class AuthViewModel : ViewModelBase
{
        // password reset
    [ObservableProperty] private string resetEmail = "";
    [ObservableProperty] private string resetStatus = "";
    [ObservableProperty] private string resetError = "";
    [ObservableProperty] private string submittedToken = "";
    [ObservableProperty] private string newPassword = "";

    // profile
    [ObservableProperty] private string newEmail = "";
    [ObservableProperty] private string newDisplayName = "";
    [ObservableProperty] private string profileError = "";
    [ObservableProperty] private string profileStatus = "";

    private readonly TodoDbContext _db = new();
    private readonly string SessionFilePath = Path.Combine(AppContext.BaseDirectory, "session.txt");

    [ObservableProperty] private string regEmail = "";
    [ObservableProperty] private string regDisplayName = "";
    [ObservableProperty] private string regPassword = "";
    [ObservableProperty] private string registerError = "";

    [ObservableProperty] private string loginEmail = "";
    [ObservableProperty] private string loginPassword = "";
    [ObservableProperty] private string loginError = "";

    [ObservableProperty] private User? currentUser;


    public AuthViewModel()
    {
        _db.Database.EnsureCreated();
        TryRestoreSession();
    }


    [RelayCommand]

    private void Register()
    {
        if(string.IsNullOrWhiteSpace(RegEmail) || string.IsNullOrWhiteSpace(RegPassword))
        {
            RegisterError = "Email and password are required.";
            return;
        }

        if(_db.Users.Any(u => u.Email == RegEmail))
        {
            RegisterError = "An account with this email already exists.";
            return;
        }

        var user = new User
        {
            Email = RegEmail,
            DisplayName = RegDisplayName,
            PasswordHash = PasswordHasher.Hash(RegPassword)
        };

        _db.Users.Add(user);
        _db.SaveChanges();
        RegisterError = "";
        //navigate to login, or auto-login here
    }

    [RelayCommand]
    private void Login()
    {
        var user = _db.Users.FirstOrDefault(u => u.Email == LoginEmail);

        if (user == null || !PasswordHasher.Verify(LoginPassword, user.PasswordHash))
        {
            LoginError = "Invalid email or password.";
            return;
        }

        var token = Guid.NewGuid().ToString(); //This generates the session token — a random, unpredictable string that acts like a temporary "key" proving the user is logged in.
        var session = new Session
        {
            UserId = user.Id,
            Token = token,
            ExpiresAt = DateTime.Now.AddDays(7)
        };
        _db.Sessions.Add(session);
        _db.SaveChanges();

        File.WriteAllText(SessionFilePath, token); //persist to disk
        CurrentUser = user;
        //naviaget to the logged-in / profile view

    }


    private void TryRestoreSession()
    {
        if(!File.Exists(SessionFilePath)) return;

        var token = File.ReadAllText(SessionFilePath);
        var session = _db.Sessions.FirstOrDefault(s => s.Token == token && s.ExpiresAt > DateTime.Now);

        if (session != null)
        {
            CurrentUser = _db.Users.FirstOrDefault(u => u.Id == session.UserId);
        } 
        else
        {
            File.Delete(SessionFilePath); //stale/expire token, clean it up
        }
            
    }

    [RelayCommand]
    private void Logout()
    {
        var token = File.Exists(SessionFilePath) ? File.ReadAllText(SessionFilePath) : null;
        if (token != null)
        {
            var session = _db.Sessions.FirstOrDefault(s => s.Token == token);
            if (session != null) { _db.Sessions.Remove(session); _db.SaveChanges(); }
            File.Delete(SessionFilePath);
        }
        CurrentUser = null;
        //navigate back to login
    }

    [RelayCommand] 
    private void RequestPasswordReset()
    {
        var user = _db.Users.FirstOrDefault(u => u.Email == ResetEmail);
        if (user == null) { ResetStatus = "If that email exists, a reset link was sent."; return;}

        user.ResetToken = Guid.NewGuid().ToString();
        user.ResetTokenExpiry = DateTime.Now.AddMinutes(30);
        _db.SaveChanges();

        EmailService.SendResetEmail(user.Email, user.ResetToken);
        ResetStatus = "If that email exists, a reset link was sent.";

    }

    [RelayCommand]
    private void CompletePasswordReset()
    {
        var user = _db.Users.FirstOrDefault(u=> u.ResetToken == SubmittedToken && u.ResetTokenExpiry > DateTime.Now);

        if (user == null)
        {
            ResetError = "Invalid or expired reset code.";
            return;
        }

        user.PasswordHash = PasswordHasher.Hash(NewPassword);
        user.ResetToken = null;
        user.ResetTokenExpiry = null;
        _db.SaveChanges();

        ResetStatus = "Password updated. You can now log in.";
    }

    [RelayCommand]
    private void UpdateProfile()
    {
        if(CurrentUser == null) return;
        if(_db.Users.Any(u => u.Email == NewEmail && u.Id != CurrentUser.Id))
        {
            ProfileError = "That email is already in use.";
            return;
        }

        CurrentUser.Email = NewEmail;
        CurrentUser.DisplayName = NewDisplayName;
        if (!string.IsNullOrWhiteSpace(NewPassword))
        {
            CurrentUser.PasswordHash = PasswordHasher.Hash(NewPassword);
        }

        _db.SaveChanges();
        ProfileStatus = "Profile updated.";
    }

}


