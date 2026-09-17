using LibraryManagementAPI.Application.DTOs.UserMembership;

namespace LibraryManagementAPI.Application.Services
{
    public interface IUserMembershipService
    {
        List<UserMembershipResponseDto> GetAll();
        UserMembershipResponseDto? GetById(Guid id);
        List<UserMembershipResponseDto> GetByUserId(Guid userId);

        void Add(UserMembershipCreateDto dto);
        void Update(Guid id, UserMembershipUpdateDto dto);
        void Delete(Guid id);
    }
}