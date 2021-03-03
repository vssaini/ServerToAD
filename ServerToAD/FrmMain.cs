using System;
using System.Diagnostics;
using System.DirectoryServices;
using System.Windows.Forms;

namespace ServerToAD
{
    public partial class FrmMain : Form
    {
        private bool _isConnected;
        public FrmMain()
        {
            InitializeComponent();
        }

        private void btnConnect_Click(object sender, EventArgs args)
        {
            try
            {
                // create LDAP connection object 
                var ldapPath = string.Format("LDAP://{0}/CN=Users,DC=domain,DC=com", txtDcIpAddress.Text);
                var ldapConnection = new DirectoryEntry
                {
                    Path = ldapPath,
                    AuthenticationType = AuthenticationTypes.Secure,
                    Username = txtUsername.Text,
                    Password = txtPassword.Text
                };

                // create search object which operates on LDAP connection object  
                // and set search object to only find the user specified  

                var search = new DirectorySearcher(ldapConnection) {Filter = "(cn=Administrator)"};

                // create results objects from search object  

                SearchResult result = search.FindOne();

                if (result != null)
                {
                    _isConnected = true;
                    // user exists, cycle through LDAP fields (cn, telephonenumber etc.)  

                    ResultPropertyCollection fields = result.Properties;

                    if (fields.PropertyNames != null)
                        foreach (String ldapField in fields.PropertyNames)
                        {
                            // cycle through objects in each field e.g. group membership  
                            // (for many fields there will only be one object such as name)  

                            foreach (Object myCollection in fields[ldapField])
                                Debug.WriteLine("{0,-20} : {1}", ldapField, myCollection);
                        }
                }

                else
                {
                    // user does not exist  
                    MessageBox.Show("User not found!");
                }

                MessageBox.Show(_isConnected ? "Got connected to AD successfully!" : "Cannot connect to AD.");
            }

            catch (Exception e)
            {
                MessageBox.Show("Exception caught:\n\n" + e); 
            }  
        }
    }
}
