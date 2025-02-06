using UniversiteDomain.Entities;

namespace UniversiteDomain.DataAdapters;

public interface IUeRepository : IRepository<Ue>
{
   
    public Task<Ue?> FindUeCompletAsync(long idUe);
}