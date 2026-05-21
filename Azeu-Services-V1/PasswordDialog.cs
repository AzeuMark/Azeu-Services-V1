using System;
using System.Windows.Forms;

namespace AzeuServices_V1 // <--- Ensure this matches your project namespace
{
    public static class PasswordDialog
    {
        public static bool Authenticate()
        {
            var settings = AppSettings.Load();

            using (Form prompt = new Form())
            {
                prompt.Width = 300;
                prompt.Height = 160;
                prompt.Text = "Security Required";
                prompt.StartPosition = FormStartPosition.CenterScreen;
                prompt.FormBorderStyle = FormBorderStyle.FixedDialog;
                prompt.MaximizeBox = false;
                prompt.MinimizeBox = false;
                prompt.TopMost = true;

                Label textLabel = new Label() { Left = 20, Top = 20, Text = "Enter Administrator Password:", Width = 250 };
                TextBox textBox = new TextBox() { Left = 20, Top = 45, Width = 240, PasswordChar = '*' };
                Button confirmation = new Button() { Text = "Ok", Left = 185, Width = 75, Top = 85, DialogResult = DialogResult.OK };

                prompt.Controls.Add(textBox);
                prompt.Controls.Add(confirmation);
                prompt.Controls.Add(textLabel);
                prompt.AcceptButton = confirmation;

                if (prompt.ShowDialog() == DialogResult.OK)
                {
                    if (textBox.Text == settings.AdminPassword)
                    {
                        return true;
                    }
                    else
                    {
                        MessageBox.Show("Incorrect password. Access denied.", "Security Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                }
                return false;
            }
        }
    }
}