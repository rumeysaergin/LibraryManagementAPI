using LibraryManagementAPI.Domain.Entities;

namespace LibraryManagementAPI.Application.Repositories
{
    public interface IFineRepository
    {
        List<Fine> GetAll();
        Fine? GetById(Guid id);
        List<Fine> GetByLoanId(Guid loanId);

        void Add(Fine fine);
        void Update(Fine fine);
        void Delete(Fine fine);
    }
}