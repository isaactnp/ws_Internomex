using System;
using System.Collections.Generic;
using System.Text;

namespace ws_Internomex.Model
{


    public class Rootobject
    {
        public RESULT RESULT { get; set; }
    }

    public class RESULT
    {
        public COMDATA[] COMDATA { get; set; }
    }

    public class COMDATA
    {
        public string idSolicitudDeComision { get; set; }
        public string fechaSolicitudDeComision { get; set; }
        public string nombreComisionista { get; set; }
        public string rfc { get; set; }
        public string puesto { get; set; }
        public string cliente { get; set; }
        public string desarrollo { get; set; }
        public string cluster { get; set; }
        public string lote { get; set; }
        public string superficie { get; set; }
        public string precioNeto { get; set; }
        public string totalComision { get; set; }
        public string porcentajeComision { get; set; }
        public string entradaVenta { get; set; }
        public string esquema { get; set; }
        public string cobro { get; set; }
        public string xml { get; set; }
        public string idPago { get; set; }
    }


}
