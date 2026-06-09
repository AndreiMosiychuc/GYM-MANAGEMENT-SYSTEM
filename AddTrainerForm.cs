using Gym.Data;
using Gym.Models;
using System;
using System.Windows.Forms;

namespace Gym.Forms
{
    public partial class AddTrainerForm : Form
    {
        public event Action<Trainer> OnTrainerAdded;

        private GymRepository _repo = new GymRepository();

        public AddTrainerForm()
        {
            InitializeComponent();
            this.Text = "Керування тренерами";
            RefreshGrid();
        }

        private void RefreshGrid()
        {
            try
            {

                var trainers = _repo.GetAllTrainers();
                dgvTrainers.DataSource = null;
                dgvTrainers.DataSource = trainers;
                if (dgvTrainers.Columns.Count > 0)
                {
                    if (dgvTrainers.Columns.Contains("Id")) dgvTrainers.Columns["Id"].Visible = false;
                    dgvTrainers.Columns["FirstName"].HeaderText = "Ім'я";
                    dgvTrainers.Columns["LastName"].HeaderText = "Прізвище";
                    dgvTrainers.Columns["Specialization"].HeaderText = "Спеціалізація";
                    dgvTrainers.Columns["Experience"].HeaderText = "Досвід";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Помилка при завантаженні списку: " + ex.Message);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtFirstName.Text) || string.IsNullOrWhiteSpace(txtLastName.Text))
            {
                MessageBox.Show("Заповніть Прізвище та Ім'я!");
                return;
            }

            var newTrainer = new Trainer(0, txtFirstName.Text, txtLastName.Text, txtPhone.Text, txtSpecialization.Text, (int)numExperience.Value);

            OnTrainerAdded?.Invoke(newTrainer);
            RefreshGrid();

            txtFirstName.Clear();
            txtLastName.Clear();
            txtPhone.Clear();
            txtSpecialization.Clear();
            numExperience.Value = 0;
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvTrainers.CurrentRow != null)
            {
                var trainer = dgvTrainers.CurrentRow.DataBoundItem as Trainer;
                if (trainer != null)
                {
                    var result = MessageBox.Show($"Видалити тренера {trainer.LastName}?", "Підтвердження", MessageBoxButtons.YesNo);
                    if (result == DialogResult.Yes)
                    {
                        _repo.DeleteTrainer(trainer.Id);
                        RefreshGrid();
                    }
                }
            }
        }

        private void txtSpecialization_TextChanged(object sender, EventArgs e)
        {

        }
    }
}