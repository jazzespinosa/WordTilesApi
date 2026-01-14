//namespace WordTilesApi.Models.Entities
//{
//  public class Feedback
//  {
//    public long FeedbackId { get; set; } = GenerateFeedbackId();
//    public DateTime FeedbackDate { get; set; } = DateTime.UtcNow;
//    public string User { get; set; } = string.Empty;
//    public string Category { get; set; } = string.Empty;
//    public string Subject { get; set; } = string.Empty;
//    public string Description { get; set; } = string.Empty;
//    public bool IsAnonymous { get; set; }
//    public bool AllowContact { get; set; }


//    private static int _sequence = 0;
//    private static readonly object _lock = new object();
//    public static long GenerateFeedbackId()
//    {
//      lock (_lock)
//      {
//        _sequence = (_sequence + 1) % 1000; // keep within 3 digits
//        return long.Parse(
//            DateTime.UtcNow.ToString("yyyyMMddHHmmss") +
//            _sequence.ToString("D3")
//        );
//      }
//    }
//  }
//}
