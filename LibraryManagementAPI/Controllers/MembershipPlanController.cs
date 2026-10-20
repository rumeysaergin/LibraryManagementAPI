
using LibraryManagementAPI.Application.DTOs.MembershipPlan;
using LibraryManagementAPI.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class MembershipPlanController : ControllerBase
    {
        private readonly IMembershipPlanService _membershipPlanService;

        public MembershipPlanController(
            IMembershipPlanService membershipPlanService)
        {
            _membershipPlanService = membershipPlanService;
        }

        // Giriş yapmış kullanıcılar planları görüntüleyebilir.
        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_membershipPlanService.GetAll());
        }

        [HttpGet("{id:guid}")]
        public IActionResult GetById(Guid id)
        {
            var membershipPlan = _membershipPlanService.GetById(id);

            if (membershipPlan == null)
                return NotFound();

            return Ok(membershipPlan);
        }

        // Plan oluşturma yalnızca Admin'e açıktır.
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Create(MembershipPlanCreateDto dto)
        {
            try
            {
                _membershipPlanService.Add(dto);

                return Ok(new
                {
                    message = "Üyelik planı başarıyla oluşturuldu."
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
        public IActionResult Update(Guid id, MembershipPlanUpdateDto dto)
        {
            try
            {
                _membershipPlanService.Update(id, dto);

                return Ok(new
                {
                    message = "Üyelik planı başarıyla güncellendi."
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
                _membershipPlanService.Delete(id);

                return Ok(new
                {
                    message = "Üyelik planı başarıyla silindi."
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
