
namespace CrosscuttingUtiles
{
    public class Rut
    {
        /// <summary>
        /// Metodo que devuelve el Rut normalizado con puntos y digito verificador.
        /// </summary>
        /// <param name="rut"> El parámetro requiere el RUN completo incluido el digito verificador (Sin guión)</param>
        /// <returns>El método devuelve un string</returns>
        public static string FormatearRut(string rut)
        {
            var tmp1 = rut.Trim().Replace("-", string.Empty);
            var tmp2 = tmp1.Trim().Replace(".", string.Empty);

            rut = tmp2;

            if (rut.Length == 9 || rut.Length == 8 || rut.Length == 2)
            {
                string[] FRut = new string[4];

                if (rut.Length == 9)
                {
                    FRut[0] = rut.Substring(0, 2);
                    FRut[1] = rut.Substring(2, 3);
                    FRut[2] = rut.Substring(5, 3);
                    FRut[3] = rut.Substring(8, 1);
                }

                if (rut.Length == 8)
                {
                    FRut[0] = rut.Substring(0, 1);
                    FRut[1] = rut.Substring(1, 3);
                    FRut[2] = rut.Substring(4, 3);
                    FRut[3] = rut.Substring(7, 1);
                }

                if (rut.Length == 2)
                {
                    FRut[0] = rut.Substring(0, 1);
                    FRut[1] = rut.Substring(1, 1);

                    rut = (FRut[0] + "-" + FRut[1]);

                    return rut;
                }

                rut = (FRut[0] + "." + FRut[1] + "." + FRut[2] + "-" + FRut[3]);
            }

            return rut;
        }

        public static string GetRutNumerico(string pRut)
        {
            var tmp1 = pRut.Trim().Replace(".", string.Empty);

            pRut = tmp1;

            var _resto = string.Empty;
            try
            {
                int index1 = pRut.IndexOf('-');
                if (index1 != -1)
                {
                    _resto = pRut.Substring(0, index1);
                }
                return _resto;
            }
            catch
            {
                return "0";
            }
        }
        public static string GetRutDigito(string pRut)
        {
            var tmp1 = pRut.Trim().Replace(".", string.Empty);

            pRut = tmp1;

            var _resto = string.Empty;
            try
            {
                int index1 = pRut.LastIndexOf('-');
                if (index1 != -1)
                {
                    _resto = pRut.Substring(index1 + 1);
                }
                return _resto;
            }
            catch
            {
                return "0";
            }
        }

    }
}
