using Dapper;
using LCA_Toolbox;
using Grasshopper.Kernel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Reflection;
using System.IO;
using System.Data.SqlClient;
using System.Data.Common;
//using static Grasshopper.DataTree<T>;

namespace LCA_Toolbox.Database
{
    public class SqliteDataAcces
    {
        string db_path = string.Empty;



        #region INIT 
        public SqliteDataAcces(string _db_path)
        {
            if (_db_path == "pyramiden")
            {
                this.db_path = GetPathPytamidenDB_path();
            }
            else
            {

                this.db_path = _db_path;
            }


        }
        public SqliteDataAcces(string folder_path,string _dbName)
        {
            string _db_path = $@"{folder_path}\{_dbName}";
            this.db_path = _db_path;
        }
        public SqliteDataAcces()
        {
        }

        #endregion INIT

        #region ConnectionString
        public void SetDB_path(string folder_path, string _dbName)
        {
            string _db_path = $@"{folder_path}\{_dbName}";
            this.db_path = _db_path;
        }
        public void SetDB_path(string _dbPath)
        {
            this.db_path = _dbPath;
        }

        public string Get_DB_path()
        {
            return db_path;
        }

        public string GetPathPytamidenDB_path()
        {
            string folder_path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string path = $@"{folder_path}\materialDB_20240220.db";
            return path;

        }


        public string LoadConnectionStringPublic { get => LoadConnectionString(); }
        private string LoadConnectionString()
        {

            return $@"Data Source={db_path}; Version = 3;";
        }

        #endregion ConnectionString



        #region Create and edit db

        public void CreateDB(string _path)
        {
            SQLiteConnection.CreateFile(_path);
        }

        public string AddToDB(LCA_Material material, bool overwrite)
        {

            try
            {
                using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
                {
                    string sql = overwrite ? "REPLACE INTO " : "INSERT INTO ";
                    sql += $"materials (name,category,density,insulation,description,A1_A3,ODB,POCP,EP,AP,C1_C4,DataSource,Notes)" +
                        $" VALUES ('{material.Name}','{material.Category}','{material.Density}','{material.A1toA3}','{material.Insulation}','{material.Description}','{material.ODP}','{material.POCP}','{material.EP}','{material.AP}','{material.C1toC4}','{material.DataSource}','{material.Notes}')";


                    cnn.Execute(sql);
                }
                

            }
            catch(SQLiteException e)
            {
                if(e.ErrorCode == 19  && e.ToString().Contains("UNIQUE")) { return "Duplicate , to replace set overwrite to true. ";}

                return e.ToString();
            }

            return "No error";
        }

        #endregion Create and edit db


        #region Get data from DB
        public List<string> GetMaterialGroups()
        {

            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<string>("SELECT DISTINCT category FROM materials", new DynamicParameters());
                return output.ToList();
            }

        }
        public List<LCA_Material> GetMaterials()
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<LCA_Material>("SELECT * FROM materials", new DynamicParameters());
                return output.ToList();
            }
        }
        public List<LCA_Material> GetMaterialsByGroup(string group)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                string sqlString = $"SELECT * FROM materials WHERE category = \"{group}\"";
                var output = cnn.Query<LCA_Material>(sqlString, new DynamicParameters());
                return output.ToList();
            }
        }

        public bool GetMaterialByName(string name, out LCA_Material outMaterial)
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

        #endregion Get data from DB





    }
}
