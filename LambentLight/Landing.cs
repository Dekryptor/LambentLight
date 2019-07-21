﻿using NLog;
using NLog.Config;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LambentLight
{
    public partial class Landing : Form
    {
        /// <summary>
        /// Sets the locked status of some of the UI elements.
        /// </summary>
        public bool Locked
        {
            set
            {
                StartItem.Enabled = !value;
                StopItem.Enabled = value;
                CreateItem.Enabled = !value;
                ExitItem.Enabled = !value;
            }
        }

        public Landing()
        {
            // Initialize the UI elements
            InitializeComponent();
        }

        private void Landing_Load(object sender, EventArgs e)
        {
            // Create a new configuration for NLog
            LoggingConfiguration NewConfig = new LoggingConfiguration();
            // Add a rule for logging to the TextBox
            NewConfig.AddRule(LogLevel.Info, LogLevel.Fatal, new TextBoxTarget() { Box = LogBox, Layout = "[${date}] [${level}] ${message}" });
            // Set the already created configuration
            LogManager.Configuration = NewConfig;
            // And filll the Builds and Data folders
            BuildManager.Fill(BuildsBox);
            DataFolderManager.Fill(DataBox);
            // Set the elements to unlocked
            Locked = false;
        }

        private async void StartItem_Click(object sender, EventArgs e)
        {
            // Start the build with the selected options
            Locked = await ServerManager.Start((Build)BuildsBox.SelectedItem, (DataFolder)DataBox.SelectedItem);
        }

        private async void StopItem_Click(object sender, EventArgs e)
        {
            // Stop the server if is present
            Locked = await ServerManager.Stop();
        }

        private async void CreateItem_Click(object sender, EventArgs e)
        {
            // Ask the user for inputing a server data folder name
            string FolderName = Microsoft.VisualBasic.Interaction.InputBox("Please insert a name for the new Server Data Folder:", "New Server Data Folder");
            // Create a server data folder
            await DataFolderManager.Create(FolderName);
        }

        private void ExitItem_Click(object sender, EventArgs e)
        {
            // Close the current form
            Close();
        }

        private void VisibleTextBox_CheckedChanged(object sender, EventArgs e)
        {
            // Change the enabled status of the License TextBox
            LicenseTextBox.Enabled = VisibleCheckBox.Checked;
            SaveLicenseButton.Enabled = VisibleCheckBox.Checked;
            // If the CheckBox is enabled
            if (VisibleCheckBox.Checked)
            {
                // Fill the text box with the license
                LicenseTextBox.Text = Properties.Settings.Default.License;
            }
            // Otherwise
            else
            {
                // Delete it
                LicenseTextBox.Text = string.Empty;
            }
        }

        private void GenerateLicenseButton_Click(object sender, EventArgs e)
        {
            // Open the FiveM Keymaster page
            Process.Start("https://keymaster.fivem.net");
        }

        private void SaveLicenseButton_Click(object sender, EventArgs e)
        {
            // Save the license on the text box
            Properties.Settings.Default.License = LicenseTextBox.Text;
            Properties.Settings.Default.Save();
        }
    }
}
