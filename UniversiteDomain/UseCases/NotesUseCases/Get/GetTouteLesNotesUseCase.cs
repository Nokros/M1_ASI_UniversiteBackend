using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;

namespace UniversiteDomain.UseCases.NotesUseCases.Get;

public class GetToutesLesNotesUseCase
{
    private readonly IRepositoryFactory _factory;
    
    public GetToutesLesNotesUseCase(IRepositoryFactory factory)
    {
        _factory = factory;
    }
    
    public async Task<List<Notes>> ExecuteAsync()
    {
        await CheckBusinessRules();
        List<Notes> notes = await _factory.NotesRepository().FindAllAsync();
        return notes;
    }
    
    private async Task CheckBusinessRules()
    {
        ArgumentNullException.ThrowIfNull(_factory);
        var notesRepo = _factory.NotesRepository();
        ArgumentNullException.ThrowIfNull(notesRepo);
    }
    
    public bool IsAuthorized(string role)
    {
        return role.Equals(Roles.Responsable) || role.Equals(Roles.Scolarite);
    }
}