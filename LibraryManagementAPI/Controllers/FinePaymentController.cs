using LibraryManagementAPI.Application.DTOs.FinePayment;
using LibraryManagementAPI.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FinePaymentController : ControllerBase
    {
        private readonly IFinePaymentService _finePaymentService;

        public FinePaymentController(
            IFinePaymentService finePaymentService)
        {
            _finePaymentService = finePaymentService;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_finePaymentService.GetAll());
        }

        [HttpGet("{id}")]
        public IActionResult GetById(Guid id)
        {
            var payment = _finePaymentService.GetById(id);

            if (payment == null)
                return NotFound();

            return Ok(payment);
        }

        [HttpGet("fine/{fineId}")]
        public IActionResult GetByFineId(Guid fineId)
        {
            return Ok(_finePaymentService.GetByFineId(fineId));
        }

        [HttpPost]
        public IActionResult Create(FinePaymentCreateDto dto)
        {
            _finePaymentService.Add(dto);

            return Ok(new
            {
                message = "Fine payment created successfully."
            });
        }

        [HttpPut("{id}")]
        public IActionResult Update(Guid id, FinePaymentUpdateDto dto)
        {
            try
            {
                _finePaymentService.Update(id, dto);

                return Ok(new
                {
                    message = "Fine payment updated successfully."
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
                _finePaymentService.Delete(id);

                return Ok(new
                {
                    message = "Fine payment deleted successfully."
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