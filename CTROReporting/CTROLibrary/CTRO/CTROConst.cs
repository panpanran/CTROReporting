﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Security.Principal;
using System.Web;

namespace CTROLibrary.CTRO
{
    public class CTROConst
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
                    new DateTime(2018,12,25),
                    new DateTime(2019,1,1),
                    new DateTime(2019,1,21),
                    new DateTime(2019,2,18),
                    new DateTime(2019,5,27),
                    new DateTime(2019,7,4),
                    new DateTime(2019,9,2),
                    new DateTime(2019,10,14),
                    new DateTime(2019,11,11),
                    new DateTime(2019,11,28),
                    new DateTime(2019,12,25)
                };

        //database connection
        //public const string connString = "Server=localhost;Port=5434;User Id=panran;Password=P@nR@n123;Database=pa_ctrpn";
        //public const string connString = "Server=localhost;Port=5434;User Id=dwprod;Password=dwprod_at_ctrp17;Database=dw_ctrpn";
        public const string connString = "Server=localhost;Port=5434;User Id=panran;Password=Prss_1234;Database=dw_ctrpn";
        public const string paconnString = "Server=localhost;Port=5434;User Id=panran;Password=Prss_1234;Database=pa_ctrpn";
        public static string EW_ACCOUNT = "panr2";
        public static string EW_PASSWORD = "Prss_5678";
        public static string PA_ACCOUNT = "panr";
        public static string PA_PASSWORD = "Prss_0345";
        public CTROConst()
        {
            if (string.IsNullOrEmpty(PA_PASSWORD))
            {
                //WindowsIdentity identity = HttpContext.Current.Request.LogonUserIdentity;
                string path = @"C:\Users\" + ConfigurationManager.AppSettings["V_NIHACCOUNT"] + @"\CTROCredentials.txt";
                EW_ACCOUNT = CTROFunctions.ReadTxtFile("EW_ACCOUNT", path);
                EW_PASSWORD = CTROFunctions.ReadTxtFile("EW_PASSWORD", path);
                PA_ACCOUNT = CTROFunctions.ReadTxtFile("PA_ACCOUNT", path);
                PA_PASSWORD = CTROFunctions.ReadTxtFile("PA_PASSWORD", path);
            }
        }
    }
    //public const string connString = "Server=localhost;Port=5434;User Id=copparead;Password=rftev@LIENSuj#43wU;Database=dw_ctrpn";
    //public const string reportpath = @"C:\Users\panr2\Downloads\DataWarehouse\CTRO Report";
}
