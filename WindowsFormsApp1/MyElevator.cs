using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    public class MyElevator
    {
        public string status { get; set; }
        public int currentFloor { get; set; }
        public string doorstatus { get; set; }
        public MyElevator() { currentFloor = 1; }
        public void GoingUp()
        {
            currentFloor++;
        }
        public void GoingDown()
        {
            currentFloor--;
        }

    }
}
