using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        static List<int> floor = new List<int>();
        static List<UpDown> outside = new List<UpDown>();
        public MyElevator elevator;
        Thread eleproc;
        Thread elemove;
        static Image down = Image.FromFile(@"img/arrow.png");
        static Image up = Image.FromFile(@"img/up.png");
        static Image close = Image.FromFile(@"img/closeele.jpg");
        static Image thumbc = close.GetThumbnailImage(70, 70, null, IntPtr.Zero);
        static Image open = Image.FromFile(@"img/openele.jpg");
        static Image thumbo = open.GetThumbnailImage(70, 70, null, IntPtr.Zero);
        static int[] floorY = { 426, 360, 292, 224, 160, 94,26 };
        bool move = true;
        bool opendoor = false;
        public Form1()
        {
            InitializeComponent();
            elevator = new MyElevator();
           
            pictureBox1.Image = thumbc;
            
        }


        #region Elevator Processor
        private void ElevatorMove()
        {
            while (true)
            {
                switch (elevator.status)
                {
                    case "Down":

                        if ((floor != null && floor.Count() > 0) || (outside != null && outside.Count > 0))
                        {
                            while (move)
                            {
                                Thread.Sleep(140);
                                pictureBox1.BeginInvoke(new Action(() => { pictureBox1.Top += 2; Invalidate(); }));
                            }
                        }
                                break;
                    case "Up":
                        if ((floor != null && floor.Count() > 0) || (outside != null && outside.Count > 0))
                        {
                            while (move)
                            {
                                Thread.Sleep(140);
                                pictureBox1.BeginInvoke(new Action(() => { pictureBox1.Top -= 2; Invalidate(); }));

                            }
                        }
                        break;
                    default:
                        break;
                }
            }
        }
        private void ElevatorProcess()
        {
            while(true)
            {
                if(floor != null && floor.Count() > 0)
                {
                    if (elevator.currentFloor == floor[0])
                    {
                        if (elevator.currentFloor == 1)
                        {
                            elevator.status = "Up"; status.BeginInvoke(new Action(() => { status.Image = up; }));
                        }
                        if (elevator.currentFloor == 7)
                        {
                            elevator.status = "Down"; status.BeginInvoke(new Action(() => { status.Image = down; }));
                        }
                        changecolor(elevator.currentFloor);
                        if (outside != null && outside.Count > 0)
                        {
                            if (outside.Exists(x => x.floornumber == elevator.currentFloor && x.UporDown.Equals(elevator.status)))
                            {
                                outside.RemoveAll(x => x.floornumber == elevator.currentFloor && x.UporDown.Equals(elevator.status));
                                changecolorout(elevator.currentFloor, elevator.status);
                            }
                        }
                        ElevatorAnimation();
                        floor = floor.Skip(1).ToList();
                       
                        goto sleep;
                    }
                    if (outside != null && outside.Count > 0)
                    {
                        if (outside.Exists(x => x.floornumber == elevator.currentFloor && x.UporDown.Equals(elevator.status)))
                        {
                            changecolorout(elevator.currentFloor, outside[0].UporDown);
                            ElevatorAnimation();
                            outside.RemoveAll(x => x.floornumber == elevator.currentFloor && x.UporDown.Equals(elevator.status));
                            goto sleep;
                        }
                    }
                    if (elevator.currentFloor > floor.Max())
                    {
                        elevator.status = "Down";
                        move = true;
                        status.BeginInvoke(new Action(() => { status.Image = down; }));
                        Thread.Sleep(2700);
                        floor.Sort();
                        floor.Reverse();
                        elevator.GoingDown();
                        Thread.Sleep(1000);
                        label1.BeginInvoke(new Action(() => { label1.Text = elevator.currentFloor.ToString(); }));
                       
                    }
                    if(elevator.currentFloor < floor.Min())
                    {
                        elevator.status = "Up";
                        move = true;
                        status.BeginInvoke(new Action(() => {status.Image = up; }));
                        Thread.Sleep(2700);
                        floor.Sort();
                        elevator.GoingUp();
                        Thread.Sleep(1000);
                        label1.BeginInvoke(new Action(() => { label1.Text = elevator.currentFloor.ToString(); }));
                      
                    }
                   
                }
               
                if((floor == null || floor.Count() == 0) && outside != null && outside.Count > 0)
                {
                    if (elevator.currentFloor == outside[0].floornumber)
                    {
                        changecolorout(elevator.currentFloor, outside[0].UporDown);
                        elevator.status = outside[0].UporDown;
                        status.BeginInvoke(new Action(() => { status.Image = elevator.status.Equals("Up") ? up : down; }));
                        ElevatorAnimation();
                        outside.RemoveAll(x => x.floornumber == elevator.currentFloor && x.UporDown.Equals(elevator.status));
                        goto sleep;
                    }
                    if (elevator.currentFloor > outside.Max(x=>x.floornumber))
                    {
                        elevator.status = "Down";
                        move = true;
                        status.BeginInvoke(new Action(() => { status.Image = down; }));
                        Thread.Sleep(2700);
                        outside=outside.OrderByDescending(x => x.floornumber).ToList();
                        elevator.GoingDown();
                        Thread.Sleep(1000);
                        label1.BeginInvoke(new Action(() => { label1.Text = elevator.currentFloor.ToString(); }));
                    }
                    if (elevator.currentFloor < outside.Min(x => x.floornumber))
                    {
                        elevator.status = "Up";
                        move = true;
                        status.BeginInvoke(new Action(() => {status.Image = up;}));
                        Thread.Sleep(2700);
                        outside=outside.OrderBy(x => x.floornumber).ToList();
                        elevator.GoingUp();
                        Thread.Sleep(1000);
                        label1.BeginInvoke(new Action(() => { label1.Text = elevator.currentFloor.ToString(); }));
                       
                    }
                }
                sleep:
                Thread.Sleep(1000);
                if ((floor == null|| floor.Count() == 0) && (outside == null||outside.Count ==0))
                {
                    elevator.status = null;
                    if(status.Image != null)
                    status.BeginInvoke(new Action(() => {
                        status.Image=null;
                    }));
                }
               
            }
        }
        private void ElevatorAnimation()
        {
            move = false;
            pictureBox1.BeginInvoke(new Action(() => { pictureBox1.Top = floorY[elevator.currentFloor - 1]; Invalidate(); }));
            elevator.doorstatus = "Opening";
            label4.BeginInvoke(new Action(() => { label4.Text = "Opened"; }));
            pictureBox1.BeginInvoke(new Action(() => { pictureBox1.Image = thumbo; Invalidate(); }));
            Thread.Sleep(3000);
            while(opendoor)
            {
                Thread.Sleep(1000);
            }
            elevator.doorstatus = "Closing";
            pictureBox1.BeginInvoke(new Action(() => { pictureBox1.Image = thumbc; Invalidate(); }));
            label4.BeginInvoke(new Action(() => { label4.Text = "Closed"; }));
        }
        #endregion
        private void button9_Click(object sender, EventArgs e)
        {

                eleproc = new Thread(new ThreadStart(ElevatorProcess));
                eleproc.Start();
                elemove = new Thread(new ThreadStart(ElevatorMove));
                elemove.Start();
                label1.Text = "1";
                button9.Enabled = false;
        }
        #region Number Button Function

        private bool FloorPress(int floorno)
        {
            if (floor.Contains(floorno)) return false;
            if (String.IsNullOrEmpty(elevator.status)) return true;
            
            if ((floorno < elevator.currentFloor && elevator.status.Equals("Down")) || (floorno > elevator.currentFloor && elevator.status.Equals("Up")))
            {

                return true;
            }
            return false;
        }
        private void changecolor(int a)
        {
            switch (a)
            {
                case 1:
                    button6.BeginInvoke(new Action(() =>
                    {
                        button6.BackColor = SystemColors.ControlLight;
                    }));
                    break;
                case 2:
                    button5.BeginInvoke(new Action(() =>
                    {
                        button5.BackColor = SystemColors.ControlLight;
                    }));
                    break;
                case 3:
                    button4.BeginInvoke(new Action(() =>
                    {
                        button4.BackColor = SystemColors.ControlLight;
                    }));
                    break;
                case 4:
                    button3.BeginInvoke(new Action(() =>
                    {
                        button3.BackColor = SystemColors.ControlLight;
                    }));
                    break;
                case 5:
                    button10.BeginInvoke(new Action(() =>
                    {
                        button10.BackColor = SystemColors.ControlLight;
                    }));
                    break;
                case 6:
                    button11.BeginInvoke(new Action(() =>
                    {
                        button11.BackColor = SystemColors.ControlLight;
                    }));
                    break;
                case 7:
                    button12.BeginInvoke(new Action(() =>
                    {
                        button12.BackColor = SystemColors.ControlLight;
                    }));
                    break;
                default:
                    a = 0;
                    break;
            }

        }
        private void button6_Click(object sender, EventArgs e)
        {
            if (FloorPress(1))
            { floor.Add(1); button6.BackColor = Color.Red; }
        }
        private void button5_Click(object sender, EventArgs e)
        {
            if (FloorPress(2))
            { floor.Add(2); button5.BackColor = Color.Red; }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (FloorPress(3))
            { floor.Add(3); button4.BackColor = Color.Red; }
        }
        private void button3_Click(object sender, EventArgs e)
        {
            if (FloorPress(4))
            {         floor.Add(4); button3.BackColor = Color.Red;
            }
        }
        private void button10_Click(object sender, EventArgs e)
        {
            if (FloorPress(5))
            {
                floor.Add(5); button10.BackColor = Color.Red;
            }
        }
        private void button11_Click(object sender, EventArgs e)
        {
            if (FloorPress(6))
            {
                floor.Add(6); button11.BackColor = Color.Red;
            }
        }
        private void button12_Click(object sender, EventArgs e)
        {
            if (FloorPress(7))
            {
                floor.Add(7); button12.BackColor = Color.Red;
            }
        }
        #endregion
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            if(eleproc != null)
            eleproc.Abort();
            if (elemove != null)
                elemove.Abort();
        }



        #region Button Up Down click
        private void changecolorout(int a,string b)
        {
            switch (a)
            {
                case 1:
           
                    button23.BeginInvoke(new Action(() =>
                    {
                        button23.BackColor = SystemColors.ControlLight;
                    }));
      
                    break;
                case 2:
                    if(b.Equals("Up"))
                    button21.BeginInvoke(new Action(() =>
                    {
                        button21.BackColor = SystemColors.ControlLight;
                    }));
                    else
                        button22.BeginInvoke(new Action(() =>
                        {
                            button22.BackColor = SystemColors.ControlLight;
                        }));
                    break;
                case 3:
                    if(b.Equals("Up"))
                    button15.BeginInvoke(new Action(() =>
                    {
                        button15.BackColor = SystemColors.ControlLight;
                    }));
                    else
                        button16.BeginInvoke(new Action(() =>
                        {
                            button16.BackColor = SystemColors.ControlLight;
                        }));
                    break;
                case 4:
                    if (b.Equals("Up"))
                        button13.BeginInvoke(new Action(() =>
                        {
                            button13.BackColor = SystemColors.ControlLight;
                        }));
                    else
                        button14.BeginInvoke(new Action(() =>
                        {
                            button14.BackColor = SystemColors.ControlLight;
                        }));
                    break;
                case 5:
                    if (b.Equals("Up"))
                        button17.BeginInvoke(new Action(() =>
                        {
                            button17.BackColor = SystemColors.ControlLight;
                        }));
                    else
                        button18.BeginInvoke(new Action(() =>
                        {
                            button18.BackColor = SystemColors.ControlLight;
                        }));
                    break;
                case 6:
                    if (b.Equals("Up"))
                        button19.BeginInvoke(new Action(() =>
                        {
                            button19.BackColor = SystemColors.ControlLight;
                        }));
                    else
                        button20.BeginInvoke(new Action(() =>
                        {
                            button20.BackColor = SystemColors.ControlLight;
                        }));
                    break;
                case 7:
                    button1.BeginInvoke(new Action(() =>
                    {
                        button1.BackColor = SystemColors.ControlLight;
                    }));
                    break;
                default:
                    a = 0;
                    break;
            }

        }
        private void button15_Click(object sender, EventArgs e)
        {
            outside.Add(new UpDown(3,"Up"));
            button15.BackColor = Color.Red;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            outside.Add(new UpDown(7, "Down"));
            button1.BackColor = Color.Red;
        }
        private void button22_Click(object sender, EventArgs e)
        {
            outside.Add(new UpDown(2, "Down"));
            button22.BackColor = Color.Red;
        }

        private void button19_Click(object sender, EventArgs e)
        {
            outside.Add(new UpDown(6, "Up"));
            button19.BackColor = Color.Red;
        }

        private void button17_Click(object sender, EventArgs e)
        {
            outside.Add(new UpDown(5, "Up"));
            button17.BackColor = Color.Red;
        }

        private void button13_Click(object sender, EventArgs e)
        {
            outside.Add(new UpDown(4, "Up"));
            button13.BackColor = Color.Red;
        }

        private void button23_Click(object sender, EventArgs e)
        {
            outside.Add(new UpDown(1, "Up"));
            button23.BackColor = Color.Red;
        }
        private void button18_Click(object sender, EventArgs e)
        {
            outside.Add(new UpDown(5, "Down"));
            button18.BackColor = Color.Red;
        }
        private void button20_Click(object sender, EventArgs e)
        {
            outside.Add(new UpDown(6, "Down"));
            button20.BackColor = Color.Red;
        }
        private void button14_Click(object sender, EventArgs e)
        {
            outside.Add(new UpDown(4, "Down"));
            button14.BackColor = Color.Red;
        }
        private void button16_Click(object sender, EventArgs e)
        {
            outside.Add(new UpDown(3, "Down"));
            button16.BackColor = Color.Red;
        }

        private void button21_Click(object sender, EventArgs e)
        {
            outside.Add(new UpDown(2, "Up"));
            button21.BackColor = Color.Red;
        }

        #endregion

        private void button7_MouseDown(object sender, MouseEventArgs e)
        {
            opendoor = true;
        }

        private void button7_MouseUp(object sender, MouseEventArgs e)
        {
            opendoor = false;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if(elevator.doorstatus != "Opening")
            floor.Add(elevator.currentFloor);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (eleproc != null)
                eleproc.Abort();
            if (elemove != null)
                elemove.Abort();
            for(int i = 1;i<=7;i++)
            {
                changecolor(i);
                changecolorout(i, "Up");
                changecolorout(i, "Down");
            }
            pictureBox1.Top = floorY[0];
        }
    }
}
