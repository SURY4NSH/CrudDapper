using System.Data;
using System.Data.SqlClient;
using Dapper;
using StackExchange.Redis; 
using Microsoft.AspNetCore.Mvc;

namespace CrudDapper.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SuperheroController : Controller
    {
        private readonly string _connectionString;
        public SuperheroController(IConfiguration config)
        {
            _config = config;
               _connectionString = _config.GetConnectionString("DefaultConnection");
    }
    public IDbConnection CreateConnection()
        => new SqlConnection(_connectionString);

        private readonly IConfiguration _config;
        [HttpGet]
        public async Task<ActionResult<List<Superhero>>> getAll()
        {
            using var connection = new SqlConnection(_connectionString);
           // ConnectionMultiplexer redisConn = ConnectionMultiplexer.Connect("Server=SURYANSH-ARORA\\SQLEXPRESS; Initial catalog=dapper; Integrated Security=True");
            //IDatabase redDb = redisConn.GetDatabase();
            var heroes = await connection.QueryFirstAsync<Superhero>("Select * from Superhero" );
            //var heroes =await redDb.HashSetAsync("Superhero",Superhero );
            return Ok(heroes);
        }


        private static async Task<IEnumerable<Superhero>> NewMethod(SqlConnection connection)
        {
            return await connection.QueryAsync<Superhero>("Select * from Superhero");
        }

        [HttpGet("GetById")]
        public async Task<ActionResult<List<Superhero>>> getOne(int heroid)
        {
            using var connection = new SqlConnection(_connectionString);
            var heroes = await connection.QueryFirstAsync<Superhero>("Select * from Superhero where Id=@Id", new
            {
                Id = heroid
            });
            return Ok(heroes);
        }
        [HttpPost]
        public async Task<ActionResult<List<Superhero>>> Add(Superhero heroid)
        {
            var sp = "insert into Superhero(FirstName,LastName,Place) values (@FirstName , @LastName, @Place)";
            using var connection = new SqlConnection(_connectionString);
            var heroes = await connection.ExecuteAsync(sp, heroid);
            return Ok(await NewMethod(connection));
         //   var client = new AmazonSimpleNotificationServiceClient(Amazon.RegionEndpoint.USEast2);
           // SendMessage(client).Wait();
        }
        [HttpPut]
        public async Task<ActionResult<List<Superhero>>> Update(Superhero heroid)
        {
            using var connection = new SqlConnection(_connectionString);
            var heroes = await connection.ExecuteAsync("Update Superhero set FirstName=@FirstName ,LastName= @LastName,Place=@Place where Id= @Id", heroid);
            return Ok(await NewMethod(connection));
        }
        
       }
    }
