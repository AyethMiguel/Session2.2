using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace MM_API_Homework2_2
{
    [TestClass]
    public class PetPost
    {
        private static RestClient restClient;

        private static readonly string BaseURL = "https://petstore.swagger.io/v2/";

        private static readonly string PetEndpoint = "pet";

        private static string GetURL(string endpoint) => $"{BaseURL}{endpoint}";

        private static Uri GetURI(string endpoint) => new Uri(GetURL(endpoint));

        private readonly List<PetInfo> cleanUpList = new List<PetInfo>();

        [TestInitialize]
        public async Task TestInitialize()
        {
            restClient = new RestClient();
        }

        [TestCleanup]
        public async Task TestCleanup()
        {
            foreach (var data in cleanUpList)
            {
                var restRequest = new RestRequest(GetURI($"{PetEndpoint}/{data.Id}"));
                var restResponse = await restClient.DeleteAsync(restRequest);
            }
        }

        [TestMethod]
        public async Task PostMethod()
        {
            #region CreateUser
            //Create User
            var newPet = new PetInfo()
            {
                Category = new Category()
                {
                    Id = 1,
                    Name = "string" 
                },
                Name = "Angelicat",
                PhotoUrls = new string[]
                {
                    "string"
                },
                Tags = new Category[]
                {
                    new Category()
                    {
                        Id = 1,
                        Name = "string"
                    }
                },
                Status = "available"
            };

            // Send Post Request
            var postRestRequest = new RestRequest(GetURI(PetEndpoint)).AddJsonBody(newPet);
            var postRestResponse = await restClient.ExecutePostAsync<PetInfo>(postRestRequest);

            //Verify POST request status code
            Assert.AreEqual(HttpStatusCode.OK, postRestResponse.StatusCode, "Status code is not equal to 200");
            #endregion
         
            #region GetPet
            var restRequest = new RestRequest(GetURI($"{PetEndpoint}/{postRestResponse.Data.Id}"), Method.Get);
            var restResponse = await restClient.ExecuteAsync<PetInfo>(restRequest);
            #endregion

            #region Assertions
            Assert.AreEqual(HttpStatusCode.OK, restResponse.StatusCode, "Status code is not equal to 200");
            Assert.AreEqual(newPet.Name, restResponse.Data.Name, "Pet name does not match.");
            Assert.AreEqual(newPet.Category.Id, restResponse.Data.Category.Id, "Category Id does not match.");
            Assert.AreEqual(newPet.Category.Name, restResponse.Data.Category.Name, "Category Name does not match.");
            Assert.AreEqual(newPet.PhotoUrls[0], restResponse.Data.PhotoUrls[0], "Photo URL does not match.");
            Assert.AreEqual(newPet.Tags[0].Id, restResponse.Data.Tags[0].Id, "Tags Id does not match.");
            Assert.AreEqual(newPet.Tags[0].Name, restResponse.Data.Tags[0].Name, "Tags Name does not match.");
            Assert.AreEqual(newPet.Status, restResponse.Data.Status, "Status does not match.");

            #endregion

            #region CleanUp
            cleanUpList.Add(newPet);
            #endregion
        }
    }
}