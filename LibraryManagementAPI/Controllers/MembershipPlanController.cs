using LibraryManagementAPI.Application.DTOs.MembershipPlan;
using LibraryManagementAPI.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MembershipPlanController : ControllerBase
    {
        private readonly IMembershipPlanService _membershipPlanService;

        public MembershipPlanController(
            IMembershipPlanService membershipPlanService)
        {
            _membershipPlanService = membershipPlanService;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_membershipPlanService.GetAll());
        }

        [HttpGet("{id}")]
        public IActionResult GetById(Guid id)
        {
            var membershipPlan = _membershipPlanService.GetById(id);

            if (membershipPlan == null)
                return NotFound();

            return Ok(membershipPlan);
        }

        [HttpPost]
        public IActionResult Create(MembershipPlanCreateDto dto)
        {
            _membershipPlanService.Add(dto);

            return Ok(new
            {
                message = "Membership plan created successfully."
            });
        }

        [HttpPut("{id}")]
        public IActionResult Update(Guid id, MembershipPlanUpdateDto dto)
        {
            try
            {
                _membershipPlanService.Update(id, dto);

                return Ok(new
                {
                    message = "Membership plan updated successfully."
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
                _membershipPlanService.Delete(id);

                return Ok(new
                {
                    message = "Membership plan deleted successfully."
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