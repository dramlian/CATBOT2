public record Payload(string url, bool is_reusable);

public record Attachment(string type, Payload payload)
{
    public Attachment(string inputUrl) : this("image", new Payload(inputUrl, false)) { }
}

public record Message(Attachment attachment)
{
    public Message(string inputUrl) : this(new Attachment(inputUrl)) { }
}

public record Recipient(string id);

public record SendImageDto(Recipient recipient, Message message, string access_token)
{
    public SendImageDto(string recipientId, string catsUrl, string pageAccessToken)
        : this(new Recipient(recipientId), new Message(catsUrl), pageAccessToken) { }
}
