using LibraryManagementAPI.Application.DTOs.UserMembership;
using LibraryManagementAPI.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserMembershipController : ControllerBase
    {
        private readonly IUserMembershipService _userMembershipService;

        public UserMembershipController(
            IUserMembershipService userMembershipService)
        {
            _userMembershipService = userMembershipService;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_userMembershipService.GetAll());
        }

        [HttpGet("{id}")]
        public IActionResult GetById(Guid id)
        {
            var membership = _userMembershipService.GetById(id);

            if (membership == null)
                return NotFound();

            return Ok(membership);
        }

        [HttpGet("user/{userId}")]
        public IActionResult GetByUserId(Guid userId)
        {
            return Ok(_userMembershipService.GetByUserId(userId));
        }

        [HttpPost]
        public IActionResult Create(UserMembershipCreateDto dto)
        {
            _userMembershipService.Add(dto);

            return Ok(new
            {
                message = "User membership created successfully."
            });
        }

        [HttpPut("{id}")]
        public IActionResult Update(Guid id, UserMembershipUpdateDto dto)
        {
            try
            {
                _userMembershipService.Update(id, dto);

                return Ok(new
                {
                    message = "User membership updated successfully."
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    message = ex.Message
                });
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            try
            {
                _userMembershipService.Delete(id);

                return Ok(new
                {
                    message = "User membership deleted successfully."
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    message = ex.Message
                });
            }
        }
    }
}