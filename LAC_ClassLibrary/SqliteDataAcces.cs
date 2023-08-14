using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;

namespace GH_LCA
{
    public class SqliteDataAcces
    {
        public static List<string> GetMaterialGroups()
        {

            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<string>("SELECT DISTINCT category FROM materials", new DynamicParameters());
                return output.ToList();
            }

        }
        public static List<LCA_Material> GetMaterials()
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<LCA_Material>("SELECT * FROM materials", new DynamicParameters());
                return output.ToList();
            }
        }
        public static List<LCA_Material> GetMaterialsByGroup(string group)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                string sqlString = $"SELECT * FROM materials WHERE category = \"{group}\"";
                var output = cnn.Query<LCA_Material>(sqlString, new DynamicParameters());
                return output.ToList();
            }
        }

        public static bool GetMaterialByName(string name, out LCA_Material outMaterial)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                string sqlString = $"SELECT * FROM materials WHERE name = \"{name}\"";
                try
                {

                    outMaterial = cnn.QueryFirst<LCA_Material>(sqlString, new DynamicParameters());
                    return true;

                }
                catch (System.InvalidOperationException e)
                {
                    Console.Write(e.Message);
                    outMaterial = new LCA_Material();
                    return false;
                }
            }
        }

        private static string LoadConnectionString()
        {
            return @"Data Source =.\materialDB_dev.db; Version = 3;";
            //return ConfigurationManager.ConnectionStrings["Sqlite"].ConnectionString;
        }
    }
}
