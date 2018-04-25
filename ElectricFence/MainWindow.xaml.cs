using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace ElectricFence
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<ScriptGenerator> policy;
        SaveHandler ps1;
        public MainWindow()
        {
            InitializeComponent();
            policy = new List<ScriptGenerator>();
        }

        private void generate_Click(object sender, RoutedEventArgs e)
        {
            if (!verifyFields())
            {
                return;
            }

            scriptPreview.Text = "New-NetFirewallRule -DisplayName \"" + displayName.Text + "\"";

            if (groupName.Text != "")
            {
                scriptPreview.Text += " -Group " + groupName.Text;
            }

            if (description.Text != "")
            {
                scriptPreview.Text += " -Description \"" + description.Text + "\"";
            }

            switch (protocol.SelectedIndex)
            {
                case 0:
                    scriptPreview.Text += " -Protocol Any";
                    break;
                case 1:
                    scriptPreview.Text += " -Protocol ICMPv4";
                    break;
                case 2:
                    scriptPreview.Text += " -Protocol TCP";
                    break;
                case 3:
                    scriptPreview.Text += " -Protocol UDP";
                    break;
            }

            if (allowButton.IsChecked == true)
            {
                scriptPreview.Text += " -Action Allow";
            }
            else
            {
                scriptPreview.Text += " -Action Block";
            }

            if (inboundButton.IsChecked == true)
            {
                scriptPreview.Text += " -Direction Inbound";
            }
            else
            {
                scriptPreview.Text += " -Direction Outbound";
            }

            if (lAddress.Text != "" && lCIDR.Text != "")
            {
                scriptPreview.Text += " -LocalAddress " + lAddress.Text + "/" + lCIDR.Text;
            }

            if (lPort1.Text != "" && lPort2.Text != "" && protocol.SelectedIndex != 1)
            {
                if (lPort1.Text == lPort2.Text)
                {
                    scriptPreview.Text += " -LocalPort " + lPort1.Text;
                }
                else
                {
                    scriptPreview.Text += " -LocalPort " + lPort1.Text + "-" + lPort2.Text;
                }
            }
            else
            {
                scriptPreview.Text += " -LocalPort Any";
            }

            switch (authentication.SelectedIndex)
            {
                case 0:
                    scriptPreview.Text += " -Authentication NotRequired";
                    break;
                case 1:
                    scriptPreview.Text += " -Authentication Required";
                    break;
                case 2:
                    scriptPreview.Text += " -Authentication NoEncap";
                    break;
            }

            switch (intfc.SelectedIndex)
            {
                case 0:
                    scriptPreview.Text += " -InterfaceType Any";
                    break;
                case 1:
                    scriptPreview.Text += " -InterfaceType Wired";
                    break;
                case 2:
                    scriptPreview.Text += " -InterfaceType Wireless";
                    break;
                case 3:
                    scriptPreview.Text += " -InterfaceType RemoteAccess";
                    break;
            }

            Save.IsEnabled = true;
            header.SelectedIndex = 1;
        }

        private void run_Click(object sender, RoutedEventArgs e)
        {
            ScriptGenerator newScript = new ScriptGenerator();

            newScript.Script = scriptPreview.Text;
            bool success = newScript.deployFirewall(newScript.Script);

            if (success)
            {
                ComboBoxItem comboBoxItem = new ComboBoxItem();
                comboBoxItem.Content = displayName.Text;
                list.Items.Add(comboBoxItem);

                policy.Add(newScript);
            }
        }

        private void msdnButton_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://technet.microsoft.com/en-us/library/jj554908.aspx");
        }

        private bool verifyFields()
        {
            //  DisplayName verification for any value
            if (displayName.Text == "")
            {
                MessageBox.Show("The DisplayName parameter cannot be empty.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            //  IP Address verification for formatting and values
            if (lAddress.Text != "")
            {
                int result;
                bool pass = true;
                string[] separate = new string[] { "." };
                string[] tokens = lAddress.Text.Split(separate, StringSplitOptions.None);

                if (tokens.Length != 4)
                {
                    pass = false;
                }

                foreach (string token in tokens)
                {
                    if (!int.TryParse(token, out result))
                    {
                        pass = false;
                    }

                    if (result < 0 || result > 255)
                    {
                        pass = false;
                    }
                }

                if (!int.TryParse(lCIDR.Text, out result))
                {
                    pass = false;
                }

                if (result < 0 || result > 32)
                {
                    pass = false;
                }

                if (!pass)
                {
                    MessageBox.Show("The IP address must be a valid IPv4 address. \n(IPv6 support coming soon!)", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    lAddress.Text = "";
                    return false;
                }
            }   // End IP address

            //  Port Range verification for values and order
            if (lPort1.Text != "" || lPort2.Text != "")
            {
                int result;
                int result2;
                bool pass = true;

                if (lPort1.Text == "")
                {
                    if (!int.TryParse(lPort2.Text, out result))
                    {
                        pass = false;
                    }

                    if (result < 0 || result > 65355)
                    {
                        pass = false;
                    }

                    lPort1.Text = lPort2.Text;
                }
                else if (lPort2.Text == "")
                {
                    if (!int.TryParse(lPort1.Text, out result))
                    {
                        pass = false;
                    }

                    if (result < 0 || result > 65355)
                    {
                        pass = false;
                    }

                    lPort2.Text = lPort1.Text;
                }
                else
                {
                    if (!int.TryParse(lPort1.Text, out result))
                    {
                        pass = false;
                    }

                    if (!int.TryParse(lPort2.Text, out result2))
                    {
                        pass = false;
                    }

                    if (result < 0 || result > 65355 || result2 < 0 || result2 > 65355)
                    {
                        pass = false;
                    }

                    if (result2 > result)
                    {
                        int temp = result2;
                        result2 = result;
                        result = temp;
                    }
                }

                if (!pass)
                {
                    MessageBox.Show("Port values must be integers from 0 to 65535 or leave blank for Any \n(Not valid for ICMP protocol)", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    lPort1.Text = "";
                    lPort2.Text = "";
                    return false;
                }
            }   // End port range

            return true;
        }

        private void list_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int sel = list.SelectedIndex;
            scriptPreview.Text = policy[sel].Script;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (ps1 == null)
            {
                ps1 = new SaveHandler();
                ps1.SavePrep(policy);
            }
        }
    }
}