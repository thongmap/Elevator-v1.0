using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    public class UpDown
    {
        public int floornumber { get; set; }
        public string UporDown { get; set; }
        public UpDown()
        { }
        public UpDown(int floornu,string updown)
        {
            floornumber = floornu;
            UporDown = updown;
        }
    }
}
