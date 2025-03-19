namespace courses.Services;

public class BlackTokensService
{
    private readonly IBlackTokensRepository _blackTokensRepository;

    public BlackTokensService(
        IBlackTokensRepository blackTokensRepository)
    {
        _blackTokensRepository = blackTokensRepository;
    }
    
    public async Task Add(string token)
    {
        await _blackTokensRepository.Add(token);
    }
    
    public async Task<bool> CheckToken(string token)
    {
        return await _blackTokensRepository.Find(token) is not null;
    }
}