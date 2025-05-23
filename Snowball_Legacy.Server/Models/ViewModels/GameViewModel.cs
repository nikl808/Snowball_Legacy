namespace Snowball_Legacy.Server.Models.ViewModels;

public class GameViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Origin { get; set; } = string.Empty;
    public string Developer { get; set; } = string.Empty;
    public string Genre { get; set; } = string.Empty;
    public string ReleaseDate { get; set; } = string.Empty;
    public string? FromSeries { get; set; }
    public string Description { get; set; } = string.Empty;
    public int DiscNumber { get; set; }
    public int IsAdditionalFiles { get; set; }
    public List<IFormFile>? TitlePicture { get; set; }
    public List<IFormFile>? Screenshots { get; set; }
    public List<IFormFile>? AdditionalFiles { get; set; }
}
