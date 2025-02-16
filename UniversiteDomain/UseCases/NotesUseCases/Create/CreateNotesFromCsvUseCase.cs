using System.Globalization;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Dtos;
using UniversiteDomain.Entities;
using UniversiteDomain.Exceptions.CsvExceptions;
using UniversiteDomain.Exceptions.EtudiantExceptions;
using UniversiteDomain.Exceptions.UeExceptions;

namespace UniversiteDomain.UseCases.NotesUseCases.Create;

public class CreateNotesFromCsvUseCase(IRepositoryFactory factory)
{
    public async Task ExecuteAsync(string numeroUe, byte[] csvFile)
    {
        var ue = await factory.UeRepository().FindByConditionAsync(e => e.NumeroUe == numeroUe);
        if (ue is { Count: 0 }) throw new UeNotFoundException(numeroUe);
        
        using var stream = new MemoryStream(csvFile);
        using var reader = new StreamReader(stream, Encoding.UTF8);
        using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture));

        var records = csv.GetRecords<CsvNotes>().ToList();
        if (records == null || !records.Any()) throw new CsvNullException("Le fichier CSV est vide ou invalide.");

        var etudiantNums = records.Select(r => r.NumEtud).Distinct().ToList();
        var etudiants = (await factory.EtudiantRepository().FindByConditionAsync(e => etudiantNums.Contains(e.NumEtud)))
                        .ToDictionary(e => e.NumEtud, e => e);

        if (etudiants.Count != etudiantNums.Count)
        {
            throw new EtudiantNotFoundException("Certains étudiants du fichier CSV sont introuvables.");
        }

        var existingNotes = (await factory.NotesRepository()
            .FindByConditionAsync(n => etudiantNums.Contains(n.Etudiant.NumEtud) && n.Ue.NumeroUe == numeroUe))
            .ToDictionary(n => (n.EtudiantId, n.UeId));

        foreach (var record in records)
        {
            var etudiant = etudiants[record.NumEtud];
            var key = (etudiant.Id, ue[0].Id);

            if (existingNotes.TryGetValue(key, out var existingNote))
            {
                if (!record.Note.HasValue)
                {
                    await factory.NotesRepository().DeleteAsync(existingNote);
                    Console.WriteLine($"Note supprimée pour l'étudiant {record.NumEtud}.");
                }
                else if (existingNote.Valeur != record.Note.Value)
                {
                    existingNote.Valeur = record.Note.Value;
                    await factory.NotesRepository().UpdateAsync(existingNote);
                    Console.WriteLine($"Note mise à jour pour l'étudiant {record.NumEtud}.");
                }
                continue;
            }

            if (record.Note.HasValue)
            {
                var nouvelleNote = new Notes
                {
                    Valeur = record.Note.Value,
                    EtudiantId = etudiant.Id,
                    UeId = ue[0].Id
                };
                await factory.NotesRepository().CreateAsync(nouvelleNote);
                Console.WriteLine($"Note créée pour l'étudiant {record.NumEtud}.");
            }
        }
    }

    public bool IsAuthorized(string role) => role.Equals(Roles.Scolarite);
}
