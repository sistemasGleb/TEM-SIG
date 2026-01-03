using System.Collections.Generic;
using BusinessEntity;
using DataAccessImpl;

namespace BusinessImpl
{
    public class MenuBusinessImpl
    {
        private readonly MenuDataAccessImpl menuDataAccessImpl = new MenuDataAccessImpl();

        public DataSetSQL get_Elementos(int intUsuario)
        {
            return menuDataAccessImpl.get_Elementos(intUsuario);
        }

        public List<MenuBusinessEntity.Menu>  fncDevolverMenus(int? intCodigoPerfil = 0)
        {
            return menuDataAccessImpl.fncDevolverMenus(intCodigoPerfil);
        }

        public DataSetSQL fncDevolverMenu(string strCurrentUser)
        {
            return menuDataAccessImpl.fncDevolverMenu(strCurrentUser);
        }

        public DataSetSQL fncDevolverMenuOpciones(string strCurrentUser)
        {
            return menuDataAccessImpl.fncDevolverMenuOpciones(strCurrentUser);
        }
    }
}
