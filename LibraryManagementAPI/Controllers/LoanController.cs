
using LibraryManagementAPI.Application.DTOs.Loan;
using LibraryManagementAPI.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoanController : ControllerBase
    {
        private readonly ILoanService _loanService;

        public LoanController(ILoanService loanService)
        {
            _loanService = loanService;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_loanService.GetAll());
        }

        [HttpGet("{id}")]
        public IActionResult GetById(Guid id)
        {
            var loan = _loanService.GetById(id);

            if (loan == null)
                return NotFound();

            return Ok(loan);
        }

        [HttpGet("user/{userId}")]
        public IActionResult GetByUserId(Guid userId)
        {
            return Ok(_loanService.GetByUserId(userId));
        }

        [HttpGet("library-book/{libraryBookId}")]
        public IActionResult GetByLibraryBookId(Guid libraryBookId)
        {
            return Ok(_loanService.GetByLibraryBookId(libraryBookId));
        }

        [HttpPost]
        public IActionResult Create(LoanCreateDto dto)
        {
            try
            {
                _loanService.Add(dto);

                return Ok(new
                {
                    message = "Loan created successfully."
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

        [HttpPut("{id}/return")]
        public IActionResult ReturnLoan(Guid id)
        {
            try
            {
                _loanService.ReturnLoan(id);

                return Ok(new
                {
                    message = "Book returned successfully."
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

        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            try
            {
                _loanService.Delete(id);

                return Ok(new
                {
                    message = "Loan deleted successfully."
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