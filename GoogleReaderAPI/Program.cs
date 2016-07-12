namespace GoogleReaderAPI
{
    using System;

    class Program
    {
        static void Main(string[] args)
        {
            //  TODO: enter your google account information here
            string email = "";
            string password = "";

            if (string.IsNullOrEmpty(password))
            {
                Console.WriteLine("Enter password");
                password = Console.ReadLine();
            }

            using (Reader reader = Reader.CreateReader(email, password, "scroll") as Reader)
            {
                Console.WriteLine("Getting...");
                // test any reader command here:
                var unreadCount = reader.GetUnreadCount();
                Console.ReadLine();
            }
        }


    }
}