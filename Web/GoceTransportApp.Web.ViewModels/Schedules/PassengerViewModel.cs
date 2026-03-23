namespace GoceTransportApp.Web.ViewModels.Schedules
{
    public class PassengerViewModel
    {
        public string CustomerId { get; set; } = null!;

        public string CustomerName { get; set; } = null!;

        public string TicketId { get; set; } = null!;

        public bool IsBoarded { get; set; }
    }
}
