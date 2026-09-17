using LibraryManagementAPI.Application.DTOs.FinePayment;

namespace LibraryManagementAPI.Application.Services
{
    public interface IFinePaymentService
    {
        List<FinePaymentResponseDto> GetAll();
        FinePaymentResponseDto? GetById(Guid id);
        List<FinePaymentResponseDto> GetByFineId(Guid fineId);

        void Add(FinePaymentCreateDto dto);
        void Update(Guid id, FinePaymentUpdateDto dto);
        void Delete(Guid id);
    }
}