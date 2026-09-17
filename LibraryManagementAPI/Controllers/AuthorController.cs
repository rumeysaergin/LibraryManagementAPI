using LibraryManagementAPI.Application.DTOs.Author;
using LibraryManagementAPI.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthorController : ControllerBase
    {
        private readonly IAuthorService _authorService;

        public AuthorController(IAuthorService authorService)
        {
            _authorService = authorService;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_authorService.GetAll());
        }

        [HttpGet("{id}")]
        public IActionResult GetById(Guid id)
        {
            var author = _authorService.GetById(id);

            if (author == null)
                return NotFound();

            return Ok(author);
        }

        [HttpPost]
        public IActionResult Create(AuthorCreateDto dto)
        {
            _authorService.Add(dto);

            return Ok(new
            {
                message = "Author created successfully."
            });
        }

        [HttpPut("{id}")]
        public IActionResult Update(Guid id, AuthorUpdateDto dto)
        {
            try
            {
                _authorService.Update(id, dto);

                return Ok(new
                {
                    message = "Author updated successfully."
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
                _authorService.Delete(id);

                return Ok(new
                {
                    message = "Author deleted successfully."
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