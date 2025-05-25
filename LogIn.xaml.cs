using System.Linq;
using System.Windows;
using Patients.Data;
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

            using var db = new AppDbContext();
            var user = db.Users.FirstOrDefault(u =>
                u.Username == username && u.Password == password);

            if (user != null)
            {
                if (user.Role == "Receptionist")
                {
                    new ReceptionistStarterWindow().Show();
                }
                else if (user.Role == "Doctor")
                {
                    new DoctorStarterWindow().Show();
                }
                this.Close();
            }
            else
            {
                MessageBox.Show("Invalid credentials.");
            }
        }
    }
}
