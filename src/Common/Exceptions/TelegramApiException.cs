namespace Common.Exceptions;

public class TelegramApiException : Exception
{
    public TelegramErrorResponse Response { get; }

    public TelegramApiException(TelegramErrorResponse response) : base(response.Description)
    {
        Response = response;
    }
}

public class TelegramErrorResponse
{
    public int ErrorCode { get; set; }
    public string Description { get; set; } = string.Empty;
}
