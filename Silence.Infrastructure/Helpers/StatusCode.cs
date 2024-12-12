using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Silence.Infrastructure.Helpers
{
    public enum StatusCode
    {
        FileAlreadyExists = 409,
        FileNotFound = 404,
        UserNotFound = 0,
        UserAlreadyExists = 409,
        OK = 200,
        InternalServerError = 500
    }
}
