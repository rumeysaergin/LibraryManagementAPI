using LibraryManagementAPI.Application.DTOs.Library;
using LibraryManagementAPI.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LibraryController : ControllerBase
    {
        private readonly ILibraryService _libraryService;

        public LibraryController(ILibraryService libraryService)
        {
            _libraryService = libraryService;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_libraryService.GetAll());
        }

        [HttpGet("{id}")]
        public IActionResult GetById(Guid id)
        {
            var library = _libraryService.GetById(id);

            if (library == null)
                return NotFound();

            return Ok(library);
        }

        [HttpPost]
        public IActionResult Create(LibraryCreateDto dto)
        {
            try
            {
                _libraryService.Add(dto);

                return Ok(new
                {
                    message = "Library created successfully."
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        [HttpPut("{id}")]
        public IActionResult Update(Guid id, LibraryUpdateDto dto)
        {
            try
            {
                _libraryService.Update(id, dto);

                return Ok(new
                {
                    message = "Library updated successfully."
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
                _libraryService.Delete(id);

                return Ok(new
                {
                    message = "Library deleted successfully."
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