using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebRosterAPI.Models;
using Microsoft.Azure.Cosmos;
using System.Reflection;

namespace WebRosterAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        public CosmosClient _cosmosClient;
        public string _databaseName = "msshiftrosterdb";
        public string _containerName = "EmployeeDetails";

        public EmployeeController(CosmosClient cosmosClient)
        {
            _cosmosClient = cosmosClient;
        }
        [HttpGet("fortnight")]
        public IActionResult GetFortnightDate()
        {
            
            MonthDate emp1 = new MonthDate() { employeeID = "abhijeet.a.fegade@accenure.com" };
            MonthDate emp2 = new MonthDate() { employeeID = "yogeshkumar.salunkhe@accenture.com" };
            MonthDate emp3 = new MonthDate() { employeeID = "anish.m.pillay@accenture.com" };

            List<MonthDate> emp = new List<MonthDate>();

            emp.Add(emp1);
            emp.Add(emp2);
            emp.Add(emp3);


            return Ok(emp);
        }
       
        [HttpPost("AddEmployee")]
        public async Task< IActionResult> UpdateEmployeeData([FromBody] List<MonthDate> empdata)
        {
            try
            {

                // Connect to the database and container
                var database = _cosmosClient.GetDatabase("msshiftrosterdb");
                var container = database.GetContainer("EmployeeDetails");
                // Insert the data into Cosmos DB
                foreach (var item in empdata)
                {
                    item.id = Guid.NewGuid().ToString();
                    await container.CreateItemAsync(item);
                }

                return Ok();
            }
            catch (CosmosException ex)
            {
                return StatusCode((int)ex.StatusCode, ex.Message);
            }
        }
    }
    
}

