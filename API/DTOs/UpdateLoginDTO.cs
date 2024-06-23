namespace API.DTOs;

public class UpdateLoginDTO
{
    public required string Username { get; set; }

    public required string Password { get; set; }

    public required string UpdateUsername { get; set; }

    public required string UpdatePassword { get; set; }
}
