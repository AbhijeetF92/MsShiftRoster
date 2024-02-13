namespace WebRosterAPI.Models
{
    //public class Employee
    //{
    //    public string Name { get; set; }


    //    public List<MonthDate> MonthDates { get; set; }
    //}

    public class MonthDate
    {
        public string employeeID { get; set; }
        public string formattedDate { get; set; }

        public string selectedLetter { get; set; }
        public string id { get; set; }
    }
}
