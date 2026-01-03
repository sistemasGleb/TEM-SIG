

namespace BusinessEntity
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    public class ExecSQL
    {
        public int intError { get; set; }
        public string strError { get; set; }
        public int intQtyReg { get; set; }
        public string strInfAdic1 { get; set; }
        public string strInfAdic2 { get; set; }
        public string strInfAdic3 { get; set; }

        public ExecSQL()
        {
            this.intError = 0;
            this.strError = string.Empty;
            this.intQtyReg = 0;
            this.strInfAdic1 = string.Empty;
            this.strInfAdic2 = string.Empty;
            this.strInfAdic3 = string.Empty;
        }
    }
}
