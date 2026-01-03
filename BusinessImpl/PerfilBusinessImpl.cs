using BusinessEntity;
using DataAccessImpl;
using System.Collections.Generic;
using static BusinessEntity.PerfilBusinessEntity;

namespace BusinessImpl
{
    public class PerfilBusinessImpl
    {
        private readonly PerfilDataAccessImpl perfilDataAccessImpl = new PerfilDataAccessImpl();

        public DataSetSQL ListAll(string strCurrentUser = "")
        {
            return perfilDataAccessImpl.ListAll(strCurrentUser);
        }
        public DataSetSQL ListById(string strCurrentUser, int iPerfil)
        {
            return perfilDataAccessImpl.ListById(strCurrentUser, iPerfil);
        }
        public DataSetSQL fncDevolverMenus(int id)
        {
            return perfilDataAccessImpl.fncDevolverMenus(id);
        }
        public List<MenuBusinessEntity.Menu> fncDevolverSubMenus(int strCodigoMenu, int strCodigoSubMenu, int? intCodigoPerfil = 0)
        {
            return perfilDataAccessImpl.fncDevolverSubMenus(strCodigoMenu, strCodigoSubMenu, intCodigoPerfil);
        }

        public bool fncGuardarPerfil(Perfil perfil)
        {
            return perfilDataAccessImpl.fncGuardarPerfil(perfil);
        }

        public DataSetSQL DeletePerfil(int intIdPerfil, string strCurrentUser)
        {
            return perfilDataAccessImpl.DeletePerfil(intIdPerfil, strCurrentUser);
        }

        public DataSetSQL ListAsignedById(string strCurrentUser, int iPerfil)
        {
            return perfilDataAccessImpl.ListAsignedById(strCurrentUser, iPerfil);
        }
    }
}
