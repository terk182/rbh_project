using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Transactions;
using BL.Entities.APILogin;
using DataModel;
using DataModel.UnitOfWork;

namespace BL
{
    public class APIConfigServices : IAPIConfigServices
    {
        private readonly UnitOfWork _unitOfWork;
        public APIConfigServices(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public void Delete(Guid id)
        {
            using (var scope = new TransactionScope())
            {
                _unitOfWork.APIUserRepository.Delete(id);
                _unitOfWork.Save();
                scope.Complete();
            }
        }

        public List<APIUser> GetAll()
        {
            return _unitOfWork.APIUserRepository.GetAll().ToList();
        }

        public APIUser GetById(Guid id)
        {
            return _unitOfWork.APIUserRepository.GetFirstOrDefault(x => x.APIUserOID == id);
        }

        public APIUser GetByToken(string token)
        {
            var apiToken = _unitOfWork.APITokenRepository.GetFirstOrDefault(x => x.Token == token);
            if (apiToken == null)
            {
                return null;
            }

            if (apiToken.Expires < DateTime.Now)
            {
                return null;
            }

            return this.GetById(apiToken.APIUserOID);
        }

        public Response Login(Request request, string webmode)
        {
            int mode = webmode == "PROD" ? 1 : 0;
            Response response = new Response();
            if (request.grantType.ToLower() != "password")
            {
                response.isError = true;
                response.errorCode = "1000";
                response.errorMessage = "wrong grant type";
                return response;
            }

            var user = _unitOfWork.APIUserRepository.GetFirstOrDefault(x => x.Username == request.username && x.Password == request.password && x.WebMode == mode);
            if (user == null)
            {
                response.isError = true;
                response.errorCode = "1001";
                response.errorMessage = "invalid account";
                return response;
            }

            string token = System.Web.Security.Membership.GeneratePassword(128, 0);
            Random random = new Random();
            token = Regex.Replace(token, @"[^a-zA-Z0-9]", m => random.Next(0, 9).ToString());
            APIToken apiToken = new APIToken();
            apiToken.APIUserOID = user.APIUserOID;
            apiToken.CreateDate = DateTime.Now;
            apiToken.Expires = DateTime.Now.AddHours(24);
            apiToken.Token = token;
            _unitOfWork.APITokenRepository.Insert(apiToken);
            _unitOfWork.Save();

            response.accessToken = token;
            response.expires = apiToken.Expires.GetValueOrDefault();
            response.tokenType = "Bearer";

            return response;
        }

        public void SaveOrUpdate(APIUser user)
        {
            using (var scope = new TransactionScope())
            {
                var check = _unitOfWork.APIUserRepository.GetFirstOrDefault(x => x.APIUserOID == user.APIUserOID);
                if (check == null)
                {
                    _unitOfWork.APIUserRepository.Insert(user);
                }
                else
                {
                    _unitOfWork.APIUserRepository.Update(user);
                }
                _unitOfWork.Save();
                scope.Complete();
            }
        }
    }
}
