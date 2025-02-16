using UniversiteDomain.Entities;

namespace UniversiteDomain.Dtos;

public class NoteDto
{
    public long Id { get; set; }
    public float Valeur { get; set; }
    public long EtudiantId { get; set; }
    public long UeId { get; set; }
    
    public NoteDto ToDto(Notes note)
    {
        Id = note.Id;
        Valeur = note.Valeur;
        EtudiantId = note.EtudiantId;
        UeId = note.UeId;
        return this;
    }
    
    public static List<NoteDto> ToDtos(List<Notes> notes)
    {
        List<NoteDto> dtos = new();
        foreach (var note in notes)
        {
            dtos.Add(new NoteDto().ToDto(note));
        }
        return dtos;
    }
    
    public Notes ToEntity()
    {
        return new Notes
        {
            Id = this.Id,
            Valeur = this.Valeur,
            EtudiantId = this.EtudiantId,
            UeId = this.UeId
        };
    }
}