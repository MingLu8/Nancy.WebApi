The purpose of the project is to enable attribute routing with Nancy framework and minimizing the code changes needed if one decides to use ASP.NET WebAPI later on.

Here is some sample usage:
 [RoutePrefix("api/responseMessage")]
    public class ResponseMessageModule : ApiController
    {
        [HttpGet(nameof(WithStatusCode))]
        public ResponseMessage<Address> WithStatusCode()
        {
            var result = new Address {City = "dummie city"};
            return new ResponseMessage<Address>(() => new JsonResponse(result, new JsonNetSerializer()).WithStatusCode(HttpStatusCode.NotModified));
        }
    }
    
    [RoutePrefix("api/userApi")]
    public class UserApi : ApiController
    {       
        [HttpGet("/")]
        public IEnumerable<User> GetAll()
        {
            return new List<User> { new User(), new User() };
        }

       
        [HttpGet("/{id}")]
        public User GetById(int id)
        {
            return new User();
        }

        [HttpGet("/name")]
        public IEnumerable<User> GetByName(Name name)
        {
            return new List<User> { new User(), new User(), new User() };
        }
      
        [HttpPost("/{id}/{firstname}/{lastname}")]
        public bool ChangeUser(int id, string firstname, string lastname)
        {
            return true;
        }
      }
