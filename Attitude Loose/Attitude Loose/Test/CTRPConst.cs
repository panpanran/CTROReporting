using System;
using System.Collections.Generic;
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
        public const string connString = "Server=localhost;Port=5434;User Id=copparead;Password=copparead_at_ctrp1;Database=pa_ctrpn";
        //turnround
        public const string turnround_template_file = @"C:\Users\panr2\Downloads\DataWarehouse\Template Report\Monthly Turnaround Template.xlsx";
        public const string turnround_original_file = @"C:\Users\panr2\Downloads\DataWarehouse\Turnround Report\Code\Version 2\original_sql.txt";
        public const string turnround_amendment_file = @"C:\Users\panr2\Downloads\DataWarehouse\Turnround Report\Code\Version 2\amendment_sql.txt";
        public const string turnround_abbreviated_file = @"C:\Users\panr2\Downloads\DataWarehouse\Turnround Report\Code\Version 2\abbreviated_sql.txt";
        public const string turnround_savepath = @"C:\Users\panr2\Downloads\DataWarehouse\Turnround Report\Turnround Report";
    }
}