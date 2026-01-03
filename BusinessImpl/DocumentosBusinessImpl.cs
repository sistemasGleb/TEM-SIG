using BusinessEntity;
using DataAccessImpl;

namespace BusinessImpl
{
    public class DocumentosBusinessImpl
    {
        private readonly DocumentosDataAccessImpl documentosDataAccessImpl = new DocumentosDataAccessImpl();

        public DataSetSQL ListAll(string strCurrentUser)
        {
            return documentosDataAccessImpl.ListAll(strCurrentUser);
        }

        public DataSetSQL DeleteByGuid(string strGUID, string strCurrentUser)
        {
            return documentosDataAccessImpl.DeleteByGuid(strGUID,strCurrentUser);
        }
        public DataSetSQL SaveDocument(DocumentoBusinessEntity collection)
        {
            return documentosDataAccessImpl.SaveDocument(collection);
        }

    }
}
