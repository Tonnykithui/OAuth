using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OAuth.Models.Response
{
    public class UserAuthorized
    {
        public string Token { get; set; }

        public bool IsSuccess { get; set; }

        public List<string> Error { get; set; }
    }
}
