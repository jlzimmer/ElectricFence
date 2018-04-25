using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ElectricFence
{
    class SaveHandler
    {
        public string path;
        private StringBuilder ps1;

        public SaveHandler()
        {
            path = null;
            ps1 = new StringBuilder();
        }

        public void SavePrep(List<ScriptGenerator> policy)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                DefaultExt = ".ps1",
                Filter = "PowerShell Script (.ps1)|*.ps1",
                FileName = path
            };

            if (path != null)
            {
                saveFileDialog.FileName = Path.GetFileName(path);
                saveFileDialog.InitialDirectory = Path.GetDirectoryName(path);
            }

            else
            {
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    path = saveFileDialog.FileName;
                }
                else
                {
                    MessageBox.Show("There was an issue finding a suitable save location.", "Save error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
            }

            SaveDocument(policy);
        }

        private void SaveDocument(List<ScriptGenerator> policy)
        {

            foreach (ScriptGenerator script in policy)
            {
                string rule = script.Script;

                var line = $"{rule}; ";
                ps1.AppendLine(line);
            }

            try
            {
                using (StreamWriter sw = new StreamWriter(path, append: true))
                {
                    sw.Write(ps1.ToString());
                }

                MessageBox.Show($"The script was saved in ps1 format at location \n{path}", "Save successful!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            catch (Exception)
            {
                MessageBox.Show("There was an issue writing to the selected save location. \nPlease select a new save path.", "Save error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
        }
    }
}
