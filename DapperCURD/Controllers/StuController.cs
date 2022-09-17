using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Data.SqlClient;
using Dapper;

namespace DapperCURD.Controllers
{

    [ApiController]
    [Route("[controller]")]

    public class StuController : Controller
    {
        private readonly IConfiguration _config;
        public StuController(IConfiguration config)
        {
            _config = config;
        }


        [HttpGet]
        public async Task<ActionResult<List<Stu>>> GetAllStudents()
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            IEnumerable<Stu> stu = await SelectAllStudents(connection);
            return Ok(stu);
        }

       

        [HttpGet("{StuId}")]
        public async Task<ActionResult<List<Stu>>> GetStu(int StuId)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            var stuid = await connection.QueryFirstAsync<Stu>("Select * from Students where StuId = @Id",
                new {Id = StuId});
            return Ok(stuid);
        }

        [HttpPost]
        public async Task<ActionResult<List<Stu>>> CreateStu(Stu stu)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            await connection.ExecuteAsync("insert into Students(StuName, Email, City) values(@StuName, @Email, @City)",stu);
            return Ok(await SelectAllStudents(connection));
        }

        [HttpPut]
        public async Task<ActionResult<List<Stu>>> UpdateStu(Stu stu)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            await connection.ExecuteAsync("update Students set StuName = @StuName, Email = @Email, City =@City  where StuId = @StuId", stu);
            return Ok(await SelectAllStudents(connection));
        }


        [HttpDelete("{StuId}")]
        public async Task<ActionResult<List<Stu>>> DeleteStu(int StuId)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            await connection.ExecuteAsync("delete from Students where StuId = @Id", new { Id = StuId });
            return Ok(await SelectAllStudents(connection));
        }


        private static async Task<IEnumerable<Stu>> SelectAllStudents(SqlConnection connection)
        {
            return await connection.QueryAsync<Stu>("Select * from Students");
        }


    }
}
