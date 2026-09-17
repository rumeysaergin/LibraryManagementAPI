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

        [HttpGet("book/{bookId}")]
        public IActionResult GetByBookId(Guid bookId)
        {
            return Ok(_loanService.GetByBookId(bookId));
        }

        [HttpPost]
        public IActionResult Create(LoanCreateDto dto)
        {
            _loanService.Add(dto);

            return Ok(new
            {
                message = "Loan created successfully."
            });
        }

        [HttpPut("{id}")]
        public IActionResult Update(Guid id, LoanUpdateDto dto)
        {
            try
            {
                _loanService.Update(id, dto);

                return Ok(new
                {
                    message = "Loan updated successfully."
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
                _loanService.Delete(id);

                return Ok(new
                {
                    message = "Loan deleted successfully."
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