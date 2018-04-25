using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace ElectricFence
{
    public class ScriptGenerator
    {
        protected string script;

        public ScriptGenerator() { }

        public string Script
        {
            get => script;
            set => script = value;
        }

        public bool deployFirewall(string script)
        {
            PowerShell instance = PowerShell.Create();
            instance.AddScript(script);
            IAsyncResult result = instance.BeginInvoke();

            //PSInvocationState state = new PSInvocationState();
            while (!result.IsCompleted)
            {
                Thread.Sleep(1000);
            }

            if (result.IsCompleted)
            {
                MessageBox.Show("The rule has been added to Windows Firewall with Advanced Security.", "Rule added!", MessageBoxButton.OK, MessageBoxImage.Information);
                instance.Dispose();
                return true;
            }

            return true;
        }
    }
}