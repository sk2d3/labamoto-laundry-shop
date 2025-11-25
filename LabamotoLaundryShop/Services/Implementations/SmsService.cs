using System;

namespace LabamotoLaundryShop.Services
{
    public static class SmsService
    {
        public static bool SendTest(string message)
        {
            // Replace this with your actual SMS API call (Twilio, Semaphore, etc.)
            // For now, we'll just simulate success
            Console.WriteLine("Sending SMS: " + message);
            return true;
        }
    }
}