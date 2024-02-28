using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebRosterAPI.Models;
using Microsoft.Azure.Cosmos;
using System.Reflection;
using System.ComponentModel;

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
        public async Task<IActionResult> GetFortnightDate()
        {

           
            var database = _cosmosClient.GetDatabase("msshiftrosterdb");
            var container = database.GetContainer("monthlyshiftdetails");

            var monthlyShifts = new List<MonthlyShift>();

            string sqlQueryText = "SELECT * FROM c";

            QueryDefinition queryDefinition = new QueryDefinition(sqlQueryText);

            var resultSetIterator = container.GetItemQueryIterator<MonthlyShift>(
                queryDefinition: queryDefinition);

            while (resultSetIterator.HasMoreResults)
            {
                var response = await resultSetIterator.ReadNextAsync();
                monthlyShifts.AddRange(response);
            }

            return Ok(monthlyShifts);
        }
       
        [HttpPost("AddEmployee")]
        public async Task< IActionResult> UpdateEmployeeData([FromBody] List<MonthlyShift> shiftdata)
        {
            try
            {

                // Connect to the database and container
                var database = _cosmosClient.GetDatabase("msshiftrosterdb");
                var container = database.GetContainer("EmployeeDetails");
                // Insert the data into Cosmos DB
                foreach (var item in shiftdata)
                {
                    //item.id = Guid.NewGuid().ToString();
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

