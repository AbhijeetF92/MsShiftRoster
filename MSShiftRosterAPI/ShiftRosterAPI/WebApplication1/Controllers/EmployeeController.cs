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
        //public  CosmosClient _cosmosClient;
        //public  string _databaseName = "YourDatabaseName";
        //public  string _containerName = "YourContainerName";

        //public EmployeeController(CosmosClient cosmosClient)
        //{
        //    _cosmosClient = cosmosClient;
        //}
        [HttpGet("fortnight")]
        public IActionResult GetFortnightDate()
        {
            
            MonthDate emp1 = new MonthDate() { name = "Abhijeet" };
            MonthDate emp2 = new MonthDate() { name = "Anish" };
            MonthDate emp3 = new MonthDate() { name = "Yogesh" };

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
                //var database = _cosmosClient.GetDatabase("");
                //var container = database.GetContainer("");
                foreach (var _mondate in empdata)
                {
                    string name = _mondate.name;
                    string selectedLetter = _mondate.selectedLetter;
                    string formattedDate = _mondate.formattedDate;
                }

                


                // Insert the data into Cosmos DB
               // var response = _mondate;//await container.CreateItemAsync(empdata);

                return Ok();
            }
            catch (CosmosException ex)
            {
                return StatusCode((int)ex.StatusCode, ex.Message);
            }
        }
    }
    
}

