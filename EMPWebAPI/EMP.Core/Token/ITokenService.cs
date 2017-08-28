using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMP.Core.Token.DTO;

namespace EMP.Core.Token
{
    public interface ITokenService
    {
        TokenDTO GenerateToken(Guid userId,string userip="");
        bool ValidateToken(string tokenId);
        bool Kill(string UserId);
        bool DeleteByUserId(Guid userId);
    }
}
