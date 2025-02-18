namespace Snowball_Legacy.Server.Models.ViewModels;

public class GameViewModel
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public int? DiskNumber { get; set; }
    byte[]? TitlePicture { get; set; }
    List<byte[]>? ScreenShoots { get; set; }
    List<byte[]>? AdditionalFiles { get; set; }
}
