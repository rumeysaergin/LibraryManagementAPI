using LibraryManagementAPI.Application.DTOs.Loan;

namespace LibraryManagementAPI.Application.Services
{
    public interface ILoanService
    {
        List<LoanResponseDto> GetAll();

        LoanResponseDto? GetById(Guid id);

        List<LoanResponseDto> GetByUserId(Guid userId);

        List<LoanResponseDto> GetByLibraryBookId(Guid libraryBookId);

        void Add(LoanCreateDto dto);

        void Update(Guid id, LoanUpdateDto dto);

        void Delete(Guid id);
    }
}