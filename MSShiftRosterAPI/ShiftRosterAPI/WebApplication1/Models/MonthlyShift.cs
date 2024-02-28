using System.Reflection.Metadata.Ecma335;

namespace WebRosterAPI.Models
{
    public class MonthlyShift
    {
        public string Name { get; set; }
        public string Location { get; set; }
        public string id { get; set; }
        public  List<MonthShiftDetails> _monthShiftData { get; set; }
    }


    public class MonthShiftDetails
    {
        public string Shift { get; set; }
        public string OnCall { get; set; }
        public string Date { get; set; }
    }
}
