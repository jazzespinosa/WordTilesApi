using WordTilesApi.Models.DTOs;

namespace WordTilesApi.Services.Interfaces
{
  public interface IUtilService
  {
    Task<FeedbackResponseDto> TriggerEmailSend(FeedbackRequestDto feedbackRequestDto, Guid playerId);
  }
}
