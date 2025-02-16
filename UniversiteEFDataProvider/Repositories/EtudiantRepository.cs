using Microsoft.EntityFrameworkCore;
using UniversiteDomain.DataAdapters;
using UniversiteDomain.Entities;
using UniversiteEFDataProvider.Data;

namespace UniversiteEFDataProvider.Repositories;

public class EtudiantRepository(UniversiteDbContext context) : Repository<Etudiant>(context), IEtudiantRepository
{
    
    public async Task AffecterParcoursAsync(long idEtudiant, long idParcours)
    {
        ArgumentNullException.ThrowIfNull(Context.Etudiants);
        ArgumentNullException.ThrowIfNull(Context.Parcours);
        Etudiant e = (await Context.Etudiants.FindAsync(idEtudiant))!;
        Parcours p = (await Context.Parcours.FindAsync(idParcours))!;
        e.ParcoursSuivi = p;
        await Context.SaveChangesAsync();
    }
    
    public async Task AffecterParcoursAsync(Etudiant etudiant, Parcours parcours)
    {
        await AffecterParcoursAsync(etudiant.Id, parcours.Id); 
    }

    /* ------ Normal Version ------*/
    public async Task<Etudiant> AddNoteAsync(long idEtudiant, long idNote)
    {
        ArgumentNullException.ThrowIfNull(Context.Etudiants);
        ArgumentNullException.ThrowIfNull(Context.Notes);
        Etudiant e = (await Context.Etudiants.FindAsync(idEtudiant))!;
        Notes n = (await Context.Notes.FindAsync(idNote))!;
        e.NotesObtenues.Add(n);
        await Context.SaveChangesAsync();
        return e;
    }
    
    public async Task<Etudiant> AddNoteAsync(Etudiant etudiant, Notes note)
    {
        return await AddNoteAsync(etudiant.Id, note.Id);
    }
    
    /* ------ Lists Version ------*/

    public async Task<Etudiant> AddNoteAsync(long idEtudiant, long[] idNotes)
    {
        ArgumentNullException.ThrowIfNull(Context.Etudiants);
        ArgumentNullException.ThrowIfNull(Context.Notes);

        Etudiant e = (await Context.Etudiants.FindAsync(idEtudiant))!;
        List<Notes> notes = new List<Notes>();

        foreach (var id in idNotes)
        {
            Notes note = (await Context.Notes.FindAsync(id))!;
            notes.Add(note);
        }

        e.NotesObtenues.AddRange(notes);
        await Context.SaveChangesAsync();
        return e;
    }
    
    public async Task<Etudiant> AddNoteAsync(Etudiant? etudiant, List<Notes> notes)
    {
        long[] noteIds = notes.Select(n => n.Id).ToArray();
        return await AddNoteAsync(etudiant.Id, noteIds);
    }
    
    public async Task<Etudiant?> FindEtudiantCompletAsync(long idEtudiant)
    {
        ArgumentNullException.ThrowIfNull(Context.Etudiants);
        return await Context.Etudiants.Include(e => e.NotesObtenues).ThenInclude(n=>n.Ue).Include(p => p.ParcoursSuivi).FirstOrDefaultAsync(e => e.Id == idEtudiant);
    }

    public async Task<List<Etudiant>> FindEtudiantsByNumUeAsync(string numUe)
    {
        ArgumentNullException.ThrowIfNull(Context.Etudiants);
        return await Context.Etudiants.Include(e => e.ParcoursSuivi)
            .ThenInclude(p => p!.UesEnseignees)
            .Include(e => e.NotesObtenues)
            .ThenInclude(n => n.Ue)
            .Where(e => e.ParcoursSuivi.UesEnseignees.Any(n => n.NumeroUe == numUe))
            .ToListAsync();
    }
}