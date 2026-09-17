using LibraryManagementAPI.Application.Repositories;
using LibraryManagementAPI.Domain.Entities;
using LibraryManagementAPI.Infrastructure.Data;

namespace LibraryManagementAPI.Infrastructure.Repositories
{
    public class LoanRepository : ILoanRepository
    {
        private readonly LibraryManagementDbContext _context;

        public LoanRepository(LibraryManagementDbContext context)
        {
            _context = context;
        }

        public List<Loan> GetAll()
        {
            return _context.Loans
                .Where(x => !x.IsDeleted)
                .ToList();
        }

        public Loan? GetById(Guid id)
        {
            return _context.Loans
                .FirstOrDefault(x => x.Id == id && !x.IsDeleted);
        }

        public List<Loan> GetByUserId(Guid userId)
        {
            return _context.Loans
                .Where(x => x.UserId == userId && !x.IsDeleted)
                .ToList();
        }

        public List<Loan> GetByBookId(Guid bookId)
        {
            return _context.Loans
                .Where(x => x.BookId == bookId && !x.IsDeleted)
                .ToList();
        }

        public void Add(Loan loan)
        {
            _context.Loans.Add(loan);
            _context.SaveChanges();
        }

        public void Update(Loan loan)
        {
            _context.Loans.Update(loan);
            _context.SaveChanges();
        }

        public void Delete(Loan loan)
        {
            loan.IsDeleted = true;
            _context.Loans.Update(loan);
            _context.SaveChanges();
        }
    }
}