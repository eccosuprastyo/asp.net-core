using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ExampleJWT.Models
{
    public class AccessToken
    {
        [Key]
        public int id { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string token { get; set; }
        public DateTime? dateexpd { get; set; }
    }
}
