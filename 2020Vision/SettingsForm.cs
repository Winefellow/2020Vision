using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Vision2020
{
    public partial class SettingsForm : Form
    {
        public SettingsForm()
        {
            InitializeComponent();
            cbApexSpeed.Checked = Config.UserConfig.AnnounceApexSpeed;
            cbTimeSpent.Checked = Config.UserConfig.AnnounceCornerTime;
            cbDistance2Apex.Checked = Config.UserConfig.AnnounceDinstance2Apex;
            comboAnnounceSpeed.SelectedIndex = comboAnnounceSpeed.Items.IndexOf(Config.UserConfig.AnnouceSpeed.ToString());
            comboAnnounceSpeed.Text = Convert.ToString(comboAnnounceSpeed.Items[comboAnnounceSpeed.SelectedIndex]);
        }

        private void SettingsForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Config.UserConfig.AnnounceApexSpeed = cbApexSpeed.Checked;
            Config.UserConfig.AnnounceCornerTime = cbTimeSpent.Checked;
            Config.UserConfig.AnnounceDinstance2Apex = cbDistance2Apex.Checked;
            Config.UserConfig.AnnouceSpeed = Convert.ToInt32(comboAnnounceSpeed.Items[comboAnnounceSpeed.SelectedIndex]);

            Config.SaveConfig();
            SpeachSynthesizer.QueueText("Configuration is saved");
        }
    }
    public class Vision2020Settings
    {
        public bool AnnounceDinstance2Apex { get; set; }
        public bool AnnounceApexSpeed { get; set; }
        public bool AnnounceCornerTime { get; set; }
        public int AnnouceSpeed { get; set; }
    }

    public static class Config
    {
        private static Vision2020Settings _instance = null;
        public static Vision2020Settings UserConfig
        {
            get 
            {
                if (_instance == null)
                {
                    if (File.Exists("config.json"))
                    {
                        _instance = JsonSerializer.Deserialize<Vision2020Settings>(File.ReadAllText("settings.json"));
                    }
                    else
                    {
                        _instance = new Vision2020Settings() { AnnouceSpeed = 2, AnnounceApexSpeed = true, AnnounceCornerTime = true, AnnounceDinstance2Apex = false };
                    }
                }
                return _instance;
            }
        }

        public static void SaveConfig()
        {
            File.WriteAllText("setting.json", JsonSerializer.Serialize(UserConfig));
            SpeachSynthesizer.UpdateSpeed();
        }
    }
}
