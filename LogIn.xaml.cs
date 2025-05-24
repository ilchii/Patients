using System.Linq;
using System.Windows;
using Patients.Models;

namespace Patients
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            var username = UsernameBox.Text;
            var password = PasswordBox.Password;

            var user = UserStore.Users.FirstOrDefault(u =>
                u.Username == username &&
                u.Password == password);

            if (user != null)
            {
                if (user.Role == "Receptionist")
                {
                    var recWindow = new ReceptionistStarterWindow();
                    recWindow.Show();
                }
                else if (user.Role == "Doctor")
                {
                    var docWindow = new DoctorStarterWindow();
                    docWindow.Show();
                }
                this.Close();
            }
            else
            {
                MessageBox.Show("Invalid credentials. Try again.");
            }
        }
    }
}
