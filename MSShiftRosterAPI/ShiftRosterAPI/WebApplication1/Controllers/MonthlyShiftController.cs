using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebRosterAPI.Models;
using Microsoft.Azure.Cosmos;
using System.Reflection;
using Newtonsoft.Json;
using System.Security.Cryptography.Xml;

namespace WebRosterAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MonthlyShiftController : Controller
    {
        public CosmosClient _cosmosClient;
        //public string _databaseName = "msshiftrosterdb";
        //public string _container = "monthlyshiftdetails";

        public MonthlyShiftController (CosmosClient cosmosClient)
        { 
            _cosmosClient = cosmosClient;
        }

        [HttpGet("monthlyrosterdetails")]
        public async Task<IActionResult> GetMonthlyShift()
        {
            try
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
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.ServiceUnavailable)
            {
                // Retry logic can be added here
                return StatusCode(StatusCodes.Status503ServiceUnavailable, "Service Unavailable");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal Server Error: {ex.Message}");
            }
        }

        [HttpPost("SubmitMonthlyData")]
        public async Task<IActionResult> SubmitMonthlyData([FromBody] List<MonthlyShift> monthdata)
        {
            List<MonthlyShift> lsShift = new List<MonthlyShift>();
            List<MonthShiftDetails> lsmonthlydetails= new List<MonthShiftDetails>();
            try
            {
                DateTime currentDate = DateTime.Now;
                int year = currentDate.Year;
                int month = currentDate.Month;

                DateTime firstDayOfMonth = new DateTime(year, month, 1);

                DateTime lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

                // Connect to the database and container
                var database = _cosmosClient.GetDatabase("msshiftrosterdb");
                var container = database.GetContainer("monthlyshiftdetails");
                foreach (var item in monthdata)
                {                    
                    foreach (var data in item._monthShiftData)
                    {
                         for (DateTime date = firstDayOfMonth; date <= lastDayOfMonth; date = date.AddDays(1))
                         {
                               string formattedDate = date.ToString("dd MMMM yyyy");
                               data.Date = formattedDate;
                                lsmonthlydetails.Add(new MonthShiftDetails
                                {
                                    Date = data.Date,
                                    Shift = date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday ? "NA" : data.Shift,
                                    OnCall = date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday ? "NA" : data.OnCall
                                });
                         }
                    }
  
                    var entity = new MonthlyShift
                    {
                        id = Guid.NewGuid().ToString(),
                        Location = item.Location,
                        Name = item.Name,
                        _monthShiftData = lsmonthlydetails
                    };

                    lsShift.Add(entity);
                }
                
                foreach (var entity in lsShift)
                {
                  
                    await container.CreateItemAsync(entity); 
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
