using System;
using System.Data.SqlClient;

namespace ATP
{
    public class Route
    {
        public int Id { get; set; }
        public string StartPoint { get; set; }
        public string EndPoint { get; set; }
        public decimal Distance { get; set; }
        public decimal TravelTime { get; set; }
        public int DriverId { get; set; } // Изменено с Driver на DriverId для работы с БД
        public int VehicleId { get; set; } // Изменено с Vehicle на VehicleId для работы с БД
        public string Status { get; set; } = "Назначен";
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Навигационные свойства (не используются в запросах к БД)
        public Driver Driver { get; set; }
        public Vehicle Vehicle { get; set; }

        // Метод для загрузки маршрутов из DataReader
        public static Route FromDataReader(SqlDataReader reader)
        {
            return new Route
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                StartPoint = reader.GetString(reader.GetOrdinal("StartPoint")),
                EndPoint = reader.GetString(reader.GetOrdinal("EndPoint")),
                Distance = reader.GetDecimal(reader.GetOrdinal("Distance")),
                TravelTime = reader.GetDecimal(reader.GetOrdinal("TravelTime")),
                DriverId = reader.GetInt32(reader.GetOrdinal("DriverId")),
                VehicleId = reader.GetInt32(reader.GetOrdinal("VehicleId")),
                Status = reader.GetString(reader.GetOrdinal("Status")),
                CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate"))
            };
        }

        // Метод для обновления статуса маршрута в БД
        public static bool UpdateStatus(int routeId, string newStatus)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(App.ConnectionString))
                {
                    connection.Open();
                    string query = "UPDATE Routes SET Status = @Status WHERE Id = @RouteId";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Status", newStatus);
                        command.Parameters.AddWithValue("@RouteId", routeId);
                        return command.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch
            {
                return false;
            }
        }
    }
}