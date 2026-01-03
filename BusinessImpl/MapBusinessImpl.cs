using BusinessEntity;
using DataAccessImpl;
using System.Collections.Generic;
using System.Data;

namespace BusinessImpl
{
    public class MapBusinessImpl
    {
        private readonly MapDataAccessImpl mapDataAccessImpl = new MapDataAccessImpl();

        public DataSetSQL GetAllMarkers(string strCurrentUser, List<int> listElementos )
        {
            return mapDataAccessImpl.GetAllMarkers(strCurrentUser, listElementos);
        }

        /// <summary>
        /// Metodo que devuelve el detalle de un elemento 'sp_app_extrae_marcador_detalle'
        /// </summary>
        /// <param name="strCurrentUser">Usuario conectado</param>
        /// <param name="idTipoMarker">Id Tipo Elemento</param>
        /// <param name="idMarker">Id Elemento</param>
        /// <returns>El método devuelve un Dataset</returns>
        public DataSetSQL GetMarkerDetail(string strCurrentUser, int idTipoMarker,int idMarker)
        {
            return mapDataAccessImpl.GetMarkerDetail(strCurrentUser, idTipoMarker, idMarker);
        }
        public DataSetSQL UpdateUserPreferences(string strCurrentUser,DataTable configrationTable, DataTable componentsTable)
        {
            return mapDataAccessImpl.UpdateUserPreferences(strCurrentUser, configrationTable, componentsTable);
        }
    }
}
