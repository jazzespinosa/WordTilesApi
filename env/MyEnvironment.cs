namespace WordTilesApi.env
{
  public class MyEnvironment
  {
    private readonly IConfiguration _config;

    public MyEnvironment(IConfiguration config)
    {
      _config = config;
    }

    public string FeedbackEmailRecipient => _config["FeedbackEmailRecipient"];

    public string CloudFirestoreDbLink => _config["CloudFirestoreDbLink"];
  }
}
