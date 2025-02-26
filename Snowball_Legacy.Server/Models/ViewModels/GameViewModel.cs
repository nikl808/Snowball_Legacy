namespace Snowball_Legacy.Server.Models.ViewModels;

public class GameViewModel
{
    public string Name { get; set; } = string.Empty;
    public string Developer { get; set; } = string.Empty;
    public string Genre { get; set; } = string.Empty;
    public string ReleaseDate { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int DiscNumber { get; set; }
    public bool IsAdditionalFiles { get; set; }
    public List<IFormFile>? TitlePicture { get; set; }
    public List<IFormFile>? Screenshots { get; set; }
    public List<IFormFile>? AdditionalFiles { get; set; }
}
