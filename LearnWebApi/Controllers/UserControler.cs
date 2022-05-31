using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using LearnWebApi.Models;
using Microsoft.EntityFrameworkCore;
using LearnWebApi.Request_Types;

namespace LearnWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : Controller
    {

        public readonly DataContext context;
        private readonly string missingDataError;

        public UserController(DataContext context)
        {
            this.context = context;
            this.missingDataError = $"Firstname, Surname, Password and Email are all required fields";
        }

        [HttpGet]
        public async Task<ActionResult<List<User>>> GetUsers()
        {
            List<User> users = await this.context.User
                .ToListAsync();

            return Ok(users);
        }

        [HttpGet("{email}")]
        public async Task<ActionResult<User>> GetUser(UserFindDto request)
        {
            try
            {
                User user = await this.context.User
                    .SingleAsync(thisUser => thisUser.Email == request.Email);

                if (request.Password != user.Password)
                {
                    return BadRequest("Email or password is wrong");
                }

                return Ok(user);

            } catch (Exception)
            {
                return BadRequest("Email or password is wrong");
            }
        }

        [HttpPost]
        public async Task<ActionResult<User>> PostUser(User user)
        {
            {
                try
                {
                    User userWithThisEmail = await this.context.User
                        .SingleAsync(thisUser => thisUser.Email == user.Email);

                    if (userWithThisEmail != null)
                    {
                        return BadRequest($"User with email {userWithThisEmail.Email} already exists");
                    }
                } catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

                if (
                    string.IsNullOrEmpty(user.Email) ||
                    string.IsNullOrEmpty(user.Name) ||
                    string.IsNullOrEmpty(user.Surname) ||
                    string.IsNullOrEmpty(user.Password)
                    )
                {
                    return BadRequest(this.missingDataError);
                }  

                try
                {
                    this.context.User.Add(user);
                    await this.context.SaveChangesAsync();

                    return Ok(user);

                } catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
        }

        [HttpDelete]
        public async Task<ActionResult<User>> DeleteUser(UserFindDto request)
        {
            try
            {
                User user = await this.context.User.SingleAsync(thisUser => thisUser.Email == request.Email);

                if (request.Password != user.Password)
                {
                    return BadRequest("email or password is wrong");
                }

                if (user != null)
                {
                    this.context.User.Remove(user);
                    await this.context.SaveChangesAsync();

                    var transactions = await this.context.Transaction.ToListAsync();

                    if (transactions != null)
                    {
                        for (int i = 0; i < transactions.Count; i++)
                        {
                            user.Transactions.Add(transactions[i]);
                        }
                    }

                }

                return Ok($"User {request.Email} has been succesfully deleted");

            } catch (Exception)
            {
                return BadRequest("email or password is wrong");
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<User>> ChangeUser(UserChangeDto request)
        {
            try
            {
                var user = await this.context.User.FindAsync(request.Id);

                if (request.Password != user?.Password)
                {
                    return BadRequest("email or password is wrong");
                }

                user.Name = request.Name; 
                user.Email = request.Email; 
                user.Surname = request.Surname;
                user.Password = request.NewPassword;

                await this.context.SaveChangesAsync();
                return Ok(user);

            }
            catch (Exception)
            {
                return BadRequest("email or password is wrong");
            }
        }
    }
}
