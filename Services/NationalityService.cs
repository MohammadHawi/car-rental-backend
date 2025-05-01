public class NationalityService : INationalityService
{
    private readonly INationalityRepository _repository;

    public NationalityService(INationalityRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<Nationality>> GetAllNationalitiesAsync()
    {
        return await _repository.GetAllAsync();
    }
}
