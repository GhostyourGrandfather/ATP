using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATP
{
    public class Route
    {
        public int Id { get; set; }
        public string StartPoint { get; set; }
        public string EndPoint { get; set; }
        public decimal Distance { get; set; }
        public decimal TravelTime { get; set; }
        public int DriverId { get; set; }
        public int VehicleId { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsCompleted { get; set; }

        // Навигационные свойства
        public Driver Driver { get; set; }
        public Vehicle Vehicle { get; set; }
    }
}
