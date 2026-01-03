
namespace BusinessEntity
{
    using System.Collections.Generic;
    public class DashboardBusinessEntity
    {
        public class RootObject
        {
            public List<Kpi> listKpi { get; set; }
        }

        public class Kpi
        {
            public int id_kpi { get; set; }
            public string nom_kpi { get; set; }
            public bool estado_kpi { get; set; }
            public int valor_kpi { get; set; }
            public List<Indicador> indicdores { get; set; }
        }
        public class Indicador
        {
            public int id_kpi { get; set; }
            public int id_indicador { get; set; }
            public string nom_indicador { get; set; }
            public bool ind_activo { get; set; }
            public int ind_valor { get; set; }
        }
    }
}