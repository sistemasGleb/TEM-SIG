
using BusinessEntity;
using DataAccessImpl;

namespace BusinessImpl
{
    public class LoginBusinessImpl
    {
        private readonly LoginDataAccessImpl loginDataAccessImpl = new LoginDataAccessImpl();

        public DataSetSQL logOn(LoginBusinessEntity form)
        {
            return loginDataAccessImpl.logOn(form);
        }
    }
}
