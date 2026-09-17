using LibraryManagementAPI.Application.DTOs.Book;
using LibraryManagementAPI.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookController : ControllerBase
    {
        private readonly IBookService _bookService;

        public BookController(IBookService bookService)
        {
            _bookService = bookService;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_bookService.GetAll());
        }

        [HttpGet("{id}")]
        public IActionResult GetById(Guid id)
        {
            var book = _bookService.GetById(id);

            if (book == null)
                return NotFound();

            return Ok(book);
        }

        [HttpGet("author/{authorId}")]
        public IActionResult GetByAuthorId(Guid authorId)
        {
            return Ok(_bookService.GetByAuthorId(authorId));
        }

        [HttpGet("category/{categoryId}")]
        public IActionResult GetByCategoryId(Guid categoryId)
        {
            return Ok(_bookService.GetByCategoryId(categoryId));
        }

        [HttpPost]
        public IActionResult Create(BookCreateDto dto)
        {
            _bookService.Add(dto);

            return Ok(new
            {
                message = "Book created successfully."
            });
        }

        [HttpPut("{id}")]
        public IActionResult Update(Guid id, BookUpdateDto dto)
        {
            try
            {
                _bookService.Update(id, dto);

                return Ok(new
                {
                    message = "Book updated successfully."
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
                _bookService.Delete(id);

                return Ok(new
                {
                    message = "Book deleted successfully."
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