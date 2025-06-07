using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace ATP
{
    public partial class App : Application
    {
        public static string ConnectionString { get; } = "Data Source=(local);Initial Catalog=ATP_Management;Integrated Security=True";
    }
}