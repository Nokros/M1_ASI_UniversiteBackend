using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;

namespace UniversiteDomain.UseCases.NotesUseCases.Get;

public class GetNoteCompletUseCase
{
    private readonly IRepositoryFactory _factory;
    
    public GetNoteCompletUseCase(IRepositoryFactory factory)
    {
        _factory = factory;
    }
    
    public async Task<Notes?> ExecuteAsync(long idNote)
    {
        await CheckBusinessRules();
        // Méthode supposée renvoyer une note avec ses associations (Etudiant et Ue)
        Notes? note = await _factory.NotesRepository().FindNotesCompletAsync(idNote);
        return note;
    }
    
    private async Task CheckBusinessRules()
    {
        ArgumentNullException.ThrowIfNull(_factory);
        var notesRepo = _factory.NotesRepository();
        ArgumentNullException.ThrowIfNull(notesRepo);
    }
    
    public async Task<bool> IsAuthorizedAsync(string role, IUniversiteUser user, long idNote)
    {
        if (role.Equals(Roles.Scolarite) || role.Equals(Roles.Responsable))
            return true;
        
        if (role.Equals(Roles.Etudiant) && user.Etudiant != null)
        {
            var note = await _factory.NotesRepository().FindAsync(idNote);
            return note != null && note.EtudiantId == user.Etudiant.Id;
        }
        
        return false;
    }

    public bool IsAuthorized(string role, IUniversiteUser user, long id)
    {
        throw new NotImplementedException();
    }
}