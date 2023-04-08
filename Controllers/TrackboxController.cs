using Microsoft.AspNetCore.Mvc;

public class TrackboxController : Controller
{
    private readonly IEmailSender emailSender;

    public TrackboxController(IEmailSender emailSender)
    {
        this.emailSender = emailSender;
    }

    [HttpPost]
    public async Task<IActionResult> Index(string email, string subject, string message)
    {
        await emailSender.SendEmailAsync(email, subject, message);
        return View();
    }
}