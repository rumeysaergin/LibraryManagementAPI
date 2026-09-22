using LibraryManagementAPI.Application.DTOs.Fine;

namespace LibraryManagementAPI.Application.Services
{
    public interface IFineService
    {
        List<FineResponseDto> GetAll();
        FineResponseDto? GetById(Guid id);
        List<FineResponseDto> GetByLoanId(Guid loanId);

        void Add(FineCreateDto dto);
        void CreateFineForLoan(Guid loanId);
        void Update(Guid id, FineUpdateDto dto);
        void Delete(Guid id);
    }
}