using LibraryManagementAPI.Application.DTOs.MembershipPayment;
using LibraryManagementAPI.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MembershipPaymentController : ControllerBase
    {
        private readonly IMembershipPaymentService _membershipPaymentService;

        public MembershipPaymentController(
            IMembershipPaymentService membershipPaymentService)
        {
            _membershipPaymentService = membershipPaymentService;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_membershipPaymentService.GetAll());
        }

        [HttpGet("{id}")]
        public IActionResult GetById(Guid id)
        {
            var payment = _membershipPaymentService.GetById(id);

            if (payment == null)
                return NotFound();

            return Ok(payment);
        }

        [HttpGet("membership/{userMembershipId}")]
        public IActionResult GetByUserMembershipId(Guid userMembershipId)
        {
            return Ok(
                _membershipPaymentService.GetByUserMembershipId(userMembershipId)
            );
        }

        [HttpPost]
        public IActionResult Create(MembershipPaymentCreateDto dto)
        {
            _membershipPaymentService.Add(dto);

            return Ok(new
            {
                message = "Membership payment created successfully."
            });
        }

        [HttpPut("{id}")]
        public IActionResult Update(Guid id, MembershipPaymentUpdateDto dto)
        {
            try
            {
                _membershipPaymentService.Update(id, dto);

                return Ok(new
                {
                    message = "Membership payment updated successfully."
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
                _membershipPaymentService.Delete(id);

                return Ok(new
                {
                    message = "Membership payment deleted successfully."
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