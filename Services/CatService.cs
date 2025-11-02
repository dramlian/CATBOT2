
public class CatService : ICatService
{
    private IHttpService _httpService;
    private string _catApiUrl = "https://api.thecatapi.com/v1/images/search";
    public CatService(IHttpService httpService)
    {
        _httpService = httpService;
    }
    public async Task<CatResponseDto> GetCatImagePayload()
    {
        return (await _httpService.GetRequest<List<CatResponseDto>>(_catApiUrl) ?? throw new Exception("Cat api error")).FirstOrDefault()!;

    }
}