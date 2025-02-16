using UniversiteDomain.Entities;

namespace UniversiteDomain.Dtos;

public class NoteCompletDto
{
    public long Id { get; set; }
    public float Valeur { get; set; }
    public EtudiantDto Etudiant { get; set; }
    public UeDto Ue { get; set; }
    
    public NoteCompletDto ToDto(Notes note)
    {
        Id = note.Id;
        Valeur = note.Valeur;
        // On suppose que note.Etudiant et note.Ue sont chargés
        Etudiant = new EtudiantDto().ToDto(note.Etudiant);
        Ue = new UeDto().ToDto(note.Ue);
        return this;
    }
    
    public Notes ToEntity()
    {
        return new Notes
        {
            Id = this.Id,
            Valeur = this.Valeur,
            EtudiantId = Etudiant.Id,
            UeId = Ue.Id
        };
    }
}