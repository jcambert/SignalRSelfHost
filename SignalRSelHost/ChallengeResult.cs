
using Microsoft.AspNet.SignalR.Owin;
using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace SignalRSelHost
{
    internal class ChallengeResult
    {
        private readonly string provider_;
        private readonly ServerRequest request_;

        public ChallengeResult(string provider, ServerRequest request)
        {
            this.provider_ = provider;
            this.request_ = request;
        }

        public  Task<HttpResponseMessage> ExecuteAsync()
        {
            OwinContext ctx = new OwinContext();

            ctx.Authentication.Challenge(provider_);
            var user=ctx.Authentication.User;


         
            /*request_.GetOwinContext().Authentication.Challenge(provider_);

            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
            response.RequestMessage = request_;
            return response;*/

            return null;
        }
    }
}
