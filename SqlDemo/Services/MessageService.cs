using System.Threading.Tasks;
using System.IO;

namespace SqlDemo.Services
{
    public class FileMessageService : IMessageService
    {
        Task IMessageService.Send(string email, string subject, string message)
        {
            var emailMessage = $"To: {email}\nSubject: {subject}\nMessage: {message}\n\n";
            File.AppendAllText("email.txt", emailMessage);
            return Task.FromResult(0);
        }
    }
}