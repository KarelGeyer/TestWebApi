using Microsoft.AspNetCore.Mvc;
using LearnWebApi.Models;
using LearnWebApi.Request_Types;
using LearnWebApi.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace LearnWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : Controller
    {

        public readonly DataContext _context;
        private readonly IAuth _auth;

        public UserController(DataContext context, IAuth auth)
        {
            _context = context;
            _auth = auth;
        }

        [HttpGet]
        public async Task<ActionResult<List<User>>> GetUsers()
        {
            List<User> users = await _context.User
                .Include(x => x.Transactions)
                .ToListAsync();

            return Ok(users);
        }

        [HttpGet("{email}"), Authorize(Roles = "Admin")]
        public async Task<ActionResult<User>> GetUser(string email)
        {
            try
            {
                User user = await _context.User
                    .SingleAsync(thisUser => thisUser.Email == email);

                return Ok(user);

            } catch (Exception)
            {
                return BadRequest("Email or password is wrong");
            }
        }

        [HttpPost]
        public async Task<ActionResult<User>> PostUser(UserPost user)
        {
            {
                try
                {
                    User userWithThisEmail = await _context.User
                        .SingleAsync(thisUser => thisUser.Email == user.Email);

                    if (userWithThisEmail != null)
                    {
                        return BadRequest($"User with email {userWithThisEmail.Email} already exists");
                    }
                } catch (Exception)
                {
                    Console.WriteLine($"Something went wrong when trying to find if user exists");
                }

                try
                {
                    _auth.CreatePasswordHash(user.Password, out byte[] passwordHash, out byte[] passwordSalt);

                    User newUser = new()
                    {
                        Email = user.Email,
                        Name = user.Name,
                        Surname = user.Surname,
                        Password = user.Password,
                        Role = user.Role,
                        PasswordHash = passwordHash,
                        PasswordSalt = passwordSalt
                    };

                    _context.User.Add(newUser);
                    await _context.SaveChangesAsync();

                    return Ok(newUser);

                } catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(string email, string password)
        {
            try
            {
                User foundUser = await _context.User
                    .SingleAsync(thisUser => thisUser.Email == email);

                if (foundUser.PasswordHash != null && foundUser.PasswordSalt != null) 
                {
                    if (!_auth.VerifyPasswordHash(password, foundUser.PasswordHash, foundUser.PasswordSalt))
                    {
                        return BadRequest("Wrong Password");
                    }
                }

                string token = _auth.CreateToken(foundUser);
                return Ok(token);
            } catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete, Authorize]
        public async Task<ActionResult<User>> DeleteUser(string email)
        {
            try
            {
                User user = await _context.User.SingleAsync(thisUser => thisUser.Email == email);

                if (user != null)
                {
                    _context.User.Remove(user);
                    await _context.SaveChangesAsync();

                    var transactions = await _context.Transaction.ToListAsync();

                    if (transactions != null)
                    {
                        for (int i = 0; i < transactions.Count; i++)
                        {
                            _context.Transaction.Remove(transactions[i]);
                        }
                    }

                }

                return Ok($"User has been succesfully deleted");

            } catch (Exception)
            {
                return BadRequest("email or password is wrong");
            }
        }

        [HttpPut("email"), Authorize]
        public async Task<ActionResult<User>> ChangeUser(UserType request)
        {
            try
            {
                User user = await _context.User
                    .SingleAsync(thisUser => thisUser.Email == request.Email);

                if (user == null)
                {
                    return BadRequest("Not found");
                }

                user.Name = request.Name;
                user.Email = request.Email;
                user.Surname = request.Surname;
                user.Password = request.NewPassword;

                await _context.SaveChangesAsync();
                return Ok(user);

            }
            catch (Exception)
            {
                return BadRequest("email or password is wrong");
            }
        }

        [HttpPut("role"), Authorize(Roles = "Admin")]
        public async Task<ActionResult<User>> ChangeUserRole(string email, string role)
        {
            try
            {
                User user = await _context.User
                    .SingleAsync(thisUser => thisUser.Email == email);

                if (user == null)
                {
                    return BadRequest("Not found");
                }

                user.Role = role;

                await _context.SaveChangesAsync();
                return Ok(user);

            }
            catch (Exception)
            {
                return BadRequest("email or password is wrong");
            }
        }
    }
}
