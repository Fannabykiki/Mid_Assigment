using BookStore.API.DTOs;
using BookStore.API.Services.BookService;
using BookStore.API.Services.UserService;
using BookStore.Common.DTOs.Book.BookBorrowingRequest;
using BookStore.Data.Entities;
using BookStore.Extensions;
using BookStore.Service.Services.Loggerservice;
using Common.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Book.API.Controllers
{
    [Authorize]
    [Route("/api/book-management")]
    [ApiController]
    public class BookController : ControllerBase
    {
        private readonly ILoggerManager _logger;
        private readonly IBookService _bookService;
        private readonly IUsersService _usersService;

        public BookController(IBookService bookService, ILoggerManager logger, IUsersService usersService)
        {
            _bookService = bookService;
            _logger = logger;
            _usersService = usersService;
        }

        [AllowAnonymous]
        [HttpGet("books")]
        public async Task<IActionResult> GetAllBook()
        {
            var result = await _bookService.GetAllBookAsync();
            if (result == null) return StatusCode(500);
            //throw new ArgumentNullException();
            return Ok(result);
        }

        [Authorize(Roles = UserRoles.SuperUser)]
        [AllowAnonymous]
        [HttpPost("books")]
        public async Task<IActionResult> Create([FromBody] AddBookRequest addBook)
        {
            var result = await _bookService.CreateAsync(addBook);

            if (result == null) return StatusCode(500);

            return Ok(result);
        }

        [HttpDelete("books/{id}")]
        [Authorize(Roles = UserRoles.SuperUser)]
        [AllowAnonymous]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _bookService.DeleteAsync(id);

            return Ok(result);
        }

        [HttpPut("books/{id}")]
        [Authorize(Roles = UserRoles.SuperUser)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateBookRequest updateBookRequest)
        {
            var result = await _bookService.UpdateAsync(id, updateBookRequest);

            if (result == null) return StatusCode(500);

            return Ok(result);
        }

        [AllowAnonymous]
        [HttpGet("books/{id}")]
        public async Task<IActionResult> GetBookById(int id)
        {
            var result = await _bookService.GetBookByIdAsync(id);

            if (result == null) return NotFound();

            return Ok(result);
        }

        [Authorize(Roles = UserRoles.SuperUser)]
        [HttpPost("book-borrowing")]
        public async Task<IActionResult> CreateBookBorrowing(CreateBookBorrowingRequest createBookBorrowingRequest)
        {
            var userId = this.GetCurrentLoginUserId();
            if(userId == null)
            {
                return NotFound();
            }
            if (userId != null)
            {
                var user = await _usersService.GetUserByIdAsync(userId.Value);
                if (user != null)
                {
                    var bookBorrowingRequest = await _bookService.CreateBookBorrowing(createBookBorrowingRequest, user);

                    return bookBorrowingRequest != null ? Ok(bookBorrowingRequest) : BadRequest();
                }
                else
                    return BadRequest();
            }
            else
                return BadRequest();
        }


        [Authorize(Roles = UserRoles.SuperUser)]
        [HttpGet("book-borrowing")]
        public async Task<IEnumerable<BookBorrowingRequest>> GetAllBookRequest()
        {
            return await _bookService.GetAllBookRequestAsync();
        }

        [Authorize(Roles = UserRoles.SuperUser)]
        [HttpPut("book-borrowing/{id}")]
        public async Task<IActionResult> UpdateBorrowingRequest(int id,UpdateBorrowingRequest updateBorrowingRequest)
        {
            var userId = this.GetCurrentLoginUserId();
            if (userId == null)
            {
                return NotFound();
            }
            if (userId != null)
            {
                var user = await _usersService.GetUserByIdAsync(userId.Value);
                if (user != null)
                {
                    var bookBorrowingRequest = await _bookService.UpdateBorrowingRequestAsync(user, updateBorrowingRequest,id);

                    return bookBorrowingRequest != null ? Ok(bookBorrowingRequest) : BadRequest();
                }
                else
                    return BadRequest();
            }
            else
                return BadRequest();
        }

        [Authorize(Roles = UserRoles.SuperUser)]
        [HttpGet("book-borrowingdetail/{id}")]
        public async Task<IActionResult> GetBorrowingDetailByRequestIdAsync(int id)
        {
            var result = await _bookService.GetBorrowingDetailByRequestIdAsync(id);
            if (result == null) return NotFound();

            return Ok(result);
        }

        [Authorize(Roles = UserRoles.SuperUser)]
        [HttpGet("book-borrowingrequest")]
        public async Task<IActionResult> GetRequestByUserId()
        {
            var userId = this.GetCurrentLoginUserId();  
            if (userId == null)
            {
                return NotFound();
            }
            if (userId != null)
            {
                var user = await _usersService.GetUserByIdAsync(userId.Value);
                if (user != null)
                {
                    var bookBorrowingRequest = await _bookService.GetRequestByUserId(user);

                    return bookBorrowingRequest != null ? Ok(bookBorrowingRequest) : BadRequest();
                }
                else
                    return BadRequest();
            }
            else
                return BadRequest();
        }
    }
}
