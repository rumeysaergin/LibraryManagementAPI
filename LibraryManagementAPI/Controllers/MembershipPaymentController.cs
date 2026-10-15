
using LibraryManagementAPI.Application.DTOs.MembershipPayment;
using LibraryManagementAPI.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LibraryManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class MembershipPaymentController : ControllerBase
    {
        private readonly IMembershipPaymentService _membershipPaymentService;

        public MembershipPaymentController(
            IMembershipPaymentService membershipPaymentService)
        {
            _membershipPaymentService = membershipPaymentService;
        }

        // Yalnızca admin bütün ödemeleri görebilir.
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult GetAll()
        {
            return Ok(_membershipPaymentService.GetAll());
        }

        // Yalnızca admin ödeme detayını ID ile görebilir.
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult GetById(Guid id)
        {
            var payment = _membershipPaymentService.GetById(id);

            if (payment == null)
                return NotFound();

            return Ok(payment);
        }

        // Kullanıcı yalnızca kendi ödeme geçmişini görebilir.
        [HttpGet("my-payments")]
        public IActionResult GetMyPayments()
        {
            var userIdValue = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(userIdValue, out var userId))
                return Unauthorized(new
                {
                    message = "Kullanıcı kimliği token içerisinde bulunamadı."
                });

            return Ok(_membershipPaymentService.GetByUserId(userId));
        }

        // Bu yönetim sorgusu yalnızca admin içindir.
        [HttpGet("membership/{userMembershipId}")]
        [Authorize(Roles = "Admin")]
        public IActionResult GetByUserMembershipId(Guid userMembershipId)
        {
            return Ok(
                _membershipPaymentService
                    .GetByUserMembershipId(userMembershipId));
        }

        // Giriş yapan kullanıcı yalnızca kendisine ait üyelik için
        // bekleyen ödeme kaydı oluşturabilir.
        [HttpPost]
        public IActionResult Create(MembershipPaymentCreateDto dto)
        {
            var userIdValue = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(userIdValue, out var userId))
                return Unauthorized(new
                {
                    message = "Kullanıcı kimliği token içerisinde bulunamadı."
                });

            try
            {
                _membershipPaymentService.Add(dto, userId);

                return Ok(new
                {
                    message = "Ödeme kaydı oluşturuldu. Yönetici onayı bekleniyor."
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // Ödeme bilgilerini düzenlemek yalnızca admin içindir.
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Update(
            Guid id,
            MembershipPaymentUpdateDto dto)
        {
            try
            {
                _membershipPaymentService.Update(id, dto);

                return Ok(new
                {
                    message = "Ödeme kaydı güncellendi."
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

        // Ödeme onayı yalnızca admin tarafından yapılabilir.
        [HttpPut("{id}/mark-as-paid")]
        [Authorize(Roles = "Admin")]
        public IActionResult MarkAsPaid(Guid id)
        {
            try
            {
                _membershipPaymentService.MarkAsPaid(id);

                return Ok(new
                {
                    message = "Ödeme onaylandı."
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        // Bekleyen bir ödeme kaydını silmek yalnızca admin içindir.
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(Guid id)
        {
            try
            {
                _membershipPaymentService.Delete(id);

                return Ok(new
                {
                    message = "Ödeme kaydı silindi."
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
