using LibraryManagementAPI.Domain.Entities;

namespace LibraryManagementAPI.Application.Repositories
{
    public interface IFinePaymentRepository
    {
        List<FinePayment> GetAll();
        FinePayment? GetById(Guid id);
        List<FinePayment> GetByFineId(Guid fineId);

        void Add(FinePayment finePayment);
        void Update(FinePayment finePayment);
        void Delete(FinePayment finePayment);
    }
}