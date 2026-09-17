using LibraryManagementAPI.Application.DTOs.UserMembership;
using LibraryManagementAPI.Application.Repositories;
using LibraryManagementAPI.Domain.Entities;

namespace LibraryManagementAPI.Application.Services
{
    public class UserMembershipService : IUserMembershipService
    {
        private readonly IUserMembershipRepository _userMembershipRepository;

        public UserMembershipService(IUserMembershipRepository userMembershipRepository)
        {
            _userMembershipRepository = userMembershipRepository;
        }

        public List<UserMembershipResponseDto> GetAll()
        {
            return _userMembershipRepository.GetAll()
                .Select(x => new UserMembershipResponseDto
                {
                    Id = x.Id,
                    UserId = x.UserId,
                    MembershipPlanId = x.MembershipPlanId,
                    StartDate = x.StartDate,
                    EndDate = x.EndDate,
                    IsActive = x.IsActive
                })
                .ToList();
        }

        public UserMembershipResponseDto? GetById(Guid id)
        {
            var userMembership = _userMembershipRepository.GetById(id);

            if (userMembership == null)
                return null;

            return new UserMembershipResponseDto
            {
                Id = userMembership.Id,
                UserId = userMembership.UserId,
                MembershipPlanId = userMembership.MembershipPlanId,
                StartDate = userMembership.StartDate,
                EndDate = userMembership.EndDate,
                IsActive = userMembership.IsActive
            };
        }

        public List<UserMembershipResponseDto> GetByUserId(Guid userId)
        {
            return _userMembershipRepository.GetByUserId(userId)
                .Select(x => new UserMembershipResponseDto
                {
                    Id = x.Id,
                    UserId = x.UserId,
                    MembershipPlanId = x.MembershipPlanId,
                    StartDate = x.StartDate,
                    EndDate = x.EndDate,
                    IsActive = x.IsActive
                })
                .ToList();
        }

        public void Add(UserMembershipCreateDto dto)
        {
            var userMembership = new UserMembership
            {
                Id = Guid.NewGuid(),
                UserId = dto.UserId,
                MembershipPlanId = dto.MembershipPlanId,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                IsActive = dto.IsActive,
                IsDeleted = false
            };

            _userMembershipRepository.Add(userMembership);
        }

        public void Update(Guid id, UserMembershipUpdateDto dto)
        {
            var userMembership = _userMembershipRepository.GetById(id);

            if (userMembership == null)
                throw new KeyNotFoundException("User membership not found.");

            userMembership.MembershipPlanId = dto.MembershipPlanId;
            userMembership.StartDate = dto.StartDate;
            userMembership.EndDate = dto.EndDate;
            userMembership.IsActive = dto.IsActive;

            _userMembershipRepository.Update(userMembership);
        }

        public void Delete(Guid id)
        {
            var userMembership = _userMembershipRepository.GetById(id);

            if (userMembership == null)
                throw new KeyNotFoundException("User membership not found.");

            _userMembershipRepository.Delete(userMembership);
        }
    }
}