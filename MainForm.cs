using Gym.Data;
using Gym.Forms;
using Gym.Models;
using Gym.Services;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Gym
{
    public partial class MainForm : Form
    {
        private GymRepository _repository = new GymRepository();
        private ExportService _exportService = new ExportService();

        public MainForm()
        {
            InitializeComponent();
            RefreshGrid();
            RefreshScheduleGrid();
            UpdateCharts();
        }

       private void UpdateCharts()
{
    UpdateScheduleChart(); 
    UpdateSubscriptionChart();
}

private void UpdateScheduleChart()
{
    if (chartSchedule == null) return;

    chartSchedule.Series.Clear();

    var endDate = DateTime.Now.Date;
    var startDate = endDate.AddDays(-7);

    var series = new Series("Активність") { ChartType = SeriesChartType.Pie };

    var data = _repository.GetAllTrainings()
        .Where(t => t.Date.Date >= startDate && t.Date.Date <= endDate)
        .GroupBy(t => t.Activity)
        .Select(g => new { Activity = g.Key, Count = g.Count() })
        .ToList();

    if (data.Count > 0)
    {
        foreach (var item in data)
        {
            series.Points.AddXY(item.Activity, item.Count);
        }
    }

    chartSchedule.Series.Add(series);

    if (chartSchedule.Titles.Count == 0)
        chartSchedule.Titles.Add("Популярність занять за тиждень");

    chartSchedule.Titles[0].Text = $"Активність ({startDate:dd.MM} - {endDate:dd.MM})";
}

private void UpdateSubscriptionChart()
{
    if (chartStats == null) return;

    chartStats.Series.Clear();

    var series = new Series("Абонементи") { ChartType = SeriesChartType.Pie };
    var clients = _repository.GetAllClients();

    int activeCount = clients.Count(c => c.SubscriptionEndDate.Date >= DateTime.Now.Date); 
    int inactiveCount = clients.Count(c => c.SubscriptionEndDate.Date < DateTime.Now.Date);


    series.Points.AddXY($"Активні ({activeCount})", activeCount);
    series.Points.AddXY($"Неактивні ({inactiveCount})", inactiveCount);
    series.Points[0].Color = Color.MediumSeaGreen;
    series.Points[1].Color = Color.IndianRed;

    chartStats.Series.Add(series);

    if (chartStats.Titles.Count == 0)
        chartStats.Titles.Add("Статус абонементів");
    else
        chartStats.Titles[0].Text = "Статус абонементів";
}

        private void RefreshGrid()
        {
            dgvClients.DataSource = null;
            dgvClients.DataSource = _repository.GetAllClients();
        }

        private void RefreshScheduleGrid()
        {
            dgvSchedule.DataSource = null;
            dgvSchedule.DataSource = _repository.GetFullSchedule();

            if (dgvSchedule.Columns.Count > 0)
            {
                if (dgvSchedule.Columns.Contains("Id")) dgvSchedule.Columns["Id"].Visible = false;
                if (dgvSchedule.Columns.Contains("ClientName")) dgvSchedule.Columns["ClientName"].HeaderText = "Клієнт";
                if (dgvSchedule.Columns.Contains("TrainerName")) dgvSchedule.Columns["TrainerName"].HeaderText = "Тренер";
                if (dgvSchedule.Columns.Contains("Date")) dgvSchedule.Columns["Date"].HeaderText = "Дата та час";
                if (dgvSchedule.Columns.Contains("Activity")) dgvSchedule.Columns["Activity"].HeaderText = "Тип заняття";
            }
            _exportService.ExportScheduleToJson(_repository.GetAllTrainings(),_repository.GetAllClients(), _repository.GetAllTrainers() );
        }

        private void btnAddClient_Click(object sender, EventArgs e)
        {
            var addForm = new AddClientForm();
            addForm.OnClientAdded += (newClient) => {
                _repository.AddClient(newClient);
                RefreshGrid();
                UpdateCharts();
            };
            addForm.ShowDialog();
        }

        private void btnAddTraining_Click(object sender, EventArgs e)
        {
            var clients = _repository.GetAllClients();
            var trainers = _repository.GetAllTrainers();

            if (clients.Count == 0 || trainers.Count == 0)
            {
                MessageBox.Show("Спочатку додайте клієнтів та тренерів!");
                return;
            }

            var form = new AddTrainingForm(clients, trainers);
            form.OnTrainingAdded += (newSession) => {
                _repository.AddTraining(newSession);
                RefreshScheduleGrid();
                UpdateCharts();
            };
            form.ShowDialog();
        }

        private void btnAddTrainer_Click(object sender, EventArgs e)
        {
            var addTrainerForm = new AddTrainerForm();
            addTrainerForm.OnTrainerAdded += (newTrainer) => {
                _repository.AddTrainer(newTrainer);
                MessageBox.Show($"Тренера {newTrainer.LastName} успішно додано!");
            };
            addTrainerForm.ShowDialog();
        }

        private void btnDeleteClient_Click(object sender, EventArgs e)
        {
            if (dgvClients.SelectedRows.Count > 0)
            {
                var client = (Client)dgvClients.SelectedRows[0].DataBoundItem;
                _repository.DeleteClient(client.Id);
                RefreshGrid();
            }
        }

        private void btnEditClient_Click(object sender, EventArgs e)
        {
            if (dgvClients.CurrentRow != null)
            {
                var client = (Client)dgvClients.CurrentRow.DataBoundItem;
                var editForm = new AddClientForm(client);
                editForm.OnClientUpdated += (updatedClient) => {
                    _repository.UpdateClient(updatedClient);
                    RefreshGrid();
                };
                editForm.ShowDialog();
            }
            else
            {
                MessageBox.Show("Спочатку виберіть клієнта в таблиці!", "Увага");
            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            string filter = txtSearch.Text.ToLower();
            var clients = _repository.GetAllClients();
            var filtered = clients.Where(c =>
                c.FirstName.ToLower().Contains(filter) ||
                c.LastName.ToLower().Contains(filter) ||
                c.Phone.Contains(filter)).ToList();

            dgvClients.DataSource = null;
            dgvClients.DataSource = filtered;
        }

        private void btnExportExcel_Click(object sender, EventArgs e)
        {
            _exportService.ExportToExcel(_repository.GetAllClients());
            MessageBox.Show("Дані експортовано!");
        }

        private void btnExportWord_Click(object sender, EventArgs e)
        {
            if (dgvClients.CurrentRow != null)
            {
                var client = (Client)dgvClients.CurrentRow.DataBoundItem;
                _exportService.CreateWordContract(client);
                MessageBox.Show("Договір сформовано!");
            }
            else
            {
                MessageBox.Show("Спочатку виберіть клієнта в таблиці!", "Увага");
            }
        }

        private void DeleteTraining_Click(object sender, EventArgs e)
        {
            if (dgvSchedule.CurrentRow != null)
            {
                int trainingId = Convert.ToInt32(dgvSchedule.CurrentRow.Cells["Id"].Value);

                var result = MessageBox.Show("Ви дійсно хочете скасувати це тренування?", "Підтвердження", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    _repository.DeleteTraining(trainingId);
                    RefreshScheduleGrid();
                    UpdateCharts();
                }
            }
            else
            {
                MessageBox.Show("Спочатку виберіть тренування в таблиці розкладу!", "Увага");
            }
        }
    }
}