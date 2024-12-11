using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace LiftCars.Model
{
    public class ModelCustomers
    {
        [SQLite.Table("Customers")]
        public class Customer
        {
            [PrimaryKey, AutoIncrement]
            public int Id { get; set; }

            [Unique]
            public string CustomerName { get; set; }

            public string Password { get; set; }

            public string Email { get; set; }
            public string RoleCode { get; set; }    
        }
    }
}
