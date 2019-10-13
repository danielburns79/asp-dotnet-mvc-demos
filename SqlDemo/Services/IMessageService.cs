using System.Threading.Tasks;

namespace SqlDemo.Services
{
    public interface IMessageService
    {
        Task Send(string email, string subject, string message);
    }
}