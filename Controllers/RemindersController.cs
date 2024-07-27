using Departments.Application.Reminders;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Departments.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class RemindersController : ControllerBase
    {
        private readonly ReminderService _reminderService;

        public RemindersController(ReminderService reminderService)
        {
            _reminderService = reminderService;
        }

        [HttpPost]
        public async Task<IActionResult> SetReminder(
            [FromBody] ReminderDto reminder)
        {
            try
            {
                var jobId = _reminderService.SetReminder(reminder);
                
                return Ok(jobId);
            }
            catch (BadHttpRequestException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
