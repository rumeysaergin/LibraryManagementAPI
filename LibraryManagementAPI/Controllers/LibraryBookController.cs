using LibraryManagementAPI.Application.DTOs.LibraryBook;
using LibraryManagementAPI.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LibraryBookController : ControllerBase
    {
        private readonly ILibraryBookService _libraryBookService;

        public LibraryBookController(ILibraryBookService libraryBookService)
        {
            _libraryBookService = libraryBookService;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_libraryBookService.GetAll());
        }

        [HttpGet("{id}")]
        public IActionResult GetById(Guid id)
        {
            var libraryBook = _libraryBookService.GetById(id);

            if (libraryBook == null)
                return NotFound();

            return Ok(libraryBook);
        }

        [HttpGet("library/{libraryId}")]
        public IActionResult GetByLibraryId(Guid libraryId)
        {
            return Ok(_libraryBookService.GetByLibraryId(libraryId));
        }

        [HttpGet("book/{bookId}")]
        public IActionResult GetByBookId(Guid bookId)
        {
            return Ok(_libraryBookService.GetByBookId(bookId));
        }

        [HttpPost]
        public IActionResult Create(LibraryBookCreateDto dto)
        {
            try
            {
                _libraryBookService.Add(dto);

                return Ok(new
                {
                    message = "Book added to library successfully."
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
        public IActionResult Update(Guid id, LibraryBookUpdateDto dto)
        {
            try
            {
                _libraryBookService.Update(id, dto);

                return Ok(new
                {
                    message = "Library book quantity updated successfully."
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

        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            try
            {
                _libraryBookService.Delete(id);

                return Ok(new
                {
                    message = "Book removed from library successfully."
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