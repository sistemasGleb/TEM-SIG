namespace BusinessImpl
{
    using BusinessEntity;
    using DataAccessImpl;

    public class DashboardBusinessImpl
    {
        private readonly DashboardDataAccessImpl dashboardDataAccessImpl = new DashboardDataAccessImpl();

        public DataSetSQL _getdashboard()
        {
            return dashboardDataAccessImpl.getDashboard();
        }
    }
}
