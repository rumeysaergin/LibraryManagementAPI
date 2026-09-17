using LibraryManagementAPI.Application.Repositories;
using LibraryManagementAPI.Domain.Entities;
using LibraryManagementAPI.Infrastructure.Data;

namespace LibraryManagementAPI.Infrastructure.Repositories
{
    public class FineRepository : IFineRepository
    {
        private readonly LibraryManagementDbContext _context;

        public FineRepository(LibraryManagementDbContext context)
        {
            _context = context;
        }

        public List<Fine> GetAll()
        {
            return _context.Fines
                .Where(x => !x.IsDeleted)
                .ToList();
        }

        public Fine? GetById(Guid id)
        {
            return _context.Fines
                .FirstOrDefault(x => x.Id == id && !x.IsDeleted);
        }

        public List<Fine> GetByLoanId(Guid loanId)
        {
            return _context.Fines
                .Where(x => x.LoanId == loanId && !x.IsDeleted)
                .ToList();
        }

        public void Add(Fine fine)
        {
            _context.Fines.Add(fine);
            _context.SaveChanges();
        }

        public void Update(Fine fine)
        {
            _context.Fines.Update(fine);
            _context.SaveChanges();
        }

        public void Delete(Fine fine)
        {
            fine.IsDeleted = true;
            _context.Fines.Update(fine);
            _context.SaveChanges();
        }
    }
}