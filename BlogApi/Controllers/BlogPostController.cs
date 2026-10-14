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

        // GET - Összes blogbejegyzés
        [HttpGet("posts")]
        public object GetAllPosts()
        {
            List<BlogPost> lista = new List<BlogPost>();

            var connector = new MySqlConnection(ConnectionString);
            connector.Open();

            string sql = "SELECT * FROM blogpost";

            var cmd = new MySqlCommand(sql, connector);
            var datareader = cmd.ExecuteReader();

            while (datareader.Read())
            {
                var post = new BlogPost
                {
                    Id = datareader.GetInt32(datareader.GetOrdinal("id")),
                    Title = datareader.GetString(datareader.GetOrdinal("title")),
                    Content = datareader.GetString(datareader.GetOrdinal("content")),
                    PostTime = datareader.GetDateTime(datareader.GetOrdinal("postTime")),
                    UpdateTime = datareader.GetDateTime(datareader.GetOrdinal("updateTime")),
                    BlogId = datareader.GetInt32(datareader.GetOrdinal("blogId"))
                };

                lista.Add(post);
            }

            connector.Close();

            return new { message = "Sikeres lekérdezés.", result = lista };
        }


        // GET - Egy blogbejegyzés ID alapján
        [HttpGet("byId/{id}")]
        public object GetPostById([FromRoute] int id)
        {
            var connector = new MySqlConnection(ConnectionString);
            connector.Open();

            string sql = @"SELECT * FROM blogpost WHERE id = @id";

            var cmd = new MySqlCommand(sql, connector);
            cmd.Parameters.AddWithValue("@id", id);

            var datareader = cmd.ExecuteReader();
            object result = null;

            if (datareader.Read())
            {
                var post = new BlogPost
                {
                    Id = datareader.GetInt32(datareader.GetOrdinal("id")),
                    Title = datareader.GetString(datareader.GetOrdinal("title")),
                    Content = datareader.GetString(datareader.GetOrdinal("content")),
                    PostTime = datareader.GetDateTime(datareader.GetOrdinal("postTime")),
                    UpdateTime = datareader.GetDateTime(datareader.GetOrdinal("updateTime")),
                    BlogId = datareader.GetInt32(datareader.GetOrdinal("blogId"))
                };

                result = new { message = "Sikeres lekérdezés.", result = post };
            }
            else
            {
                result = new { message = "Sikertelen lekérdezés.", result = "" };
            }

            connector.Close();

            return result;
        }


        // POST - Új blogbejegyzés
        [HttpPost("create")]
        public object AddNewPost([FromBody] BlogPost blogPost)
        {
            var connector = new MySqlConnection(ConnectionString);
            connector.Open();

            string sql = @"INSERT INTO blogpost
                           (title, content, postTime, updateTime, blogId)
                           VALUES
                           (@title, @content, @postTime, @updateTime, @blogId)";

            var cmd = new MySqlCommand(sql, connector);

            cmd.Parameters.AddWithValue("@title", blogPost.Title);
            cmd.Parameters.AddWithValue("@content", blogPost.Content);
            cmd.Parameters.AddWithValue("@postTime", DateTime.Now);
            cmd.Parameters.AddWithValue("@updateTime", DateTime.Now);
            cmd.Parameters.AddWithValue("@blogId", blogPost.BlogId);

            cmd.ExecuteNonQuery();

            connector.Close();

            return new { message = "Sikeres hozzáadás", result = blogPost };
        }


        // PUT - Blogbejegyzés módosítása
        [HttpPut("update/{id}")]
        public object UpdatePost([FromRoute] int id, [FromBody] BlogPost blogPost)
        {
            var connector = new MySqlConnection(ConnectionString);
            connector.Open();

            string sql = @"UPDATE blogpost
                           SET title = @title,
                               content = @content,
                               updateTime = @updateTime,
                               blogId = @blogId
                           WHERE id = @id";

            var cmd = new MySqlCommand(sql, connector);

            cmd.Parameters.AddWithValue("@title", blogPost.Title);
            cmd.Parameters.AddWithValue("@content", blogPost.Content);
            cmd.Parameters.AddWithValue("@updateTime", DateTime.Now);
            cmd.Parameters.AddWithValue("@blogId", blogPost.BlogId);
            cmd.Parameters.AddWithValue("@id", id);

            object result = null;

            if (cmd.ExecuteNonQuery() > 0)
            {
                result = StatusCode(200, new
                {
                    message = "Sikeres frissítés",
                    result = blogPost
                });
            }
            else
            {
                result = NotFound(new
                {
                    message = "Nincs ilyen bejegyzés",
                    result = ""
                });
            }

            connector.Close();

            return result;
        }


        // DELETE - Blogbejegyzés törlése
        [HttpDelete("delete")]
        public object DeletePost([FromQuery] int id)
        {
            var connector = new MySqlConnection(ConnectionString);
            connector.Open();

            string sql = @"DELETE FROM blogpost WHERE id = @id";

            var cmd = new MySqlCommand(sql, connector);
            cmd.Parameters.AddWithValue("@id", id);

            object result = null;

            if (cmd.ExecuteNonQuery() > 0)
            {
                result = StatusCode(200, new
                {
                    message = "Sikeres törlés",
                    result = id
                });
            }
            else
            {
                result = NotFound(new
                {
                    message = "Nincs ilyen bejegyzés",
                    result = ""
                });
            }

            connector.Close();

            return result;
        }





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


        [HttpGet("getBloggerPostsNumber/{id}")]
        public object GetBloggerPostsNumber([FromRoute] int id)
        {
            var connector = new MySqlConnection(ConnectionString);

            connector.Open();

            string sql = @"SELECT blogger.name, COUNT(blogpost.id) AS `BejegyzesekSzama`
                            FROM `blogger` 
                            INNER JOIN blogpost ON blogger.id = blogpost.blogId
                            WHERE blogger.id = @id;";

            var cmd = new MySqlCommand(sql, connector);

            cmd.Parameters.AddWithValue("@id", id);

            var datareader = cmd.ExecuteReader();
            object result = null;

            if (datareader.Read() && datareader != null)
            {
                var bloggerPosts = new NumberOfBloggerPostsDto
                {
                    Name = datareader.GetString(0),
                    Number = datareader.GetInt32(1)
                };

                result = new { message = "Sikeres lekérdezés.", result = bloggerPosts };
            }
            else
            {
                result = new { message = "Sikertelen lekérdezés.", result = "" };
            }



            connector.Close();

            return result;
        }
    }
}