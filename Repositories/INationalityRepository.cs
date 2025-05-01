public interface INationalityRepository
{
    Task<IEnumerable<Nationality>> GetAllAsync();
}
