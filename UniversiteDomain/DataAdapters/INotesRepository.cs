using UniversiteDomain.Entities;

namespace UniversiteDomain.DataAdapters;

public interface INotesRepository : IRepository<Notes>
{
    public Task<Notes?> FindNotesCompletAsync(long idNote);
}