using BusinessEntity;

namespace BusinessImpl
{
    public class SetupDataAccessImpl
    {
        public BusinessEntity.DataSetSQL ListSetupItems()
        {
            return new DataSetSQL();
        }
    }
}
