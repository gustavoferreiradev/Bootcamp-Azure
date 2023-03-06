using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using gamecatalog.Models;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace gamecatalog.Controllers
{
    public class GameController : Controller
    {

        static BlobContainerClient blobContainer;

        private readonly IConfiguration _configuration;

        public GameController(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        // GET: Game
        public ActionResult Index(int _console)
        {
            var model = new List<Game>();
            
            string connectionString = _configuration.GetValue<string>("ConnectionStrings:AzureSQL");

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("SELECT game.id, game.name, consoles.console, game.descricao, game.data_compra, game.finalizado, game.data_finalizacao, game.id_console FROM game INNER JOIN consoles on game.id_console = consoles.id where consoles.id = " + _console.ToString());
                    cmd.Connection = con;

                    SqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        Game games = new Game();

                        games.id = Convert.ToInt32(rdr["id"]);
                        games.console = rdr["console"].ToString();
                        games.consoleid = Convert.ToInt32(rdr["id_console"]);
                        games.nome = rdr["name"].ToString();
                        games.descricao = rdr["descricao"].ToString();
                        games.datacompra = Convert.ToDateTime(rdr["data_compra"]);
                        games.finalizado = Convert.ToBoolean(rdr["finalizado"]);
                        games.datafinalizacao = Convert.ToDateTime(rdr["data_finalizacao"]);

                        model.Add(games);
                    }
                    con.Close();
                }
            }
            catch (SqlException)
            {
                return View("Houve um erro ao tentar acessar o Banco de dados"); //tratamento em caso de erro
            }

            return View(model);
        }

        // GET: Game/Details/5
        public async Task<ActionResult> Details(int id)
        {
            var model = new List<Game>();

            string connectionString = _configuration.GetValue<string>("ConnectionStrings:AzureSQL");  

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("SELECT game.id, game.name, consoles.console, game.descricao, game.data_compra, game.finalizado, game.data_finalizacao, game.thumb, game.id_console FROM game INNER JOIN consoles on game.id_console = consoles.id where game.id = " + id.ToString());
                    cmd.Connection = con;

                    SqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        Game games = new Game();

                        games.id = Convert.ToInt32(rdr["id"]);
                        games.console = rdr["console"].ToString();
                        games.consoleid = Convert.ToInt32(rdr["id_console"]);
                        games.nome = rdr["name"].ToString();
                        games.descricao = rdr["descricao"].ToString();
                        games.datacompra = Convert.ToDateTime(rdr["data_compra"]);
                        games.finalizado = Convert.ToBoolean(rdr["finalizado"]);
                        games.datafinalizacao = Convert.ToDateTime(rdr["data_finalizacao"]);
                        games.thumb = rdr["thumb"].ToString();

                        var blobs = new List<Blobs>();

                        blobs = await getcontainer();

                        foreach(Blobs blob in blobs)
                        {
                            if (blob.name == games.thumb)
                            {
                                games.URLthumb = blob.URL;
                            }
                        }

                        model.Add(games);
                    }
                    con.Close();
                }
            }
            catch (SqlException)
            {
                return View("Houve um erro ao tentar acessar o Banco de dados"); //tratamento em caso de erro
            }

            return View(model);
        }


        // GET: Game/Edit/5
        public ActionResult Edit(int id)
        {
            var model = new Game();

            string connectionString = _configuration.GetValue<string>("ConnectionStrings:AzureSQL");

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("SELECT game.id, game.name, consoles.console, game.id_console, game.descricao, game.data_compra, game.finalizado, game.data_finalizacao FROM game INNER JOIN consoles on game.id_console = consoles.id where game.id = " + id.ToString());
                    cmd.Connection = con;

                    SqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        //Game games = new Game();

                        model.id = Convert.ToInt32(rdr["id"]);
                        model.console = rdr["console"].ToString();
                        model.nome = rdr["name"].ToString();
                        model.descricao = rdr["descricao"].ToString();
                        model.datacompra = Convert.ToDateTime(rdr["data_compra"]);
                        model.finalizado = Convert.ToBoolean(rdr["finalizado"]);
                        model.datafinalizacao = Convert.ToDateTime(rdr["data_finalizacao"]);
                        model.consoleid = Convert.ToInt32(rdr["id_console"]);

                        //model. Add(games);                        
                    }
                    con.Close();
                }
            }
            catch (SqlException)
            {
                return View("Houve um erro ao tentar acessar o Banco de dados"); //tratamento em caso de erro
            }

            return View(model);
        }

        // POST: Game/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            var model = new Game();

            string connectionString = _configuration.GetValue<string>("ConnectionStrings:AzureSQL");

            try
            {
                // TODO: Add update logic here

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();

                    model.nome = "test";
                    model.descricao = "Oitavo jogo da saga RE 3";
                    model.datacompra = Convert.ToDateTime("01/01/2023");
                    model.finalizado = Convert.ToBoolean("True");
                    model.datafinalizacao = Convert.ToDateTime("01/01/2023");
                    model.consoleid = Convert.ToInt32("1");

                    string query = "UPDATE game SET descricao = " + model.descricao + " Where id = " + id;

                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.ExecuteNonQuery();

                    con.Close();
                }

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }


        // GET: Game/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Game/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }


        // GET: Game/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Game/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        //Get Container
        public async Task<List<Blobs>> getcontainer()
        {
            string storageconnectionstring = _configuration.GetValue<string>("ConnectionStrings:AzureBlobStorage");
            string blobContainername = _configuration.GetValue<string>("Container:name");


            BlobServiceClient blobServiceClient = new BlobServiceClient(storageconnectionstring);

            blobContainer = blobServiceClient.GetBlobContainerClient(blobContainername);
            await blobContainer.CreateIfNotExistsAsync(PublicAccessType.None);

            var blobs = new List<Blobs>();
            foreach (BlobItem blob in blobContainer.GetBlobs())
            {
                if (blob.Properties.BlobType == BlobType.Block)

                    blobs.Add(new Blobs { URL = blobContainer.GetBlobClient(blob.Name).Uri.ToString(), name = blob.Name });
            }

            return blobs;
        }
    }
}