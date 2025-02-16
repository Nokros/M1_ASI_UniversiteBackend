using Microsoft.AspNetCore.Identity;
using UniversiteDomain.Entities;

namespace UniversiteEFDataProvider.Entities;

public class UniversiteRole: IdentityRole, IUniversiteRole
{
    public UniversiteRole()
    {
    }
    public UniversiteRole(string role) : base(role)
    {
    }}