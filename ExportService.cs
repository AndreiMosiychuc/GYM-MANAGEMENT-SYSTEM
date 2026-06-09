using Gym.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows.Forms;
using ClosedXML.Excel;
using Xceed.Words.NET;
using Xceed.Document.NET;

namespace Gym.Services
{
    public class ExportService
    {
        public void ExportToExcel(List<Client> clients)
        {
            try
            {
                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("Клієнти");

                    worksheet.Cell(1, 1).SetValue("ID");
                    worksheet.Cell(1, 2).SetValue("Прізвище");
                    worksheet.Cell(1, 3).SetValue("Ім'я");
                    worksheet.Cell(1, 4).SetValue("Телефон");
                    worksheet.Cell(1, 5).SetValue("Вік");
                    worksheet.Cell(1, 6).SetValue("Дійсний до");

                    var headerRow = worksheet.Row(1);
                    headerRow.Style.Font.Bold = true;
                    headerRow.Style.Fill.BackgroundColor = XLColor.LightGray;

                    for (int i = 0; i < clients.Count; i++)
                    {
                        int row = i + 2;
                        worksheet.Cell(row, 1).SetValue(clients[i].Id);
                        worksheet.Cell(row, 2).SetValue(clients[i].LastName);
                        worksheet.Cell(row, 3).SetValue(clients[i].FirstName);
                        worksheet.Cell(row, 4).SetValue(clients[i].Phone);
                        worksheet.Cell(row, 5).SetValue(clients[i].Age);
                        worksheet.Cell(row, 6).SetValue(clients[i].SubscriptionEndDate.ToShortDateString());
                    }
                    worksheet.Columns().AdjustToContents();

                    string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Clients_Export.xlsx");
                    workbook.SaveAs(filePath);

                    MessageBox.Show($"Дані успішно експортовано в Excel!\nШлях: {filePath}", "Успіх", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка експорту в Excel: {ex.Message}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void CreateWordContract(Client client)
        {
            try
            {
                string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"Contract_{client.LastName}.docx");

                using (var document = DocX.Create(filePath))
                {
                    var title = document.InsertParagraph("ДОГОВІР НА НАДАННЯ ПОСЛУГ")
                        .FontSize(18)
                        .Bold();

                    title.Alignment = Alignment.center; 
                    title.SpacingAfter(20);

                    document.InsertParagraph($"Цей документ підтверджує, що клієнт ")
                        .Append($"{client.FirstName} {client.LastName}").Bold()
                        .Append(" уклав угоду з тренажерним залом 'GymMaster'.");

                    document.InsertParagraph("\nДеталі клієнта:").Bold();

                    var list = document.AddList("Номер телефону: " + client.Phone, 0, ListItemType.Bulleted);
                    document.AddListItem(list, "Вік: " + client.Age);
                    document.AddListItem(list, "Дата закінчення абонемента: " + client.SubscriptionEndDate.ToShortDateString());
                    document.InsertList(list);

                    document.InsertParagraph("\n\n______________________\n(Підпис клієнта)")
                        .Alignment = Alignment.right;

                    document.Save();
                    MessageBox.Show($"Договір для {client.LastName} сформовано!", "Успіх", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка створення Word документа: {ex.Message}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void ExportScheduleToJson(List<TrainingSession> trainings, List<Client> clients, List<Trainer> trainers)
        {
            try
            {
                string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "schedule.json");

                var exportData = trainings.Select(t => {
                    var client = clients.FirstOrDefault(c => c.Id == t.ClientId);
                    var trainer = trainers.FirstOrDefault(tr => tr.Id == t.TrainerId);

                    return new
                    {
                        ClientPhone = client?.Phone,
                        ClientName = client != null ? $"{client.FirstName} {client.LastName}" : "Невідомий",
                        TrainingDate = t.Date.ToString("yyyy-MM-dd HH:mm"),
                        Activity = t.Activity,
                        Trainer = trainer != null ? new
                        {
                            FullName = $"{trainer.FirstName} {trainer.LastName}",
                            Specialization = trainer.Specialization,
                            ContactPhone = trainer.Phone
                        } : null
                    };
                }).ToList();

                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                };

                string jsonString = JsonSerializer.Serialize(exportData, options);
                File.WriteAllText(filePath, jsonString);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Сталася помилка при JSON експорті:\n{ex.Message}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}