using BusinessEntity;
using DataAccessImpl;
using System.Data;

namespace BusinessImpl
{
    public class ElementosBusinessImpl
    {
        private readonly ElementoDataAccessImpl elementoDataAccessImpl = new ElementoDataAccessImpl();

        public DataSetSQL AllInfoElement(string strUsuario, short IdTipo, int IdElemento)
        {
            return elementoDataAccessImpl.AllInfoElement(strUsuario, IdTipo, IdElemento);
        }

        public DataSetSQL CreateEditElements(string strUsuario, ParametrosBusinessEntity collection)
        {
            return elementoDataAccessImpl.CreateEditElements(strUsuario,  collection);
        }
        public DataSetSQL DeleteElementImage(int intImg, string strCurrentUser)
        {
            return elementoDataAccessImpl.DeleteElementImage(intImg, strCurrentUser);
        }

        public DataSetSQL AddImageElements(string strUsuario, DataTable dt)
        {
            return elementoDataAccessImpl.AddImageElements( strUsuario, dt);
        }
    }
}