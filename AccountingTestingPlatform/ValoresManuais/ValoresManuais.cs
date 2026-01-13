using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Windows;
using System.Globalization;

namespace AccountingTestingPlatform.ValoresManuais;

internal class ValoresManuais
{

    public static DataTable MakeValoresManuaisTable(NpgsqlConnection connection, string remessa, string entidade)
    {
        DataTable dt = new DataTable();
        DataColumn column;

        column = new DataColumn();
        column.DataType = typeof(string);
        column.ColumnName = "remessa";
        column.Caption = "Remessa";
        column.ReadOnly = true;
        column.Unique = false;
        dt.Columns.Add(column);
        
        column = new DataColumn();
        column.DataType = typeof(string);
        column.ColumnName = "entidade";
        column.Caption = "entidade";
        column.ReadOnly = true;
        column.Unique = false;
        dt.Columns.Add(column);
        
        column = new DataColumn();
        column.DataType = typeof(string);
        column.ColumnName = "item";
        column.Caption = "Item";
        column.ReadOnly = true;
        column.Unique = true;
        dt.Columns.Add(column);

        column = new DataColumn();
        column.DataType = typeof(decimal);
        column.ColumnName = "valor";
        column.Caption = "Valor";
        column.AutoIncrement = false;
        column.ReadOnly = false;
        column.Unique = false;
        column.DefaultValue = 0m;
        dt.Columns.Add(column);

        DataColumn[] PrimaryKeyColumns = new DataColumn[3];
        PrimaryKeyColumns[0] = dt.Columns["remessa"];
        PrimaryKeyColumns[1] = dt.Columns["entidade"];
        PrimaryKeyColumns[2] = dt.Columns["item"];
        dt.PrimaryKey = PrimaryKeyColumns;

        InsertValoresManuaisFor(connection, remessa, entidade);

        string sql = $"select remessa, entidade, item, valor from auxiliar.dados_manuais where remessa = {remessa} and entidade like '{entidade}' order by ordem asc;";
        using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
        using (NpgsqlDataReader reader = cmd.ExecuteReader())
        dt.Load(reader);

        return dt;
    }

    private static bool ExistsRemessa(NpgsqlConnection connection, string remessa, string entidade)
    {
        string sql = $"select * from auxiliar.dados_manuais where remessa = {remessa} and entidade like '{entidade}'";
        using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
        using (NpgsqlDataReader reader = cmd.ExecuteReader())
            return reader.HasRows;
    }
    private static void InsertValoresManuaisFor(NpgsqlConnection connection, string remessa, string entidade)
    {
        if (ExistsRemessa(connection, remessa, entidade)) return;

        string prevRemessa = GetPreviousRemessa(remessa);
        List<string> values = new();

        using (NpgsqlCommand cmd = new NpgsqlCommand($"select * from auxiliar.dados_manuais where remessa = {prevRemessa} and entidade like '{entidade}' order by ordem asc", connection))
        using (NpgsqlDataReader reader = cmd.ExecuteReader())
        {
            while (reader.Read())
            {
                string item = reader.GetString(2);
                double valor = reader.GetDouble(3);
                int ordem = reader.GetInt16(4);
                values.Add($"({remessa}, '{entidade}', '{item}', '{valor.ToString("F2", new CultureInfo("en-US"))}', {ordem})");
            }
        }
        string sql = $"insert into auxiliar.dados_manuais (remessa, entidade, item, valor, ordem) values {string.Join(", ", values)};";
        using (NpgsqlCommand cmdInsert = new NpgsqlCommand(sql, connection))
            cmdInsert.ExecuteNonQuery();
    }

    private static string GetPreviousRemessa(string remessa)
    {
        if (remessa.Length != 6)
        {
            throw new ArgumentException($"O valor '{remessa}' informado não está no formado adequado (AAAAMM ex.: 202509)!");

        }
        int year = int.Parse(remessa.Substring(0, 4));
        int month = int.Parse(remessa.Substring(4, 2));

        month--;
        if (month == 0)
        {
            month = 12;
            year--;
        }

        return $"{year:D4}{month:D2}";
    }


    public static void Update(DataTable dt, NpgsqlConnection connection)
    {
        DataTable edited = dt?.GetChanges(DataRowState.Modified);

        NpgsqlTransaction transaction = connection.BeginTransaction();
        string remessa;
        string entidade;
        string item;
        double valor;
        if (edited != null)
        {
            foreach (DataRow row in edited.Rows)
            {
                remessa = row["remessa"].ToString();
                entidade = row["entidade"].ToString();
                item = row["item"].ToString();
                valor = double.Parse(row["valor"].ToString());
                string sql = $"update auxiliar.dados_manuais set valor = {valor.ToString("F2", new CultureInfo("en-US"))} where remessa = {remessa} and entidade like '{entidade}' and item like '{item}';";
                using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }
        transaction.Commit();
        MessageBox.Show("Valores manuais atualizados.");
    }

}
