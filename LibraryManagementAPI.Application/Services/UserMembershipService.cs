
using LibraryManagementAPI.Application.DTOs.UserMembership;
using LibraryManagementAPI.Application.Repositories;
using LibraryManagementAPI.Domain.Entities;

namespace LibraryManagementAPI.Application.Services
{
    public class UserMembershipService : IUserMembershipService
    {
        private readonly IUserMembershipRepository _userMembershipRepository;

        public UserMembershipService(
            IUserMembershipRepository userMembershipRepository)
        {
            _userMembershipRepository = userMembershipRepository;
        }

        private static bool IsMembershipCurrentlyActive(UserMembership membership)
        {
            var now = DateTime.Now;

            return membership.IsActive
                && !membership.IsDeleted
                && membership.StartDate <= now
                && membership.EndDate >= now;
        }

        private static UserMembershipResponseDto MapToDto(UserMembership membership)
        {
            return new UserMembershipResponseDto
            {
                Id = membership.Id,
                UserId = membership.UserId,
                MembershipPlanId = membership.MembershipPlanId,
                StartDate = membership.StartDate,
                EndDate = membership.EndDate,
                IsActive = IsMembershipCurrentlyActive(membership)
            };
        }

        public List<UserMembershipResponseDto> GetAll()
        {
            return _userMembershipRepository.GetAll()
                .Where(x => !x.IsDeleted)
                .Select(MapToDto)
                .ToList();
        }

        public UserMembershipResponseDto? GetById(Guid id)
        {
            var membership = _userMembershipRepository.GetById(id);

            if (membership == null || membership.IsDeleted)
                return null;

            return MapToDto(membership);
        }

        public List<UserMembershipResponseDto> GetByUserId(Guid userId)
        {
            return _userMembershipRepository.GetByUserId(userId)
                .Where(x => !x.IsDeleted)
                .Select(MapToDto)
                .ToList();
        }

        public void Add(UserMembershipCreateDto dto)
        {
            if (dto.EndDate <= dto.StartDate)
            {
                throw new InvalidOperationException(
                    "Membership end date must be after the start date.");
            }

            var membership = new UserMembership
            {
                Id = Guid.NewGuid(),
                UserId = dto.UserId,
                MembershipPlanId = dto.MembershipPlanId,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                IsActive = dto.IsActive,
                IsDeleted = false
            };

            _userMembershipRepository.Add(membership);
        }

        public void Update(Guid id, UserMembershipUpdateDto dto)
        {
            var membership = _userMembershipRepository.GetById(id);

            if (membership == null || membership.IsDeleted)
            {
                throw new KeyNotFoundException(
                    "User membership not found.");
            }

            if (dto.EndDate <= dto.StartDate)
            {
                throw new InvalidOperationException(
                    "Membership end date must be after the start date.");
            }

            membership.MembershipPlanId = dto.MembershipPlanId;
            membership.StartDate = dto.StartDate;
            membership.EndDate = dto.EndDate;
            membership.IsActive = dto.IsActive;

            _userMembershipRepository.Update(membership);
        }

        public void Delete(Guid id)
        {
            var membership = _userMembershipRepository.GetById(id);

            if (membership == null || membership.IsDeleted)
            {
                throw new KeyNotFoundException(
                    "User membership not found.");
            }

            _userMembershipRepository.Delete(membership);
        }
    }
}
