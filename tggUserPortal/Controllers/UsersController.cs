using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using tggUserPortal.Model;
using tggUserPortal.Objects;

namespace tggUserPortal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {

        private readonly DataContext _context;
        private readonly IConfiguration _configuration;

        public static User usert =new User();

        public UsersController(DataContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }


        //Get all users
        [HttpGet]
        public async Task<ActionResult<List<UsersDto>>> listUsers()  
        {
            return Ok(await _context.Users.ToListAsync());
        }

        //Get the specified user
        [HttpGet("{id}")]
        public async Task<ActionResult<List<UsersDto>>> Get(int id)
        {
            var user=await _context.Users.FindAsync(id);
            if (user == null)
                return BadRequest("User not found");

            return Ok(user);
        }


        //Register a new user
        [HttpPost("register")]
        public async Task<ActionResult<List<UsersDto>>> Register(UsersDto userreq) 
        {
            CreatePswxHash(userreq.Password, out byte[] pswxHash, out byte[] pswxSalt);

            usert.UserName = userreq.UserName;
            usert.PwHash = pswxHash;
            usert.PwSalt = pswxSalt;

            _context.Users.Add(userreq);
            await _context.SaveChangesAsync();


            return Ok(usert); 
        }
        private void CreatePswxHash(string pswx,out byte[] pswxHash, out byte[] pswxSalt)
        {
            using(var hmac=new HMACSHA512())
            {
                pswxSalt = hmac.Key;
                pswxHash=hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(pswx));
            }
        }

        //User Login
        [HttpPost("login")]
        public async Task<ActionResult<List<UsersDto>>> Login(UsersDto usereq)
        {
            var usertemp = await _context.Users.Where(u => u.UserName == usereq.UserName).ToListAsync();

            if (usertemp.Count == 0)
            { 
                return BadRequest("User not found!");
            }

            CreatePswxHash(usertemp[0].Password, out byte[] pswxHash, out byte[] pswxSalt);

            if (!VerifyPswxHash(usereq.Password, pswxHash, pswxSalt))
            {
                return BadRequest("Wrong Password");
            }

            string token = CreateToken(usertemp[0]);
            return Ok(token);
        }

        private string CreateToken(UsersDto user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
            };

            var key=new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value));

            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: cred);
            var jwt=new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }


        private bool VerifyPswxHash(string pswx, byte[] pswxHash, byte[] pswxSalt)
        {
            using (var hmac=new HMACSHA512(pswxSalt))
            {
                var computedHash =hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(pswx));
                return computedHash.SequenceEqual(pswxHash);
            }

        }


        //Update the user profile
        [HttpPut]
        public async Task<ActionResult<List<UsersDto>>> UpdateUser(UsersDto request)
        {
            var dbuser = await _context.Users.FindAsync(request.Id);
            if (dbuser == null)
                return BadRequest("User not found");

            dbuser.FirstName = request.FirstName;
            dbuser.LastName = request.LastName;
            dbuser.Email = request.Email;
            
            await _context.SaveChangesAsync();

            return Ok(await _context.Users.ToListAsync());
        }

        //Delete the specified user
        [HttpDelete("{id}")]
        public async Task<ActionResult<List<UsersDto>>> DeleteUser(int id)
        {
            var dbuser = await _context.Users.FindAsync(id);
            if (dbuser == null)
                return BadRequest("User not found");

            _context.Users.Remove(dbuser);
            await _context.SaveChangesAsync();

            return Ok(await _context.Users.ToListAsync());
        }



    }
}
