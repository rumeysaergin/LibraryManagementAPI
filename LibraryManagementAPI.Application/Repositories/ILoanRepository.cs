using LibraryManagementAPI.Domain.Entities;

namespace LibraryManagementAPI.Application.Repositories
{
    public interface ILoanRepository
    {
        List<Loan> GetAll();
        Loan? GetById(Guid id);
        List<Loan> GetByUserId(Guid userId);
        List<Loan> GetByLibraryBookId(Guid libraryBookId);

        void Add(Loan loan);
        void Update(Loan loan);
        void Delete(Loan loan);
    }
}