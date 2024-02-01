using Dapper;
using GH_LCA;
using Grasshopper.Kernel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Reflection;
using System.IO;

namespace LCA_Toolbox.Database
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
                catch (InvalidOperationException e)
                {
                    Console.Write(e.Message);
                    outMaterial = new LCA_Material();
                    return false;
                }
            }
        }

        private static string LoadConnectionString()
        {
            string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            return $@"Data Source={path}\materialDB_20240130.db; Version = 3;";
        }
    }
}
