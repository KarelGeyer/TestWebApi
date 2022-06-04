using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using LearnWebApi.Models;
using LearnWebApi.Types;
using LearnWebApi.Request_Types;
using Microsoft.AspNetCore.Authorization;

namespace LearnWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    // [Authorize] - You have to add AllowAnonymous to routes you want the be excludes - example -> [HttpGet, AllowAnonymous]
    public class TransactionController : Controller
    {
        private readonly DataContext _context;

        public TransactionController(DataContext context)
        {
            _context = context;
        }

        [HttpGet, Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<Transaction>>> GetTransactions()
        {
            try
            {
                List<Transaction> transactions = await _context.Transaction
                    .ToListAsync();

                return Ok(transactions);
            } catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{userId}"), Authorize]
        public async Task<ActionResult<Transaction>> GetUserTransactions(int userId) {
            try
            {
                Transaction transaction = await _context.Transaction
                    .Where(transaction => transaction.UserId == userId)
                    .ToListAsync();

                return Ok(transaction);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost(), Authorize(Roles = "Admin")]
        public async Task<ActionResult<Transaction>> PostTransaction(TransactionPostDto request)
        {
            try
            {
                User? user = await _context.User
                    .FindAsync(request.UserId);

                if (user == null)
                {
                    return BadRequest("User with this {userId} ID does not exist");
                }

                Transaction thisTransaction = new()
                {
                    UserId = request.UserId,
                    Type = request.Type,
                    Recipient = request.Recipient,
                    TransactionId = request.TransactionId,
                    Sum = request.Sum,
                };

                var newTransaction = _context.Transaction
                    .Add(thisTransaction);

                await _context.SaveChangesAsync();

                return Ok($"transaction with ID {thisTransaction.Id} was added");
            } catch (Exception ex)
            {
                return BadRequest(ex.Message);
            } 
        }

        [HttpDelete("{id}"), Authorize]
        public async Task<ActionResult> DeleteTransaction(int id)
        {
            try
            {
                Transaction? transaction = await _context.Transaction
                    .FindAsync(id);

                if (transaction == null)
                {
                    return BadRequest($"Transaction with ID {id} was not found");
                }

                _context.Transaction
                    .Remove(transaction);
                await _context
                    .SaveChangesAsync();

                return Ok(transaction);
            } catch(Exception ex)
            {
                return NotFound(ex);
            }
        }

        [HttpDelete, Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteTransactionsByIds(int[] ids)
        {
            try
            {
                for (int i = 0; i < ids.Length; i++)
                {
                    Transaction? transaction = await _context.Transaction
                        .FindAsync(ids[i]);

                    if (transaction != null)
                    {
                        _context.Transaction
                            .Remove(transaction);
                        await _context
                            .SaveChangesAsync();
                    }
                    
                }

                return Ok();
            } catch(Exception ex)
            {
                return NotFound(ex);
            }
        }

        [HttpDelete("{userId}"), Authorize]
        public async Task<ActionResult> DeleteUsersTransactions(int userId)
        {
            try
            {
                List<Transaction> transactions = await _context.Transaction
                    .Where(transaction => transaction.UserId == userId)
                    .ToListAsync();

                for (int i = 0; i < transactions.Count; i++)
                {
                    _context.Transaction
                      .Remove(transactions[i]);
                }

                return Ok(transactions);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
