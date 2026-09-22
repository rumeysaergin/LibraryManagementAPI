using LibraryManagementAPI.Application.DTOs.Fine;
using LibraryManagementAPI.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FineController : ControllerBase
    {
        private readonly IFineService _fineService;

        public FineController(IFineService fineService)
        {
            _fineService = fineService;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_fineService.GetAll());
        }

        [HttpGet("{id}")]
        public IActionResult GetById(Guid id)
        {
            var fine = _fineService.GetById(id);

            if (fine == null)
                return NotFound();

            return Ok(fine);
        }

        [HttpGet("loan/{loanId}")]
        public IActionResult GetByLoanId(Guid loanId)
        {
            return Ok(_fineService.GetByLoanId(loanId));
        }

        [HttpPost]
        public IActionResult Create(FineCreateDto dto)
        {
            try
            {
                _fineService.Add(dto);

                return Ok(new
                {
                    message = "Fine calculated and created successfully."
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    message = ex.Message
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        [HttpPut("{id}")]
        public IActionResult Update(Guid id, FineUpdateDto dto)
        {
            try
            {
                _fineService.Update(id, dto);

                return Ok(new
                {
                    message = "Fine updated successfully."
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
                _fineService.Delete(id);

                return Ok(new
                {
                    message = "Fine deleted successfully."
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