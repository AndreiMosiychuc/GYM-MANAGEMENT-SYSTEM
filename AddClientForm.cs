using Gym.Models;
using System;
using System.Windows.Forms;

namespace Gym.Forms
{
    public partial class AddClientForm : Form
    {
        public event Action<Client> OnClientAdded;
        public event Action<Client> OnClientUpdated;

        private Client _clientToEdit = null;

        public AddClientForm()
        {
            InitializeComponent();
            this.Text = "Додати клієнта";
        }

        public AddClientForm(Client clientToEdit)
        {
            InitializeComponent();
            _clientToEdit = clientToEdit;

            txtFirstName.Text = clientToEdit.FirstName;
            txtLastName.Text = clientToEdit.LastName;
            txtPhone.Text = clientToEdit.Phone;
            numAge.Value = clientToEdit.Age;
            dtpSubEnd.Value = clientToEdit.SubscriptionEndDate;

            this.Text = "Редагувати клієнта";
            btnSave.Text = "Оновити";
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (_clientToEdit == null)
                {
                    var newClient = new Client(0, txtFirstName.Text, txtLastName.Text, txtPhone.Text, (int)numAge.Value, dtpSubEnd.Value);
                    OnClientAdded?.Invoke(newClient);
                }
                else
                {
                    _clientToEdit.FirstName = txtFirstName.Text;
                    _clientToEdit.LastName = txtLastName.Text;
                    _clientToEdit.Phone = txtPhone.Text;
                    _clientToEdit.Age = (int)numAge.Value;
                    _clientToEdit.SubscriptionEndDate = dtpSubEnd.Value;

                    OnClientUpdated?.Invoke(_clientToEdit);
                }
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка: {ex.Message}");
            }
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }
    }
}
