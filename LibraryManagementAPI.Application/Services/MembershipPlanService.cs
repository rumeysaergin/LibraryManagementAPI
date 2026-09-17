using LibraryManagementAPI.Application.DTOs.MembershipPlan;
using LibraryManagementAPI.Application.Repositories;
using LibraryManagementAPI.Domain.Entities;

namespace LibraryManagementAPI.Application.Services
{
    public class MembershipPlanService : IMembershipPlanService
    {
        private readonly IMembershipPlanRepository _membershipPlanRepository;

        public MembershipPlanService(IMembershipPlanRepository membershipPlanRepository)
        {
            _membershipPlanRepository = membershipPlanRepository;
        }

        public List<MembershipPlanResponseDto> GetAll()
        {
            return _membershipPlanRepository.GetAll()
                .Select(x => new MembershipPlanResponseDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    MaxBorrowLimit = x.MaxBorrowLimit,
                    Price = x.Price,
                    DurationInDays = x.DurationInDays
                })
                .ToList();
        }

        public MembershipPlanResponseDto? GetById(Guid id)
        {
            var membershipPlan = _membershipPlanRepository.GetById(id);

            if (membershipPlan == null)
                return null;

            return new MembershipPlanResponseDto
            {
                Id = membershipPlan.Id,
                Name = membershipPlan.Name,
                MaxBorrowLimit = membershipPlan.MaxBorrowLimit,
                Price = membershipPlan.Price,
                DurationInDays = membershipPlan.DurationInDays
            };
        }

        public void Add(MembershipPlanCreateDto dto)
        {
            var membershipPlan = new MembershipPlan
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                MaxBorrowLimit = dto.MaxBorrowLimit,
                Price = dto.Price,
                DurationInDays = dto.DurationInDays,
                IsDeleted = false
            };

            _membershipPlanRepository.Add(membershipPlan);
        }

        public void Update(Guid id, MembershipPlanUpdateDto dto)
        {
            var membershipPlan = _membershipPlanRepository.GetById(id);

            if (membershipPlan == null)
                throw new KeyNotFoundException("Membership plan not found.");

            membershipPlan.Name = dto.Name;
            membershipPlan.MaxBorrowLimit = dto.MaxBorrowLimit;
            membershipPlan.Price = dto.Price;
            membershipPlan.DurationInDays = dto.DurationInDays;

            _membershipPlanRepository.Update(membershipPlan);
        }

        public void Delete(Guid id)
        {
            var membershipPlan = _membershipPlanRepository.GetById(id);

            if (membershipPlan == null)
                throw new KeyNotFoundException("Membership plan not found.");

            _membershipPlanRepository.Delete(membershipPlan);
        }
    }
}