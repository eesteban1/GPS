using Screen.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace Screen
{
    public class GpsDao
    {
        public void Guardarposisicon(GPS gps )
        {
            SqlConnection con = new SqlConnection("data source=198.71.226.6; initial catalog=facturaciondb;user id=us1;password=Ees2018$");
            con.Open();
            SqlCommand cmd = new SqlCommand("usp_InsertPosicion", con);
            cmd.Parameters.Add("@img", System.Data.SqlDbType.Binary);
            cmd.Parameters["@img"].Value = gps.Imagen;
            cmd.Parameters.AddWithValue("@lon", gps.longitud);
            cmd.Parameters.AddWithValue("@lat", gps.Latitud);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.ExecuteNonQuery();
            con.Close();
        }

    }
}
