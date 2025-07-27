namespace EventHUB.Models.Entities
{
    public class Financial
    {
        public int Id { get; set; }
        public string EventName { get; set; }
        public DateTime EventDate { get; set; }
        public decimal Revenue { get; set; }
        public decimal Expenses { get; set; }
        public string Status { get; set; }
    }
}
