namespace LibrarySystemApi.Models;

public class AccountPayload
{
    public string OldName { get; set; } = null!;

    public string? NewName { get; set; }
    
    public string OldPassword { get; set; } = null!;

    public string? NewPassword { get; set; }
}