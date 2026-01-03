using BusinessEntity;
using DataAccessImpl;

namespace BusinessImpl
{
    public class PasswordBusinessImpl
    {
        private readonly PasswordDataAccessImpl passwordDataAccessImpl = new PasswordDataAccessImpl();

        public DataSetSQL PasswordRecovery(PasswordBusinessEntity collection)
        {
            return passwordDataAccessImpl.PasswordRecovery(collection);
        }

        public DataSetSQL ListAll(string strCurrentUser)
        {
            return passwordDataAccessImpl.ListAll(strCurrentUser);
        }

        public DataSetSQL PasswordUpdate(PasswordBusinessEntity collection)
        {
            return passwordDataAccessImpl.PasswordUpdate(collection);
        }
    }
}

