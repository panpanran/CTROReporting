using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Attitude_Loose.Test
{
    public class CTRPConst
    {
        //database connection
        public const string connString = "Server=localhost;Port=5434;User Id=dwprod;Password=dwprod_at_ctrp17;Database=dw_ctrpn";
        //turnround
        public const string turnround_template_file = @"C:\Users\panr2\Downloads\DataWarehouse\Turnround Report\Monthly Turnaround Template.xlsx";
        public const string turnround_original_file = @"C:\Users\panr2\Downloads\DataWarehouse\Turnround Report\Code\original_sql.txt";
        public const string turnround_amendment_file = @"C:\Users\panr2\Downloads\DataWarehouse\Turnround Report\Code\amendment_sql.txt";
        public const string turnround_abbreviated_file = @"C:\Users\panr2\Downloads\DataWarehouse\Turnround Report\Code\abbreviated_sql.txt";
        public const string turnround_savepath = @"C:\Users\panr2\Downloads\DataWarehouse\Turnround Report\Turnround Report";

    }
}