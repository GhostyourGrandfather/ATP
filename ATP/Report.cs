using System;
using System.Data.SqlClient;
using System.Data;

namespace ATP
{
    public class Report
    {
        public int Id { get; set; }
        public string ReportType { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime GeneratedAt { get; set; }
        public string GeneratedBy { get; set; }
        public string ReportData { get; set; }

        // Метод для генерации отчета
        public static DataTable GenerateReport(string reportType, DateTime startDate, DateTime endDate)
        {
            DataTable result = new DataTable();

            try
            {
                using (SqlConnection connection = new SqlConnection(App.ConnectionString))
                {
                    connection.Open();
                    string query = "";

                    switch (reportType)
                    {
                        case "По маршрутам":
                            query = @"SELECT r.StartPoint + ' → ' + r.EndPoint AS Маршрут, 
                                    r.Distance AS Расстояние, r.TravelTime AS Время,
                                    d.Name AS Водитель, v.Brand + ' ' + v.Model AS Транспорт
                                    FROM Routes r
                                    JOIN Drivers d ON r.DriverId = d.Id
                                    JOIN Vehicles v ON r.VehicleId = v.Id
                                    WHERE r.CreatedDate BETWEEN @StartDate AND @EndDate";
                            break;
                        case "По водителям":
                            query = @"SELECT d.Name AS Водитель, COUNT(r.Id) AS КоличествоМаршрутов,
                                    SUM(r.Distance) AS ОбщийПробег, AVG(r.TravelTime) AS СреднееВремя
                                    FROM Drivers d
                                    LEFT JOIN Routes r ON d.Id = r.DriverId
                                    WHERE r.CreatedDate BETWEEN @StartDate AND @EndDate
                                    GROUP BY d.Name";
                            break;
                        case "По транспорту":
                            query = @"SELECT v.Brand + ' ' + v.Model AS Транспорт, 
                                    COUNT(r.Id) AS КоличествоРейсов, SUM(r.Distance) AS ОбщийПробег
                                    FROM Vehicles v
                                    LEFT JOIN Routes r ON v.Id = r.VehicleId
                                    WHERE r.CreatedDate BETWEEN @StartDate AND @EndDate
                                    GROUP BY v.Brand, v.Model";
                            break;
                        case "Финансовый":
                            query = @"SELECT d.Name AS Водитель, 
                                    SUM(r.Distance * 10) AS Заработок -- Пример расчета
                                    FROM Routes r
                                    JOIN Drivers d ON r.DriverId = d.Id
                                    WHERE r.CreatedDate BETWEEN @StartDate AND @EndDate
                                    GROUP BY d.Name";
                            break;
                    }

                    if (!string.IsNullOrEmpty(query))
                    {
                        using (SqlCommand command = new SqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@StartDate", startDate);
                            command.Parameters.AddWithValue("@EndDate", endDate.AddDays(1)); // Чтобы включить весь последний день

                            SqlDataAdapter adapter = new SqlDataAdapter(command);
                            adapter.Fill(result);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка генерации отчета: {ex.Message}");
            }

            return result;
        }
    }
}