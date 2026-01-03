using DataAccessImpl;
using BusinessEntity;

namespace BusinessImpl
{
    public class CategoriaBusinessImpl
    {
        private readonly CategoriaDataAccessImpl categoriaDataAccessImpl = new CategoriaDataAccessImpl();

        public DataSetSQL ListAll(string strCurrentUser)
        {
            return categoriaDataAccessImpl.ListAll(strCurrentUser);
        }

        public DataSetSQL Create(CategoriaBusinessEntity collection, string strCurrentUser = "")
        {
            return categoriaDataAccessImpl.Create(collection, strCurrentUser);
        }
    }
}
