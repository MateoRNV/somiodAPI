using somiod.DTOs;
using somiod.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Web.Http;
using static System.Net.Mime.MediaTypeNames;

namespace somiod.Controllers
{
    [RoutePrefix("api/applications/{applicationID}/modules")]
    public class ModulesController : ApiController
    {

        SqlConnection connection = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\somiodDB.mdf;Integrated Security=True");

        // GET: api/applications/{applicationId}/modules
        [Route("")]
        [HttpGet]
        public IEnumerable<ModuleClass> Get(string applicationID)
        {
            List<ModuleClass> modules = new List<ModuleClass>();
            string query = "SELECT * FROM dbo.Modules where parent = " +
                "(select id from dbo.Applications where name = @applicationID);";

            using (connection)
            {
                SqlCommand command = new SqlCommand(query, connection);
                try
                {
                    connection.Open();
                    command.Parameters.AddWithValue("@applicationID", applicationID);
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        ModuleClass moduleItem = new ModuleClass();

                        moduleItem.id = int.Parse(reader["id"].ToString());
                        moduleItem.name = reader["name"].ToString();
                        moduleItem.creation_dt = DateTime.Parse(reader["creation_dt"].ToString());
                        moduleItem.parent = int.Parse(reader["parent"].ToString());

                        modules.Add(moduleItem);

                    }
                    connection.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message.ToString());
                }
            }

            return  modules;
        }

        // GET: Prefix/modules/moduleID
        [Route("{moduleID}")]
        [HttpGet]
        public ModuleClass GetModule(string applicationID,string moduleID)
        {
            ModuleClass module = new ModuleClass();
            string query = "SELECT * FROM dbo.Modules where name = @NAME AND parent = " +
                "(select id from dbo.Applications where name = @applicationID);";

            using (connection)
            {
                SqlCommand command = new SqlCommand(query, connection);
                try
                {
                    connection.Open();
                    command.Parameters.AddWithValue("name", moduleID);
                    command.Parameters.AddWithValue("applicationID", applicationID);
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        module.id = int.Parse(reader["id"].ToString());
                        module.name = reader["name"].ToString();
                        module.creation_dt = DateTime.Parse(reader["creation_dt"].ToString());
                        module.parent = int.Parse(reader["parent"].ToString());

                    }
                    connection.Close();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                    connection.Close();
                }
            }

            return module;
        }

        // POST: /Modules
        [Route("")]
        [HttpPost]
        public IHttpActionResult Post(string applicationID, [FromBody]ModuleDTO moduleDTO)
        {
            string createCommand = "Insert into dbo.Modules (name, creation_dt, parent) output INSERTED.ID VALUES (@NAME, @DATE, @PARENT);";
            ModuleClass moduleCreated = new ModuleClass();

            using (connection)
            {
                SqlCommand command = new SqlCommand(createCommand, connection);

                try
                {
                    connection.Open();

                    int parent = getApplicationID(applicationID);
                    DateTime date = DateTime.Now;

                    command.CommandType = System.Data.CommandType.Text;
                    command.Parameters.AddWithValue("@NAME", moduleDTO.name);
                    command.Parameters.AddWithValue("@DATE", date);
                    command.Parameters.AddWithValue("@PARENT", parent);
                    int id = (int)command.ExecuteScalar();


                    moduleCreated.id = id;
                    moduleCreated.name = moduleDTO.name;
                    moduleCreated.creation_dt = date;
                    moduleCreated.parent = parent;

                    connection.Close();

                    return Ok(moduleCreated);

                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                    connection.Close();
                    return BadRequest(ex.Message);
                }
            }
        }


        // PUT: Prefix/modules/moduleID
        [Route("{moduleID}")]
        [HttpPut]
        public IHttpActionResult Put(string applicationID, string moduleID, [FromBody]ModuleDTO moduleDTO)
        {
            string updateCommand = "Update dbo.Modules SET name = (@NAME) WHERE id = @ID AND parent = " +
                "(select id from dbo.Applications where name = @applicationID);";
            ApplicationClass applicationCreated = new ApplicationClass();


            using (connection)
            {
                SqlCommand command = new SqlCommand(updateCommand, connection);

                try
                {
                    connection.Open();
                    command.CommandType = System.Data.CommandType.Text;
                    command.Parameters.AddWithValue("id", moduleID);
                    command.Parameters.AddWithValue("@NAME", moduleDTO.name);
                    command.Parameters.AddWithValue("applicationID", applicationID);

                    command.ExecuteNonQuery();
                    connection.Close();


                    return Ok();
                }
                catch (Exception ex)
                {
                    connection.Close();
                    return BadRequest(ex.Message);
                }
            }

        }

        // DELETE: api/Modules/5
        public void Delete(int id)
        {
        }



        private int getApplicationID(string name)
        {
            int id;
            string query = "select id from dbo.Applications where name = @NAME;";

                SqlCommand commandID = new SqlCommand(query, connection);


                commandID.Parameters.AddWithValue("@NAME", name);
                id = (int)commandID.ExecuteScalar();


            return id;

        }
    }

}
