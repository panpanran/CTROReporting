using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace Attitude_Loose.Test
{
    public class CTRPConst
    {
        //holiday
        public static DateTime[] Holidays = new DateTime[]{
                    new DateTime(2018,1,1),
                    new DateTime(2018,1,15),
                    new DateTime(2018,2,19),
                    new DateTime(2018,5,28),
                    new DateTime(2018,7,4),
                    new DateTime(2018,9,3),
                    new DateTime(2018,9,3),
                    new DateTime(2018,10,8),
                    new DateTime(2018,11,12),
                    new DateTime(2018,11,22),
                    new DateTime(2018,12,25)
                };

        //database connection
        //public const string connString = "Server=localhost;Port=5434;User Id=panran;Password=P@nR@n123;Database=pa_ctrpn";
        //public const string connString = "Server=localhost;Port=5434;User Id=dwprod;Password=dwprod_at_ctrp17;Database=dw_ctrpn";
        public const string connString = "Server=localhost;Port=5434;User Id=panran;Password=Prss_1234;Database=dw_ctrpn";
        //public const string connString = "Server=localhost;Port=5434;User Id=copparead;Password=rftev@LIENSuj#43wU;Database=dw_ctrpn";
    }
}