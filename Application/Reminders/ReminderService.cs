using Hangfire;

namespace Departments.Application.Reminders
{
    public class ReminderService
    {
        private readonly IBackgroundJobClient _backgroundJobClient;

        public ReminderService(IBackgroundJobClient backgroundJobClient)
        {
            _backgroundJobClient = backgroundJobClient;
        }

        public string SetReminder(ReminderDto reminder)
        {

            if(reminder.TimeToSend < DateTime.Now)
            {
                throw new BadHttpRequestException("Cannot set a reminder in the past");
            }

            return _backgroundJobClient.Schedule(() =>
                    SendEmail(reminder.Title), reminder.TimeToSend);
        }

        public void SendEmail(string title)
        {
            Console.WriteLine($"Sent email {title}");
        }

    }
}
