using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PostOffice
{
    public partial class Pochta_Clients
    {
        public override string ToString()
        {
            return $"{lastname} {name} {patronymic}".Trim();
        }
        public string FIO
        {
            get
            {
                return $"{lastname} {name} {patronymic}".Trim();
            }
        }
    }
}