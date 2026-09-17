using LibraryManagementAPI.Application.Repositories;
using LibraryManagementAPI.Domain.Entities;
using LibraryManagementAPI.Infrastructure.Data;

namespace LibraryManagementAPI.Infrastructure.Repositories
{
    public class FinePaymentRepository : IFinePaymentRepository
    {
        private readonly LibraryManagementDbContext _context;

        public FinePaymentRepository(LibraryManagementDbContext context)
        {
            _context = context;
        }

        public List<FinePayment> GetAll()
        {
            return _context.FinePayments
                .Where(x => !x.IsDeleted)
                .ToList();
        }

        public FinePayment? GetById(Guid id)
        {
            return _context.FinePayments
                .FirstOrDefault(x => x.Id == id && !x.IsDeleted);
        }

        public List<FinePayment> GetByFineId(Guid fineId)
        {
            return _context.FinePayments
                .Where(x => x.FineId == fineId && !x.IsDeleted)
                .ToList();
        }

        public void Add(FinePayment finePayment)
        {
            _context.FinePayments.Add(finePayment);
            _context.SaveChanges();
        }

        public void Update(FinePayment finePayment)
        {
            _context.FinePayments.Update(finePayment);
            _context.SaveChanges();
        }

        public void Delete(FinePayment finePayment)
        {
            finePayment.IsDeleted = true;
            _context.FinePayments.Update(finePayment);
            _context.SaveChanges();
        }
    }
}