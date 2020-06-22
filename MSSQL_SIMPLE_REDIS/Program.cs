using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSSQL_SIMPLE
{
    class Program
    {
        public const string LANDING_QUERY_REDIS_KEY = "QueryResultLanding";

      
        class Employee
        {
            public int ID { get; set; }
            public string FirstName { get; set; }
        }

        static List<Employee> GetEmployeesFromDB()
        {
            //Command and Data Reader
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(@"Data Source=.;Initial Catalog=EmployeeDB;Integrated Security=True");
            cmd.Connection.Open();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "SELECT * FROM Employees";


            SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.Default);

            List<Employee> list = new List<Employee>();
            while (reader.Read() == true)
            {
                Console.WriteLine($" {reader["ID"]} {reader["FIRSTNAME"]} {reader["LASTNAME"]} {reader["SALARY"]}");
                Employee e = new Employee
                {
                    ID = (int)reader["ID"],
                    FirstName = reader["FIRSTNAME"].ToString()
                };
                list.Add(e);
            }

            cmd.Connection.Close();

            return list;
        }

        static List<Employee> GetEmployees()
        {
            // check if result is in redis and also time valid
            IRedisObject result = RedisDataAccess.GetWithTimeStamp(LANDING_QUERY_REDIS_KEY);
            if (result == null || DateTime.Now.Subtract(result.LastUpdateTime).Minutes > 30)
            {
                List<Employee> employees = GetEmployeesFromDB();

                string employeesJson = JsonConvert.SerializeObject(employees);
                RedisDataAccess.SaveWithTimeStamp(LANDING_QUERY_REDIS_KEY, employeesJson);

                return employees;
            }
            else
            {
                return JsonConvert.DeserializeObject<List<Employee>>(result.JsonData);
            }
        }

        // a nice thought ...
        // wrapper for DAO method which puts and gets the result from redis
        //static T GetEmployeesGeneric<T>()
        //{
        //    // check if result is in redis and also time valid
        //    IRedisObject result = RedisDataAccess.GetWithTimeStamp(LANDING_QUERY_REDIS_KEY);
        //    if (result == null || DateTime.Now.Subtract(result.LastUpdateTime).Minutes > 30)
        //    {
        //        T employees = GetEmployeesFromDB();

        //        string employeesJson = JsonConvert.SerializeObject(employees);
        //        RedisDataAccess.SaveWithTimeStamp(LANDING_QUERY_REDIS_KEY, employeesJson);

        //        return employees;
        //    }
        //    else
        //    {
        //        return JsonConvert.DeserializeObject<T>(result.JsonData);
        //    }
        //}


        static void Main(string[] args)
        {
            // Data Source=.;Initial Catalog=EmployeeDB;Integrated Security=True

            List<Employee> employees = GetEmployees();
            //string value = RedisDataAccess.Get("dotnet2206");


        }
    }
}
