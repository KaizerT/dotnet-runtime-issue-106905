using System.DirectoryServices.AccountManagement;

namespace PrincipalContextIssueRepro
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void cmdLogin_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtUsername.Text?.Trim()) || string.IsNullOrEmpty(txtPassword.Text?.Trim()))
            {
                MessageBox.Show("Username and password required", "Fill all fields", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                try
                {
                    string username = txtUsername.Text.Trim();
                    string password = txtPassword.Text.Trim();

                    bool hasDomain = username.Contains("\\");

                    string domainName = hasDomain ? username.Split('\\')[0] : "";

                    PrincipalContext context = hasDomain ? new PrincipalContext(ContextType.Domain, domainName)
                                                : new PrincipalContext(ContextType.Machine);

                    var principal = UserPrincipal.FindByIdentity(context, IdentityType.SamAccountName, username);

                    // User is known to AD
                    if (principal != null)
                    {
                        if (principal.IsAccountLockedOut())
                        {
                            MessageBox.Show("Account is locked out", "Locked User", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            context?.Dispose();
                            return;
                        }

                        if (principal.Enabled.HasValue && !principal.Enabled.Value)
                        {
                            MessageBox.Show("Account is disabled", "Disabled User", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            context?.Dispose();
                            return;
                        }

                        bool isValid = context.ValidateCredentials(username, password);

                        if (isValid)
                        {
                            MessageBox.Show("User valid", "Disabled User", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            context?.Dispose();
                            return;
                        }
                        else
                        {
                            MessageBox.Show("Username/Password incorrect", "Disabled User", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            context?.Dispose();
                            return;
                        }
                    }
                    else
                    {
                        MessageBox.Show("Unknown username", "Invalid User", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    context?.Dispose();

                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Unexpected exception encountered {ex}", "Unexpected exception", MessageBoxButtons.OK, MessageBoxIcon.Error);

                }


            }
        }
    }
}
