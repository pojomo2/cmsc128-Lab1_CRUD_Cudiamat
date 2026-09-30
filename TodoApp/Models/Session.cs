using System;
using System.IO;
using Avalonia.Metadata;
using CommunityToolkit.Mvvm.Input;
using Org.BouncyCastle.Asn1.Misc;

public class Session
{
    public int Id { get; set; }
    public int UserId { get; set; }

    public string Token { get; set; } = "";
    public DateTime ExpiresAt { get; set; }
    public int UserID { get; set; }

}

