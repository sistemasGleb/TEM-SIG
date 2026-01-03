using BusinessEntity;
using DataAccessImpl;

namespace BusinessImpl
{
    public class UsuarioBusinessImpl
    {
        private readonly UsuarioDataAccessImpl usuarioDataAccessImpl = new UsuarioDataAccessImpl();

        public DataSetSQL ListUser(string strCurrentUser)
        {
            return usuarioDataAccessImpl.ListUser(strCurrentUser);
        }
        public DataSetSQL DeleteUser(int intUsuario, string strCurrentUser)
        {
            return usuarioDataAccessImpl.DeleteUser(intUsuario, strCurrentUser);
        }
        public DataSetSQL CreateUser(UsuarioBusinessEntity.UsuarioViewModel collection, string strCurrentUser)
        {
            return usuarioDataAccessImpl.CreateUser(collection, strCurrentUser);
        }

    }
}
