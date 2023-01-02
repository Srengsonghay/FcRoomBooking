namespace FcRoomBooking.Models.ViewModel
{
    public class EventViewModel
    {
        public int Id { get; set; }
        public string title { get; set; }
        public string start { get; set; }
        public string end { get; set; } 
        public bool allDay { get; set; }
        public string detail { get; set; }
        public string color { get; set; }
        public string display { get; set; }
        public string bookby { get; set; }
        public string room { get; set; }
    }
}
