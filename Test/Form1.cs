﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using WPMTK;

namespace Test
{
    public partial class Form1 : Form
    {
        #region Fields
        private IntPtr address;
        private Process process;
        private OverlaySettings overlaySettings;
        private List<System.Diagnostics.Process> processes = new List<System.Diagnostics.Process>();

#endregion

        public Form1()
        {
            InitializeComponent();
            timer1.Enabled = true;
        }

        #region Private Methods
        private void UpdateProcesses()
        {
            processes.Clear();
            foreach (System.Diagnostics.Process proc in System.Diagnostics.Process.GetProcesses())
            {
                if (!String.IsNullOrEmpty(proc.MainWindowTitle))
                    processes.Add(proc);
            }
        }

#endregion

        // set address's value (int/string)
        private void button1_Click(object sender, EventArgs e)
        {
            if (dataTypeBox.SelectedItem == dataTypeBox.Items[0]) // int
            {
                int value = (int)numericUpDown1.Value;
                process.memory.WriteInt32(address, value);
            }
            else if (dataTypeBox.SelectedItem == dataTypeBox.Items[1]) // string
            {
                process.memory.WriteStringASCII(address, addressNewBox.Text);
            }
        }

        private void Form1_Load(object sender, EventArgs e) {

        }

        // set the address
        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                int str_int = (int)new Int32Converter().ConvertFromString(addressBox.Text);
                address = (IntPtr)str_int;
                addressSet.Enabled = false;
            }
            catch
            {
                MessageBox.Show("\"" + addressBox.Text + "\" is not recognized as a valid memory address.", "Address Error!");
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (addressNewSet.Enabled != true)
                addressNewSet.Enabled = true;
        }

        /*
        // set process title
        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                if (process != null)
                    process.ChangeProcess(processTitleBox.Text);
                else
                    process = new Process(processTitleBox.Text); // Process initializer
                processTitleInfo.Visible = false; // hide the info label
            }
            catch
            {
                MessageBox.Show("Could not attach to that process. The title does not seem to meet any matches.", "Failed changing processes.");
            }
        }
        */

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (dataTypeBox.SelectedItem == dataTypeBox.Items[0]) // int
            {
                label1.Visible = true;
                addressNewBox.Visible = false;
                numericUpDown1.Visible = true;
            }
            else if (dataTypeBox.SelectedItem == dataTypeBox.Items[1]) // string
            {
                label1.Visible = true;
                numericUpDown1.Visible = false;
                addressNewBox.Visible = true;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            // update address
            if (address != null && process != null) // sync is available
            {
                if (dataTypeBox.SelectedItem == dataTypeBox.Items[0]) // int
                    addressCurrentNum.Value = process.memory.ReadInt32(address);
                else if (dataTypeBox.SelectedItem == dataTypeBox.Items[1]) // string
                    addressNewBox.Text = process.memory.ReadStringASCII(address, 0);
            }

            // update status strip
            if (process != null && !String.IsNullOrEmpty(processBox.SelectedText))
            {
                statusPID.Text = "PID: " + processes[processBox.SelectedIndex].Id.ToString();
                statusPID.ToolTipText = statusPID.Text;
                statusPName.Text = processes[processBox.SelectedIndex].ProcessName;
                statusPName.ToolTipText = statusPName.Text;
                statusProcessMemory.Text = "Paged M. Size: " + processes[processBox.SelectedIndex].PagedMemorySize64.ToString() + " (bytes)";
                statusProcessMemory.ToolTipText = statusProcessMemory.Text;
            }
            else
            {
                // null everything
                statusPID.Text = "";
                statusPName.Text = "";
                statusProcessMemory.Text = "";
            }
        }

        // Overlay Settings
        private void toolbarOverlayButton_Click(object sender, EventArgs e)
        {
            if (overlaySettings != null && !overlaySettings.IsDisposed) // hasn't been closed yet, just unfocused
                overlaySettings.Show();
            else // the form had been closed, reopen it
            {
                if (process != null)
                {
                    overlaySettings = new OverlaySettings(process);
                    overlaySettings.Show();
                }
                else
                {
                    MessageBox.Show("You must specify a window title for the target process before you can draw an overlay.");
                }
            }
        }

        // processBox dropdown is opened
        private void processBox_DropDown(object sender, EventArgs e)
        {
            // update list of selectable processes
            UpdateProcesses();
            processBox.Items.Clear();
            foreach (System.Diagnostics.Process proc in processes)
            {
                processBox.Items.Add(proc.MainWindowTitle);
            }
        }

        // processBox selection made
        private void processBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            process = new Process(processes[processBox.SelectedIndex].MainWindowTitle);
            processTitleInfo.Visible = false;
        }
    }
}
