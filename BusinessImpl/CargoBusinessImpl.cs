using BusinessEntity;
using DataAccessImpl;
using System.Collections.Generic;

namespace BusinessImpl
{
    public class CargoBusinessImpl
    {
        private readonly CargoDataAccessImpl cargoDataAccessImpl = new CargoDataAccessImpl();

        public DataSetSQL ListAll(string strCurrentUser = "")
        {
            return cargoDataAccessImpl.ListAll(strCurrentUser);
        }

        public DataSetSQL ListById(string strCurrentUser, int iPerfil)
        {
            return cargoDataAccessImpl.ListById(strCurrentUser, iPerfil);
        }

        public DataSetSQL fncDevolverMenus(int id)
        {
            return cargoDataAccessImpl.fncDevolverMenus(id);
        }
        public List<MenuBusinessEntity.Menu> fncDevolverSubMenus(int strCodigoMenu, int strCodigoSubMenu, int? intCodigoCargo = 0)
        {
            return cargoDataAccessImpl.fncDevolverSubMenus(strCodigoMenu, strCodigoSubMenu, intCodigoCargo);
        }
        public bool fncGuardarCargo(BusinessEntity.CargoBusinessEntity.Cargo cargo)
        {
            return cargoDataAccessImpl.fncGuardarCargo(cargo);
        }

        public DataSetSQL DeleteCargo(int intIdCargo, string strCurrentUser)
        {
            return cargoDataAccessImpl.DeleteCargo(intIdCargo, strCurrentUser);
        }
    }
}
