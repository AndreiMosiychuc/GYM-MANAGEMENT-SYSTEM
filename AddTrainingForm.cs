using Gym.Models;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Gym.Forms
{
    public partial class AddTrainingForm : Form
    {
        public event Action<TrainingSession> OnTrainingAdded;

        public AddTrainingForm(List<Client> clients, List<Trainer> trainers)
        {
            InitializeComponent();
            this.Text = "Запис на тренування";

            dtpDate.Format = DateTimePickerFormat.Custom;
            dtpDate.CustomFormat = "dd.MM.yyyy HH:mm";

            cmbClient.DataSource = clients;
            cmbClient.DisplayMember = "LastName";
            cmbClient.ValueMember = "Id";

            cmbTrainer.DataSource = trainers;
            cmbTrainer.DisplayMember = "LastName";
            cmbTrainer.ValueMember = "Id";
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (cmbClient.SelectedItem == null || cmbTrainer.SelectedItem == null)
            {
                MessageBox.Show("Оберіть клієнта та тренера!", "Помилка");
                return;
            }

            if (string.IsNullOrWhiteSpace(txtActivity.Text))
            {
                MessageBox.Show("Введіть тип тренування!", "Помилка");
                return;
            }

            try
            {
                var newTraining = new TrainingSession(
                    0,
                    Convert.ToInt32(cmbClient.SelectedValue),
                    Convert.ToInt32(cmbTrainer.SelectedValue),
                    dtpDate.Value,
                    txtActivity.Text
                );

                OnTrainingAdded?.Invoke(newTraining);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Помилка створення тренування: " + ex.Message);
            }
        }

        private void AddTrainingForm_Load(object sender, EventArgs e)
        {

        }
    }
}