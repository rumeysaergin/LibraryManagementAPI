using LibraryManagementAPI.Application.DTOs.UserLibrary;
using LibraryManagementAPI.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserLibraryController : ControllerBase
    {
        private readonly IUserLibraryService _userLibraryService;

        public UserLibraryController(IUserLibraryService userLibraryService)
        {
            _userLibraryService = userLibraryService;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_userLibraryService.GetAll());
        }

        [HttpGet("{id}")]
        public IActionResult GetById(Guid id)
        {
            var result = _userLibraryService.GetById(id);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpGet("user/{userId}")]
        public IActionResult GetByUserId(Guid userId)
        {
            return Ok(_userLibraryService.GetByUserId(userId));
        }

        [HttpGet("library/{libraryId}")]
        public IActionResult GetByLibraryId(Guid libraryId)
        {
            return Ok(_userLibraryService.GetByLibraryId(libraryId));
        }

        [HttpPost]
        public IActionResult Add(UserLibraryCreateDto dto)
        {
            _userLibraryService.Add(dto);

            return Ok();
        }

        [HttpPut("{id}")]
        public IActionResult Update(Guid id, UserLibraryUpdateDto dto)
        {
            _userLibraryService.Update(id, dto);

            return Ok();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            _userLibraryService.Delete(id);

            return Ok();
        }
    }
}