namespace LibrarySystemApi.Models;

public class AccountPayload
{
    public string OldUsername { get; set; } = null!;

    public string? NewUsername { get; set; }
    
    public string OldPassword { get; set; } = null!;

    public string? NewPassword { get; set; }
}