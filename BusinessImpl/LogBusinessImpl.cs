using BusinessEntity;
using DataAccessImpl;

namespace BusinessImpl
{
    public class LogBusinessImpl
    {
        private readonly LogDataAccessImpl logDataAccessImpl = new LogDataAccessImpl();

        public void _AddNewLog(LogBusinessEntity form)
        {
            logDataAccessImpl.AddNewLog(form);
        }
    }
}
