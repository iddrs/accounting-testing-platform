using System.Data;
using Npgsql;
using System.Windows;

namespace AccountingTestingPlatform.NaturezaSaldos;

internal class NaturezaSaldo
{

    public static DataTable MakeNaturezaSaldoTable(NpgsqlConnection connection)
    {
        DataTable dt = new DataTable("tblNaturezaSaldo");
        DataColumn column;
        DataRow row;

        column = new DataColumn();
        //column.DataType = Type.GetType("System.String");
        column.DataType = typeof(string);
        column.ColumnName = "conta_contabil";
        column.Caption = "Conta contábil";
        column.AutoIncrement = false;
        column.ReadOnly = false;
        column.Unique = true;
        dt.Columns.Add(column);
        
        column = new DataColumn();
        //column.DataType = Type.GetType("System.Boolean");
        column.DataType = typeof(bool);
        column.ColumnName = "devedora";
        column.Caption = "Devedora";
        column.AutoIncrement = false;
        column.ReadOnly = false;
        column.Unique = false;
        column.DefaultValue = false;
        dt.Columns.Add(column);
        
        column = new DataColumn();
        //column.DataType = Type.GetType("System.Boolean");
        column.DataType = typeof(bool);
        column.ColumnName = "credora";
        column.Caption = "Credora";
        column.AutoIncrement = false;
        column.ReadOnly = false;
        column.Unique = false;
        column.DefaultValue = false;
        dt.Columns.Add(column);
        
        DataColumn[] PrimaryKeyColumns = new DataColumn[1];
        PrimaryKeyColumns[0] = dt.Columns["conta_contabil"];
        dt.PrimaryKey = PrimaryKeyColumns;

        using (NpgsqlCommand cmd = new NpgsqlCommand("select * from auxiliar.natureza_saldos order by conta_contabil asc", connection))
        using(NpgsqlDataReader reader = cmd.ExecuteReader())
        {
            dt.Load(reader);
        }

        return dt;
    }

    public static void Update(DataTable dt, NpgsqlConnection connection)
    {
        DataTable added = dt?.GetChanges(DataRowState.Added);
        DataTable edited = dt?.GetChanges(DataRowState.Modified);
        DataTable deleted = dt?.GetChanges(DataRowState.Deleted);

        NpgsqlTransaction transaction = connection.BeginTransaction();
        string cc;
        int devedora;
        int credora;
        if(added != null)
        {
            foreach (DataRow row in added.Rows)
            {
                cc = row["conta_contabil"].ToString();
                devedora = (row["devedora"].ToString() == "True") ? 1 : 0;
                credora = (row["credora"].ToString() == "True") ? 1 : 0;
                string sql = $"insert into auxiliar.natureza_saldos (conta_contabil, devedora, credora) values('{cc}', {devedora}, {credora});";
                using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }
        if(edited != null)
        {
            foreach (DataRow row in edited.Rows)
            {
                cc = row["conta_contabil"].ToString();
                devedora = (row["devedora"].ToString() == "True") ? 1 : 0;
                credora = (row["credora"].ToString() == "True") ? 1 : 0;
                string sql = $"update auxiliar.natureza_saldos set devedora = {devedora}, credora = {credora} where conta_contabil like '{cc}';";
                using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }
        if(deleted != null)
        {
            foreach (DataRow row in deleted.Rows)
            {
                cc = row["conta_contabil", DataRowVersion.Original].ToString();
                string sql = $"delete from auxiliar.natureza_saldos where conta_contabil like '{cc}';";
                using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }
        transaction.Commit();
        MessageBox.Show("Naturezas de saldos atualizadas.");
    }
}
