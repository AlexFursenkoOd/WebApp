using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Configuration;
using Dapper;

namespace WebApplication2.Services
{
    public static class RepositoryService
    {
        public static SqlConnection Sql()
        {
            var connectionStringSettings = ConfigurationManager.ConnectionStrings["Extensible"];
            var connectionString = connectionStringSettings.ConnectionString;
            return new SqlConnection(connectionString);
        }

        public static void CreateDefaultObject(string objectName)
        {
            using (var context = Sql())
            {
                try
                {
                    var beforeUpdateTriggerName = objectName + "BeforeUpdateTrigger";
                    var beforeInsertTriggerName = objectName + "BeforeInsertTrigger";
                    context.Execute($@"
                        IF (NOT EXISTS (SELECT * 
                            FROM INFORMATION_SCHEMA.TABLES 
                            WHERE  TABLE_NAME = '{objectName}'))
                        CREATE TABLE {objectName}(
                        	Id varchar(20) DEFAULT CONCAT('{objectName}', next value for Minor_ID),
	                        CreatedDate datetime DEFAULT GETDATE(),
	                        CreatedBy nvarchar(100) DEFAULT CURRENT_USER,
	                        ModifiedDate datetime,
	                        ModifiedBy nvarchar(100)
                        )");
                    context.Execute($@"
                        IF EXISTS (select * from sys.objects where type = 'TR' and name = '{beforeUpdateTriggerName}')
                        DROP TRIGGER {beforeUpdateTriggerName}");
                    context.Execute($@"
                        	CREATE TRIGGER {beforeUpdateTriggerName} ON {objectName}
                        	AFTER UPDATE
                        	AS BEGIN 
                        		UPDATE {objectName}
                        		SET ModifiedBy = CURRENT_USER
                        			,ModifiedDate = GETDATE()
                        		FROM {objectName}
                        		INNER JOIN inserted i on i.Id = {objectName}.Id
                        	END 
                        ");
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }

        public static IEnumerable<T> Select<T>(string query)
        {
            using (var context = Sql())
            {
                var result = context.Query<T>(query);
                return result;
            }
        }

        public static T GetByID<T>(string id)
        {
            if (id == null) throw new ArgumentNullException(id);
            using (var context = Sql())
            {
                var table = id.Substring(0, id.Length - 5);
                var result = context.Query<T>($@"
                    SELECT
                        *
                    FROM
                        {table}
                    WHERE
                        ID = @id"
                ,new { id = id }).FirstOrDefault();
                return result;
            }
        }

        public static void Insert(string query)
        {
            using (var context = Sql())
            {
                context.Execute(query);
            }
        }

        public static void Update(string query)
        {
            using (var context = Sql())
            {
                context.Execute(query);
            }
        }

        public static void AddField(string objectName, string fieldName, string type)
        {
            using (var context = Sql())
            {
                try
                {
                    context.Execute($"ALTER TABLE {objectName} ADD {fieldName} {type}");
                }
                catch (SqlException)
                {

                }
            }
        }

        public static void DeleteField(string objectName, string fieldName)
        {
            using (var context = Sql())
            {
                context.Execute($"ALTER TABLE {objectName} DROP COLUMN {fieldName}");
            }
        }
    }
}