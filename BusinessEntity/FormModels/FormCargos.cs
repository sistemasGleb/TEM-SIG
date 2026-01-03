using System.Collections.Generic;

namespace BusinessEntity.FormModels
{
    public class FormCargos
    {
        public List<BusinessEntity.CargoBusinessEntity.Cargo> listCargos { get; set; }
        public BusinessEntity.CargoBusinessEntity.Cargo singleCargo { get; set; }
    }
}
