using Microsoft.AspNetCore.Identity;
using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteEFDataProvider.Data;
using UniversiteEFDataProvider.Entities;
using UniversiteEFDataProvider.Repositories;

namespace UniversiteEFDataProvider.RepositoryFactories;

public class RepositoryFactory (UniversiteDbContext context, UserManager<UniversiteUser> userManager, RoleManager<UniversiteRole> roleManager)
    : IRepositoryFactory
{
    private IParcoursRepository? _parcours;
    private IEtudiantRepository? _etudiants;
    private IUeRepository? _ues;
    private INotesRepository? _notes;
    private IUniversiteRoleRepository? _roles;
    private IUniversiteUserRepository? _users;
    
    public IParcoursRepository ParcoursRepository()
    {
        if (_parcours == null)
        {
            _parcours = new ParcoursRepository(context ?? throw new InvalidOperationException());
        }
        return _parcours;
    }

    public IEtudiantRepository EtudiantRepository()
    {
        if (_etudiants == null)
        {
            _etudiants = new EtudiantRepository(context ?? throw new InvalidOperationException());
        }
        return _etudiants;
    }

    public IUeRepository UeRepository()
    {
        if (_ues == null)
        {
            _ues = new UeRepository(context ?? throw new InvalidOperationException());
        }
        return _ues;
    }

    public INotesRepository NotesRepository()
    {
        if (_notes == null)
        {
            _notes = new NotesRepository(context ?? throw new InvalidOperationException());
        }
        return _notes;

    }
    
    public IUniversiteRoleRepository UniversiteRoleRepository()
    {
        if (_roles == null)
        { 
            _roles = new UniversiteRoleRepository(context ?? throw new InvalidOperationException(), roleManager);
        }
        return _roles;
    }
    
    public IUniversiteUserRepository UniversiteUserRepository()
    {
        if (_users == null)
        { 
            _users = new UniversiteUserRepository(context ?? throw new InvalidOperationException(), userManager, roleManager);
        }
        return _users;
    }
       
    public async Task SaveChangesAsync()
    {
        context.SaveChangesAsync().Wait();
    }
    public async Task EnsureCreatedAsync()
    {
        context.Database.EnsureCreated();
    }
    public async Task EnsureDeletedAsync()
    {
        context.Database.EnsureDeleted();
    }
}