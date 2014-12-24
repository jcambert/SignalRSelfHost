using AspNet.Identity.MongoDB;
using Microsoft.AspNet.Identity;
using MongoDB.Driver;
using MongoRepository;
using SignalRSelHost.Entities;
using SignalRSelHost.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalRSelHost
{
    public class AuthRepository : IDisposable
    {
        // MongoRepository<Client> 

        private UserManager<IdentityUser> userManager_;
        private readonly IdentityContext context_;
        private readonly MongoRepository<string, IdentityUser> users_;
        private readonly MongoRepository<string,Client> clients_;
        private readonly MongoRepository<string, RefreshToken> tokens_;
        public AuthRepository()
        {
            users_ = Repositories.GetCollection< IdentityUser>("Users");
            context_ = new IdentityContext(users_.Collection);
            userManager_ = new UserManager<IdentityUser>(new UserStore<IdentityUser>(context_));

            clients_ = Repositories.GetCollection<Client>("Clients");
            tokens_ = Repositories.GetCollection<RefreshToken>("RefreshToken");
        }

        public async Task<IdentityResult> RegisterUser(UserModel userModel)
        {
            IdentityUser user = new IdentityUser
            {
                UserName = userModel.UserName
            };

            var result = await userManager_.CreateAsync(user, userModel.Password);

            return result;
        }

        public async Task<IdentityUser> FindUser(string userName, string password)
        {
            IdentityUser user = await userManager_.FindAsync(userName, password);
           
            return user;
        }

        public Client FindClient(string clientId)
        {
            var client = clients_.GetById(clientId);// context_.Clients.Find(clientId);

            return client;
        }

        public async Task<bool> AddRefreshToken(RefreshToken token)
        {

            var existingToken =tokens_.AsQueryable().Where(r => r.Subject == token.Subject && r.ClientId == token.ClientId).SingleOrDefault();

            if (existingToken != null)
            {
                var result = await RemoveRefreshToken(existingToken);
            }

            token.AddDocument();

            return true;
           // _ctx.RefreshTokens.Add(token);

            //return await _ctx.SaveChangesAsync() > 0;
        }

        public async Task<bool> RemoveRefreshToken(string refreshTokenId)
        {
            var refreshToken = tokens_.GetById(refreshTokenId); //await _ctx.RefreshTokens.FindAsync(refreshTokenId);

            if (refreshToken != null)
            {
                tokens_.Delete(refreshToken);
                return true;
                //_ctx.RefreshTokens.Remove(refreshToken);
                //return await _ctx.SaveChangesAsync() > 0;
            }

            return false;
        }

        public async Task<bool> RemoveRefreshToken(RefreshToken refreshToken)
        {
            tokens_.Delete(refreshToken);
            //_ctx.RefreshTokens.Remove(refreshToken);
            //return await _ctx.SaveChangesAsync() > 0;
            return true;
        }

        public async Task<RefreshToken> FindRefreshToken(string refreshTokenId)
        {
            var refreshToken = tokens_.GetById(refreshTokenId);
           // var refreshToken = await _ctx.RefreshTokens.FindAsync(refreshTokenId);

            return refreshToken;
        }

        public List<RefreshToken> GetAllRefreshTokens()
        {
            return tokens_.AsQueryable().ToList();
        }

        public async Task<IdentityUser> FindAsync(UserLoginInfo loginInfo)
        {
            IdentityUser user = await userManager_.FindAsync(loginInfo);

            return user;
        }

        public async Task<IdentityResult> CreateAsync(IdentityUser user)
        {
            var result = await userManager_.CreateAsync(user);

            return result;
        }

        public async Task<IdentityResult> AddLoginAsync(string userId, UserLoginInfo login)
        {
            var result = await userManager_.AddLoginAsync(userId, login);

            return result;
        }

        public void Dispose()
        {
           // context_.Dispose();
            userManager_.Dispose();

        }
    }
}
