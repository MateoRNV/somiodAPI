﻿using somiod.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Results;
using System.Xml.Linq;

namespace somiod.Controllers
{
    public class ApplicationsController : ApiController
    {
        SqlConnection connection = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\somiodDB.mdf;Integrated Security=True");

        // GET: api/Application
        public IEnumerable<Application> Get()
        {
            List<Application> applications = new List<Application>();
            string query = "SELECT * FROM dbo.Applications;";

            using (connection)
            {
                SqlCommand command = new SqlCommand(query, connection);
                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        var applicationsItem = new Application();

                        applicationsItem.id = int.Parse(reader["id"].ToString());
                        applicationsItem.name = reader["name"].ToString();
                        applications.Add(applicationsItem);

                    }
                    connection.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message.ToString());
                }
            }

            return applications;
        }

        // GET: api/Application/5
        public Application Get(int id)
        {
            Application application = new Application();
            string query = "SELECT * FROM dbo.Applications where id = @ID;";

            using (connection)
            {
                SqlCommand command = new SqlCommand(query, connection);
                try
                {
                    connection.Open();
                    command.Parameters.AddWithValue("id", id);
                    SqlDataReader reader = command.ExecuteReader();
                    while(reader.Read())
                    {
                        application.id = int.Parse(reader["id"].ToString());
                        application.name = reader["name"].ToString();
                        application.creation_dt = DateTime.Parse(reader["creation_dt"].ToString());

                    }
                    connection.Close();
                }
                catch(Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                    connection.Close();
                }
            }



            return application;
        }

        // POST: api/Application
        public IHttpActionResult PostApplication([FromBody]ApplicationDTO applicationReq)
        {

            string createCommand = "Insert into dbo.Applications (name, creation_dt) output INSERTED.ID VALUES (@NAME, @DATE);";
            Application applicationCreated = new Application();

            using (connection)
            {
                SqlCommand command = new SqlCommand(createCommand, connection);

                try
                {
                    connection.Open();

                    DateTime date = DateTime.Now;

                    command.CommandType = System.Data.CommandType.Text;
                    command.Parameters.AddWithValue("@NAME", applicationReq.name);
                    command.Parameters.AddWithValue("@DATE", date);
                    int id = (int)command.ExecuteScalar();


                    applicationCreated.id = id;
                    applicationCreated.name = applicationReq.name;
                    applicationCreated.creation_dt = date;

                    //System.Diagnostics.Debug.WriteLine("test "+ date);
                    connection.Close();

                    return Ok(applicationCreated);  
                    
                }catch(Exception ex) { System.Diagnostics.Debug.WriteLine(ex.Message);
                    connection.Close();
                    return BadRequest(ex.Message);
                }
            }

        }

        // PUT: api/Application/5
        public IHttpActionResult Put(int id, [FromBody]ApplicationDTO applicationDTO)
        {
            string updateCommand = "Update dbo.Applications SET name = (@NAME) where id = @ID;";
            Application applicationCreated = new Application();


            using (connection)
            {
                SqlCommand command = new SqlCommand(updateCommand, connection);

                try
                {
                    connection.Open();
                    command.CommandType = System.Data.CommandType.Text;
                    command.Parameters.AddWithValue("id", id);
                    command.Parameters.AddWithValue("@NAME", applicationDTO.name.ToString());

                    command.ExecuteNonQuery();
                    connection.Close();


                    return Ok();
                }catch(Exception ex)
                {
                    connection.Close( );
                    return BadRequest(ex.Message);
                }
            }
                
        }

        // DELETE: api/Application/5
        public IHttpActionResult Delete(int id)
        {
            string deleteCommand = "Delete from dbo.Applications where id = @ID;";
            Application applicationCreated = new Application();

            using (connection)
            {
                SqlCommand command = new SqlCommand(deleteCommand, connection);

                try
                {
                    connection.Open( );
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("id", id);
                    command.ExecuteNonQuery();
                    connection.Close();

                    return Ok();
                }catch(Exception ex)
                {
                    connection.Close( );
                    return BadRequest(ex.Message);
                }
            }
        }
    }
}
