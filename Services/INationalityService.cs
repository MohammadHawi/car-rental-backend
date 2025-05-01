public interface INationalityService
{
    Task<IEnumerable<Nationality>> GetAllNationalitiesAsync();
}
