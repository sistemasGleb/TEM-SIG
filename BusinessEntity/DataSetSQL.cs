using System.Data;

namespace BusinessEntity
{
    public class DataSetSQL
    {
        public int intError { get; set; }
        public string strError { get; set; }
        public int intQtyReg { get; set; }
        public DataSet dsSQL { get; set; }

        public DataSetSQL()
        {
            this.intError = 0;
            this.intQtyReg = 0;
            this.strError = string.Empty;
            this.dsSQL = new DataSet();
        }

    }
}
