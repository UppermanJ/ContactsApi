using Models;

namespace DataAccess.Repositories.Interfaces
{
    public interface IContactRepository
    {
        Contact Create(Contact contact);
        Contact GetSingle(int id);
    }
}
