using FamilyTreeApp.Server.Infrastructure.Interfaces;
using FamilyTreeApp.Server.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace FamilyTreeApp.Server.Infrastructure.Repositories
{
    public class FamilyTreeRepository : IFamilyTreeRepository
    {
        private readonly AppDbContext _context;
        public FamilyTreeRepository(AppDbContext context)
        {
            _context = context;
        }
        /// <inheritdoc />
        public async Task<IEnumerable<PersonRelationship>> GetPersonRelationshipListAsync()
        {
            return await _context.PersonRelationships.ToListAsync();
        }
    }
}
