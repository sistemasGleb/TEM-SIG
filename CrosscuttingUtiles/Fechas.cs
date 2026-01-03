using System;
using System.Globalization;


namespace CrosscuttingUtiles
{
    public class Fechas
    {
        /// <summary>
        /// Función que devuelve un string 'YYYYMMDD' de ISO Calendar dates.
        /// El parámetro de entrada es un valor Date o Datetime.
        /// </summary>
        public static string IsoStringCalendarDate(string data)
        {
            try
            {
                DateTime myDate = DateTime.ParseExact(data, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                return myDate.ToString("yyyyMMdd");
            }
            catch { return "19000101"; }
        }
    }
}
