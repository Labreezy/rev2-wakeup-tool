using System;
using System.Windows.Forms;
using GGXrdWakeupDPUtil.Library;

namespace GGXrdWakeupDPUtil.Test
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private ReversalTool _reversalTool;

        private void button1_Click(object sender, EventArgs e)
        {
            var dummy = _reversalTool.GetDummy();

            MessageBox.Show($@"Current Dummy : {dummy.CharName}");
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            _reversalTool = new ReversalTool();

            _reversalTool.AttachToProcess();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            _reversalTool.Dispose();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //TODO Refactor
            // _reversalTool.PlayReversal();
        }



        private void button2_Click(object sender, EventArgs e)
        {
            _reversalTool.SetInputInSlot(1, textBox1.Text);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            _reversalTool.SetInputInSlot(2, textBox1.Text);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            _reversalTool.SetInputInSlot(3, textBox1.Text);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            button6.Enabled = false;
            button7.Enabled = true;
            var slotInput = _reversalTool.SetInputInSlot(1, textBox1.Text);

            _reversalTool.StartReversalLoop(slotInput);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            button6.Enabled = true;
            button7.Enabled = false;
            _reversalTool.StopReversalLoop();
        }


    }
}
