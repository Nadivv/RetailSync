using System;
using Npgsql;

var connString = "Host=localhost;Port=5432;Database=Final_Project_PBO;Username=postgres;Password=";
try
{
    using var conn = new NpgsqlConnection(connString);
    conn.Open();
    Console.WriteLine("Koneksi berhasil! Database: " + conn.Database);
}
catch (Exception ex)
{
    Console.WriteLine("Koneksi GAGAL: " + ex.Message);
}
