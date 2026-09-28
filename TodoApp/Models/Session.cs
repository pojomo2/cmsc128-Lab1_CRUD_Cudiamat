using System;
using System.IO;
using Avalonia.Metadata;
using CommunityToolkit.Mvvm.Input;

public class Session
{
    public int Id { get; set; }
    public int UserId { get; set; }

    [RelayCommand] 
    private void Login()
    {
        var user = _db.Users.FirstOrDefault(u => u.Email == LoginEmail);

        if(user == null || !PasswordHasher.Verify(LoginPassword, user.PasswordHash))
        {
            LoginError = "Invalid email or password.";

            return;
        }

        var token = Guid.NewGuid().ToString();
        var session = new Session
        {
            UserId = user.Id;
            Token = token,
            ExpiresAt = DateTime.Now.AddDays(7)
        };
        _db.Sessions.Add(session);
        _db.SaveChanges();

        File.WriteAllText(SessionFilePath, token); //persist to disk
        CurrentUser = user;
        //navigate to the logged-in / profile view

    } public string Token { get; set; } = "";
    public DateTime ExpiresAt { get; set; }    
}

