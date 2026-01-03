using BusinessEntity;
using DataAccessImpl;

namespace BusinessImpl
{
    public class ConfiguracionBusinessImpl
    {
        private readonly ConfiguracionDataAccessImpl configuracionDataAccessImpl = new ConfiguracionDataAccessImpl();
        public DataSetSQL ListAll(string strCurrentUser, string strKey)
        {
            return configuracionDataAccessImpl.ListAll(strCurrentUser, strKey);
        }
    }
}
