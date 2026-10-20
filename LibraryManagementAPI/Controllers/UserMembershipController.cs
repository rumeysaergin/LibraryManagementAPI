
using LibraryManagementAPI.Application.DTOs.UserMembership;
using LibraryManagementAPI.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LibraryManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserMembershipController : ControllerBase
    {
        private readonly IUserMembershipService _userMembershipService;

        public UserMembershipController(
            IUserMembershipService userMembershipService)
        {
            _userMembershipService = userMembershipService;
        }

        // Yalnızca Admin tüm üyelikleri görebilir.
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult GetAll()
        {
            return Ok(_userMembershipService.GetAll());
        }

        // Yalnızca Admin herhangi bir üyeliği ID ile görüntüleyebilir.
        [HttpGet("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public IActionResult GetById(Guid id)
        {
            var membership = _userMembershipService.GetById(id);

            if (membership == null)
                return NotFound();

            return Ok(membership);
        }

        // Kullanıcı kendi üyeliklerini, Admin ise tüm kullanıcıların
        // üyeliklerini ilgili kullanıcı ID'si üzerinden görebilir.
        [HttpGet("user/{userId:guid}")]
        public IActionResult GetByUserId(Guid userId)
        {
            if (!User.IsInRole("Admin"))
            {
                var userIdValue =
                    User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (!Guid.TryParse(userIdValue, out var currentUserId))
                    return Unauthorized(new
                    {
                        message = "Kullanıcı kimliği token içerisinde bulunamadı."
                    });

                if (currentUserId != userId)
                    return Forbid();
            }

            return Ok(_userMembershipService.GetByUserId(userId));
        }

        // Üyelikler ödeme akışı üzerinden oluşturulmalıdır.
        // Doğrudan oluşturma yalnızca Admin'e açıktır.
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Create(UserMembershipCreateDto dto)
        {
            try
            {
                _userMembershipService.Add(dto);

                return Ok(new
                {
                    message = "Üyelik kaydı oluşturuldu."
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Update(
            Guid id,
            UserMembershipUpdateDto dto)
        {
            try
            {
                _userMembershipService.Update(id, dto);

                return Ok(new
                {
                    message = "Üyelik kaydı güncellendi."
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(Guid id)
        {
            try
            {
                _userMembershipService.Delete(id);

                return Ok(new
                {
                    message = "Üyelik kaydı silindi."
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}
