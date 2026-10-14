using BlogApi.Models;
using BlogApi.Models.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;

namespace BlogApi.Controllers
{
    [Route("blogpost")]
    [ApiController]
    public class BlogPostController : ControllerBase
    {
        public readonly string ConnectionString = "server=localhost;database=blog;user=root;password=";

        [HttpGet("getAllBloggerAndPost")]
        public object GetAllBloggerAndPost([FromQuery] int id)
        {
            List<BlogAndPostDto> lista = new List<BlogAndPostDto>();
            var connector = new MySqlConnection(ConnectionString);

            connector.Open();

            string sql = @"SELECT blogger.name, blogpost.title, blogpost.content
                            FROM blogger
                            INNER JOIN blogpost ON blogger.id = blogpost.blogId
                            WHERE blogger.id = @id;";

            var cmd = new MySqlCommand(sql, connector);

            cmd.Parameters.AddWithValue("@id", id);

            var datareader = cmd.ExecuteReader();

            while (datareader.Read())
            {
                var blogandpost = new BlogAndPostDto
                {
                    Name = datareader.GetString(0),
                    Title = datareader.GetString(1),
                    Content = datareader.GetString(2)
                };

                lista.Add(blogandpost);
            }

            connector.Close();

            return new { message = "Sikeres lekérdezés.", result = lista };
        }

        [HttpGet("getNumberOfPosts")]
        public object NumberOfPosts()
        {
            var connector = new MySqlConnection(ConnectionString);

            connector.Open();

            string sql = @"SELECT COUNT(*) FROM blogpost";

            var cmd = new MySqlCommand(sql, connector);

            var datareader = cmd.ExecuteReader();

            datareader.Read();

            var db = datareader.GetInt32(0);

            connector.Close();

            return new { message = "Sikeres lekérdezés.", result = db };
        }


    }
}