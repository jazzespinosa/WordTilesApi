using Google.Cloud.Firestore;
using Microsoft.EntityFrameworkCore;
using WordTilesApi.Data;
using WordTilesApi.env;
using WordTilesApi.Models.DTOs;
using WordTilesApi.Services.Interfaces;

namespace WordTilesApi.Services.Implementations
{
  public class UtilService : IUtilService
  {
    private readonly GameContext _gameContext;
    private readonly MyEnvironment _env;

    public UtilService(GameContext gameContext, MyEnvironment env)
    {
      _gameContext = gameContext;
      _env = env;
    }

    public async Task<FeedbackResponseDto> TriggerEmailSend(FeedbackRequestDto feedbackRequestDto, Guid playerId)
    {
      var feedbackEmailRecipient = _env.FeedbackEmailRecipient;
      var cloudFirestoreDbLink = _env.CloudFirestoreDbLink;
      FirestoreDb db = FirestoreDb.Create("wordtiles1");
      var feedbackTimeStamp = DateTime.UtcNow;
      var category = feedbackRequestDto.Category;
      var subject = feedbackRequestDto.Subject;
      var description = feedbackRequestDto.Description;
      var allowContact = feedbackRequestDto.AllowContact;
      var isAnonymous = feedbackRequestDto.IsAnonymous;
      var user = "Anonymous";
      if (!isAnonymous)
      {
        var userDetails = await _gameContext.UserData.Where(u => u.PlayerId == playerId).FirstOrDefaultAsync();
        user = $"{userDetails.Name} ({userDetails.Email})";
      }

      var response = new FeedbackResponseDto();

      var feedbackData = new Dictionary<string, object>
      {
        { "FeedbackId", response.FeedbackId },
        { "FeedbackTimeStamp", feedbackTimeStamp },
        { "User", user },
        { "Category", category },
        { "Subject", subject },
        { "Description", description },
        { "IsAnonymous", isAnonymous },
        { "AllowContact", allowContact },
      };
      CollectionReference feedbackCollection = db.Collection("feedback");
      DocumentReference feedbackDocRef = feedbackCollection.Document(response.FeedbackId.ToString());
      await feedbackDocRef.SetAsync(feedbackData);

      var emailBody = $"<p>Hi,</p>\n<p>&nbsp;</p>\n<p>You have received new feedback. See details below.</p>\n<p>&nbsp;</p>\n<p><strong>FeedbackId</strong> = {response.FeedbackId}</p>\n<p><strong>FeedbackTimeStamp</strong> = {feedbackTimeStamp}</p>\n<p><strong>IsAnonymous</strong> = {isAnonymous}</p>\n<p><strong>User</strong> = {user}</p>\n<p><strong>Category</strong> = {category}</p>\n<p><strong>Subject</strong> = {subject}</p>\n<p><strong>Description</strong> = {description}&nbsp;</p>\n<p><strong>AllowContact</strong> = {allowContact}</p>\n<p>&nbsp;</p>\n<p>For further information, check your Realtime Database via this&nbsp;<a href=\"{cloudFirestoreDbLink}\" target=\"_blank\">link</a>.</p>";

      var emailData = new Dictionary<string, object>
        {
            { "to", feedbackEmailRecipient },
            { "message", new Dictionary<string, object>
                {
                    { "subject", "New Feedback" },
                    { "html", emailBody }
                }
            }
        };

      CollectionReference emailCollection = db.Collection("mail");
      DocumentReference emailDocRef = emailCollection.Document(response.FeedbackId.ToString());
      await emailDocRef.SetAsync(emailData);

      return response;
    }
  }
}
