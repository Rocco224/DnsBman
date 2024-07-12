using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DnsBman.Data;
using DnsBman.Models;
using DnsBman.Services;
using DnsBman.Services.ApiKey;
using Serilog;
using Microsoft.AspNetCore.Cors;

namespace DnsBman.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    [ApiKey]
    public class CustomersController : ControllerBase
    {
        private readonly DnsBmanContext _context;
        private readonly BmanCustomerService _bmanCustomerService;

        public CustomersController(DnsBmanContext context, BmanCustomerService bmanCustomerService)
        {
            _context = context;
            _bmanCustomerService = bmanCustomerService;
        }

        // GET: api/Customers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Customer>>> GetCustomers()
        {
            try
            {
                return await _context.Customers.ToListAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Errore durante il recupero della lista dei clienti");
                return Problem(ex.Message);
            }
        }

        // GET: api/Customers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Customer>> GetCustomer(int id)
        {
            try
            {
                var customer = await _context.Customers.FindAsync(id);

                if (customer == null)
                {
                    return NotFound("Error: The customer with the passed ID does not exist");
                }

                return customer;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Errore durante il recupero del cliente con ID {Id}", id);
                return Problem(ex.Message);
            }
        }

        // POST: api/Customers
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Customer>> PostCustomer(Customer customer) // refactor?
        {
            try
            {
                if (customer.Name == null || customer.ValueBmanIt == null)
                {
                    return BadRequest("Error: Name or ValueBmanIt are null");
                }

                customer = await _bmanCustomerService.AddCustomer(customer);

                _context.Customers.Add(customer);
                await _context.SaveChangesAsync();

                return CreatedAtAction("PostCustomer", customer);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Errore durante la creazione del cliente {@Customer}", customer);
                return Problem(ex.Message);
            }
        }

        // PUT: api/Customers/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCustomer(int id, Customer modifiedCustomer) // refactor?
        {
            try
            {
                var customer = await _context.Customers.FirstOrDefaultAsync(customer => customer.Id == id);

                if (customer == null)
                {
                    return NotFound("Error: The customer with the passed ID does not exist");
                }

                customer = await _bmanCustomerService.EditCustomer(customer, modifiedCustomer);

                _context.Entry(customer).State = EntityState.Modified;

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CustomerExists(id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                customer = await _context.Customers.FirstOrDefaultAsync(customer => customer.Id == id);

                return Ok(customer);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Errore durante la modifica del cliente con ID {Id}", id);
                return Problem(ex.Message);
            }
        }

        // DELETE: api/Customers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            try
            {
                var customer = await _context.Customers.FindAsync(id);

                if (customer == null)
                {
                    return NotFound("Error: The customer with the passed ID does not exist");
                }

                if (await _bmanCustomerService.IsCustomerEliminated(customer))
                {
                    _context.Customers.Remove(customer);
                    await _context.SaveChangesAsync();

                    return Ok(customer);
                }

                throw new Exception();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Errore durante l'eliminazione del cliente con ID {Id}", id);
                return Problem(ex.Message);
            }
        }
        private bool CustomerExists(int id)
        {
            return _context.Customers.Any(e => e.Id == id);
        }
    }
}
