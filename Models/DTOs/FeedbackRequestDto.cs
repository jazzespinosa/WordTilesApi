namespace WordTilesApi.Models.DTOs
{
  public class FeedbackRequestDto
  {
    public string Category { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsAnonymous { get; set; }
    public bool AllowContact { get; set; }
  }
}
