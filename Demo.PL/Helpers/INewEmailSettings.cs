using Demo.DAL.Models;

namespace Demo.PL.Helpers
{
    public interface INewEmailSettings
    {

        public void SendEmail(NewEmail email);
    }
}
