using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using LearnWebApi.Models;
using LearnWebApi.Types;
using LearnWebApi.Request_Types;

namespace LearnWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionController : Controller
    {
        private readonly DataContext context;

        public TransactionController(DataContext context)
        {
            this.context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<Transaction>>> GetTransactions()
        {
            try
            {
                var transactions = await this.context.Transaction
                    .ToListAsync();

                return Ok(transactions);
            } catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{userId}")]
        public async Task<ActionResult<List<Transaction>>> GetUserTransactions(int userId) {
            try
            {
                var transactions = await this.context.Transaction
                    .Where(transaction => transaction.UserId == userId)
                    .ToListAsync();

                return Ok(transactions);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost()]
        public async Task<ActionResult<Transaction>> PostTransaction(TransactionPostDto request)
        {
            try
            {
                var user = await this.context.User
                    .FindAsync(request.UserId);

                if (user == null)
                {
                    return BadRequest("User with this {userId} ID does not exist");
                }

                var thisTransaction = new Transaction
                {
                    UserId = request.UserId,
                    Type = request.Type,
                    Recipient = request.Recipient,
                    TransactionId = request.TransactionId,
                    Sum = request.Sum,
                };        

                var newTransaction = this.context.Transaction
                    .Add(thisTransaction);

                await this.context.SaveChangesAsync();

                return Ok($"transaction with ID {thisTransaction.Id} was added");
            } catch (Exception ex)
            {
                return BadRequest(ex.Message);
            } 
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteTransaction(int id)
        {
            try
            {
                var transaction = await this.context.Transaction
                    .FindAsync(id);

                if (transaction == null)
                {
                    return BadRequest($"Transaction with ID {id} was not found");
                }

                this.context.Transaction
                    .Remove(transaction);
                await this.context
                    .SaveChangesAsync();

                return Ok(transaction);
            } catch(Exception ex)
            {
                return NotFound(ex);
            }
        }

        [HttpDelete]
        public async Task<ActionResult> DeleteTransactionsByIds(int[] ids)
        {
            try
            {
                for (int i = 0; i < ids.Length; i++)
                {
                    var transaction = await this.context.Transaction
                        .FindAsync(ids[i]);

                    if (transaction != null)
                    {
                        this.context.Transaction
                            .Remove(transaction);
                        await this.context
                            .SaveChangesAsync();
                    }
                    
                }

                return Ok();
            } catch(Exception ex)
            {
                return NotFound(ex);
            }
        }

        [HttpDelete("{userId}")]
        public async Task<ActionResult> DeleteUsersTransactions(int userId)
        {
            try
            {
                var transactions = await this.context.Transaction
                    .Where(transaction => transaction.UserId == userId)
                    .ToListAsync();

                for (int i = 0; i < transactions.Count; i++)
                {
                    this.context.Transaction
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
