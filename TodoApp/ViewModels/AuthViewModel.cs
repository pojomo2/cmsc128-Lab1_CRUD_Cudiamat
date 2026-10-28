using System;
using System.Linq;
using System.IO;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace TodoApp.ViewModels;
 
public enum AuthState
{
    Login,
    Register,
    ResetPassword,
    Profile
}


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
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsLoginVisible))]
    [NotifyPropertyChangedFor(nameof(IsResetPasswordVisible))]
    [NotifyPropertyChangedFor(nameof(IsRegisterVisible))]
    [NotifyPropertyChangedFor(nameof(IsProfileVisible))]
    
    private AuthState _currentState = AuthState.Login;

    public bool IsLoginVisible => CurrentState == AuthState.Login;
    public bool IsResetPasswordVisible => CurrentState == AuthState.ResetPassword;
    public bool IsRegisterVisible => CurrentState == AuthState.Register;
    public bool IsProfileVisible => CurrentState == AuthState.Profile;


    private readonly TodoDbContext _db = new();
    private readonly string SessionFilePath = AppPaths.SessionPath;

    [ObservableProperty] private string regEmail = "";
    [ObservableProperty] private string regDisplayName = "";
    [ObservableProperty] private string regPassword = "";
    [ObservableProperty] private string registerError = "";

    [ObservableProperty] private string loginEmail = "";
    [ObservableProperty] private string loginPassword = "";
    [ObservableProperty] private string loginError = "";

    [ObservableProperty] private User? currentUser;

    [RelayCommand]
    public void ChangeState(AuthState newState)
    {
        CurrentState = newState;
    }


    public AuthViewModel()
    {
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

        var email = NormalizeEmail(RegEmail);

        if(_db.Users.Any(u => u.Email == email))
        {
            RegisterError = "An account with this email already exists.";
            return;
        }

        var user = new User
        {
            Email = email,
            DisplayName = RegDisplayName,
            PasswordHash = PasswordHasher.Hash(RegPassword)
        };

        _db.Users.Add(user);
        _db.SaveChanges();
        RegisterError = "";
        //navigate to login, or auto-login here
        LoginEmail = email;
        LoginPassword = "";
        CurrentState = AuthState.Login;
    }

   internal static string NormalizeEmail(string email) => email.Trim().ToLowerInvariant();

    [RelayCommand]
    private void Login()
    {
        var email = NormalizeEmail(LoginEmail);
        var user = _db.Users.FirstOrDefault(u => u.Email == email);

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

        NewEmail = user.Email;
        NewDisplayName = user.DisplayName;
        NewPassword = "";
        CurrentState = AuthState.Profile;



    }


    private void TryRestoreSession()
    {
        if(!File.Exists(SessionFilePath)) return;

        var token = File.ReadAllText(SessionFilePath);
        var session = _db.Sessions.FirstOrDefault(s => s.Token == token && s.ExpiresAt > DateTime.Now);

        if (session != null)
        {
            var user = _db.Users.FirstOrDefault(u => u.Id == session.UserId);
            if (user != null)
            {
                CurrentUser = user;
                NewEmail = user.Email;
                NewDisplayName = user.DisplayName;
                NewPassword = "";
                CurrentState = AuthState.Profile; 
            }
            
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
        CurrentState = AuthState.Login;
        LoginPassword = "";
    }

    [RelayCommand] 
    private async Task RequestPasswordReset()
    {
        var email = NormalizeEmail(ResetEmail);
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == email);
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
        if (string.IsNullOrWhiteSpace(NewEmail))
{
            ProfileError = "Email is required.";
            return;
        }

        var email = NormalizeEmail(NewEmail);

        if(_db.Users.Any(u => u.Email == email && u.Id != CurrentUser.Id))
        {
            ProfileError = "That email is already in use.";
            return;
        }

        CurrentUser.Email = email;
        CurrentUser.DisplayName = NewDisplayName;
        if (!string.IsNullOrWhiteSpace(NewPassword))
        {
            CurrentUser.PasswordHash = PasswordHasher.Hash(NewPassword);
        }

        _db.SaveChanges();
        NewEmail = email;
        ProfileStatus = "Profile updated.";
    }

}


