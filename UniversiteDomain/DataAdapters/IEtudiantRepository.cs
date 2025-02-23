using System.Collections;
using UniversiteDomain.Entities;
namespace UniversiteDomain.DataAdapters;

public interface IEtudiantRepository : IRepository<Etudiant>
{
    Task<Etudiant> AddNoteAsync(long idEtudiant, long idNote);
    Task<Etudiant> AddNoteAsync(Etudiant etudiant, Notes note);
    Task<Etudiant> AddNoteAsync(Etudiant? etudiant, List<Notes> notes);
    Task<Etudiant> AddNoteAsync(long IdEtudiant, long[] IdNotes);
    public Task<Etudiant?> FindEtudiantCompletAsync(long idEtudiant);
    public Task<List<Etudiant>> FindEtudiantsByNumUeAsync(string numUe);
    
}