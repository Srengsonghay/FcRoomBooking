namespace FcRoomBooking.Constants
{
    public static class Permissions
    {
        public static List<string> GeneratePermissionsForModule(string module)
        {
            return new List<string>()
        {
            $"Permissions.{module}.Create",
            $"Permissions.{module}.View",
            $"Permissions.{module}.Edit",
            $"Permissions.{module}.Delete",
        };
        }

        public static class Room
        {
            public const string View = "Permissions.Room.View";
            public const string Create = "Permissions.Room.Create";
            public const string Edit = "Permissions.Room.Edit";
            public const string Delete = "Permissions.Room.Delete";
        }
        public static class Booking
        {
            public const string View = "Permissions.Booking.View";
            public const string Create = "Permissions.Booking.Create";
            public const string Edit = "Permissions.Booking.Edit";
            public const string Delete = "Permissions.Booking.Delete";
        }
        public static class User
        {
            public const string View = "Permissions.User.View";
            public const string Create = "Permissions.User.Create";
            public const string Edit = "Permissions.User.Edit";
            public const string Delete = "Permissions.User.Delete";
        }
    }
}
