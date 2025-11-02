
public class CatImageSender : ICatImageSender
{
    public async Task SendCatImage(string PAT, string recipientId)
    {
        IHttpService apiService = new HttpService();
        IMetaService metaService = new MetaService(apiService);
        ICatService catService = new CatService(apiService);

        var catUrl = await catService.GetCatImagePayload();
        await metaService.SendImagePayloadMessage(recipientId, catUrl.Url, PAT);
    }
}