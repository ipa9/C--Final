using Microsoft.AspNetCore.Mvc;
using PersonManagementApi.Data;
using PersonManagementApi.Models;
using Microsoft.Extensions.Logging;


namespace PersonManagementApi.Controllers
{
    [ApiController]
    [Route("Api/[controller]")]
    public class PersonsController : ControllerBase
    {
        private readonly PersonDbContext _dbContext;
        private readonly ILogger<PersonsController> _logger;

        public PersonsController(PersonDbContext dbContext, ILogger<PersonsController> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult AddPerson([FromBody] Person person)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _dbContext.Persons.Add(person);
            _dbContext.SaveChanges();
            return Ok();
        }

        [HttpGet("Persons")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetPersons()
        {
            var persons = _dbContext.Persons.ToList();
            return Ok(persons);
        }

        [HttpGet("Person/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetPersonById(int id)
        {
            var person = _dbContext.Persons.Find(id);
            if (person == null)
            {
                return NotFound();
            }
            return Ok(person);
        }

        [HttpPut("Person/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult UpdatePerson(int id, [FromBody] Person updatedPerson)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingPerson = _dbContext.Persons.Find(id);
            if (existingPerson == null)
            {
                return NotFound();
            }

            existingPerson.Name = updatedPerson.Name;
            existingPerson.Email = updatedPerson.Email;
            existingPerson.PhoneNumber = updatedPerson.PhoneNumber;
            existingPerson.DateOfBirth = updatedPerson.DateOfBirth;

            _dbContext.SaveChanges();
            return Ok();
        }

        [HttpDelete("Person/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult DeletePerson(int id)
        {
            var person = _dbContext.Persons.Find(id);
            if (person == null)
            {
                return NotFound();
            }

            _dbContext.Persons.Remove(person);
            _dbContext.SaveChanges();
            return Ok();
        }

        [HttpGet("Persons/find/name/{name}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult FindPersonsByName(string name)
        {
            try
            {
                var persons = _dbContext.Persons.Where(p => p.Name == name).ToList();
                return Ok(persons);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while finding persons by name.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
            }
        }

        [HttpGet("Persons/find/email/{email}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult FindPersonByEmail(string email)
        {
            try
            {
                var person = _dbContext.Persons.FirstOrDefault(p => p.Email == email);
                if (person == null)
                {
                    return NotFound();
                }
                return Ok(person);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while finding a person by email.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
            }
        }
        private void InitializeDatabase()
        {
            // Add some sample persons
            if (!_dbContext.Persons.Any())
            {
                _dbContext.Persons.AddRange(
                    new Person { Name = "John Doe", Email = "john.doe@example.com", PhoneNumber = "555-1234", DateOfBirth = new DateTime(1985, 5, 15) },
                    new Person { Name = "Jane Smith", Email = "jane.smith@example.com", PhoneNumber = "555-5678", DateOfBirth = new DateTime(1990, 8, 22) }
                // Add more sample persons as needed
                );
                _dbContext.SaveChanges();
            }
        }

        [HttpGet("InitializeDatabase")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult InitializeDatabaseAction()
        {
            InitializeDatabase();
            return Ok("Database initialized with sample data");
        }
    }
}
