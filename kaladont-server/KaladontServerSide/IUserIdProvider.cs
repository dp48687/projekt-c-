using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaladontServerSide
{
    public interface IUserIdProvider
    {
        /// <summary>
        /// Gets the connection identification
        /// </summary>
        /// <param name="request"></param>
        /// <returns>String that represents users id</returns>
        string GetUserId(IRequest request);
    }
}
