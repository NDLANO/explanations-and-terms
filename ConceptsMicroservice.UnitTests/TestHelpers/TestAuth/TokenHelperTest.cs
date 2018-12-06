using System.Collections.Generic;
using System.Threading.Tasks;
using Auth0.AuthenticationApi;
using Auth0.AuthenticationApi.Models;
using ConceptsMicroservice.Utilities.Auth;
using FakeItEasy;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace ConceptsMicroservice.UnitTests.TestHelpers.TestAuth
{
    public class TokenHelperTest
    {
        Microsoft.AspNetCore.Http.HttpContext _context;
        private readonly IConfiguration _iconfig;
        private readonly string _token = "fdsdgatretet.ttwttwtwtwrt.khjkhjkhjkkhk";
        private AuthenticationApiClient _authApiClient;
        private readonly string allowedUserEmail = "somebody@somedomain";
        private readonly ITokenHelper TokenHelper;

        public TokenHelperTest()
        {
            //_iconfig = A.Fake<IConfiguration>();
            //TokenHelper = A.Fake<ITokenHelper>(iconfig); //x => x.WithArgumentsForConstructor((new object[] {_config})));
        }
        //public async Task<string> ReturnClaimEmail(HttpContext context)
        [Fact]
        public async void  ReturnClaimEmail_Succeed()
        {
           // string auth0Domain = "ndla.eu.auth0.com";
           // //_context = new HttpContext();
           // _context = A.Fake<HttpContext>();

           // //var result = await _context.GetTokenAsync("access_token");


           //// A.CallTo( () =>   _context.GetTokenAsync("access_token")).Returns(_token);
           // AuthenticationApiClient fakeauthApiClient = A.Fake<AuthenticationApiClient>(x => x.WithArgumentsForConstructor((new object[] { auth0Domain })));
           //    // new AuthenticationApiClient(auth0Domain);


           // //A.CallTo(() => _authApiClient).Returns(new AuthenticationApiClient(auth0Domain));
           // var fakeUserInfo = A.Fake<Auth0.AuthenticationApi.Models.UserInfo>();   // new Auth0.AuthenticationApi.Models.UserInfo();
           // fakeUserInfo.Email = allowedUserEmail;
           // //A.CallTo(() => fakeauthApiClient.GetUserInfoAsync(_token)).Returns(fakeUserInfo);
           // //fakeUserInfo = await fakeauthApiClient.GetUserInfoAsync(_token);


           // Assert.Equal(allowedUserEmail, fakeUserInfo.Email);

        }
        [Fact]
        public void ReturnClaimEmail_Failed()
        {
            //string auth0Domain = "ndla.eu.auth0.com";
            //_context = A.Fake<HttpContext>();
            //AuthenticationApiClient fakeauthApiClient = A.Fake<AuthenticationApiClient>(x => x.WithArgumentsForConstructor((new object[] { auth0Domain })));
            //var fakeUserInfo = A.Fake<Auth0.AuthenticationApi.Models.UserInfo>();   // new Auth0.AuthenticationApi.Models.UserInfo();
            //fakeUserInfo.Email = "someunknownemail@somedoamin";
            ////A.CallTo(() => fakeauthApiClient.GetUserInfoAsync(_token)).Returns(fakeUserInfo);
            ////fakeUserInfo = await fakeauthApiClient.GetUserInfoAsync(_token);

            //TokenHelper.ReturnClaimEmail(_context);

            //Assert.NotEqual(allowedUserEmail, fakeUserInfo.Email);
        }
        [Fact]
        //public string ReturnScope(ClaimsPrincipal user)
        public void ReturnScope_Succeed()
        {

        }
        [Fact]
        //public string ReturnScope(ClaimsPrincipal user)
        public void ReturnScope_Failed()
        {

        }
        //public async Task<string> ReturnToken(HttpContext context)
        [Fact]
        public void ReturnToken_Succeed()
        {

        }

        public void ReturnToken_Failed()
        {

        }
    }
}
