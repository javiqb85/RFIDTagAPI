using System;
using Xunit;
using System.Collections.Generic;
using RFIDTagAPI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Primitives;
using Microsoft.Extensions.Logging.Abstractions;

namespace UnitTest
{
    public class UnitTest1
    {
        // Creation of new RFID tag should be tested with a new rfid tag not existing
        //[Fact]
        //public void TestRFIDTagCreation()
        //{
        //    var request = new DefaultHttpRequest(new DefaultHttpContext())
        //    {
        //        Path = "/RFID/rfid3",
        //        Method = "post"
        //    };

        //    var logger = NullLoggerFactory.Instance.CreateLogger("Null Logger");
        //    var response = Create.Run(request, "rfid3", logger);
        //    response.Wait();

        //    Assert.IsAssignableFrom<StatusCodeResult>(response.Result);

        //    // Check that the contents of the response are the expected contents
        //    var result = (StatusCodeResult)response.Result;
        //    var match = new StatusCodeResult(StatusCodes.Status201Created);
        //    Assert.Equal(match.StatusCode, result.StatusCode);
        //}

        [Fact]
        public void TestRFIDCreateExistingRFID()
        {
            string rfidtag = "rfid1";
            var request = new DefaultHttpRequest(new DefaultHttpContext())
            {
                Path = "/RFID/" + rfidtag,
                Method = "post"
            };

            var logger = NullLoggerFactory.Instance.CreateLogger("Null Logger");
            var response = Create.Run(request, rfidtag, logger);
            response.Wait();

            Assert.IsAssignableFrom<ObjectResult>(response.Result);

            var result = (ObjectResult)response.Result;
            var watchInfo = new ObjectResult(new { Result = $"RFID tag {rfidtag} has already been added!" });
            Assert.Equal(watchInfo.Value.ToString(), result.Value.ToString());
        }

        [Fact]
        public void TestAuthExisting()
        {
            string existingRFID = "rfid1";
            var request = new DefaultHttpRequest(new DefaultHttpContext())
            {
                Query = new QueryCollection(new Dictionary<string, StringValues>() { { "RFID", existingRFID } }),
                Method = "get"
            };
            var logger = NullLoggerFactory.Instance.CreateLogger("Null Logger");
            var response = Authentication.Run(request, logger);
            response.Wait();
            Assert.IsAssignableFrom<OkObjectResult>(response.Result);
            // Check that the contents of the response are the expected contents
            var result = (OkObjectResult)response.Result;
            string watchInfo = "true";
            Assert.Equal(watchInfo, result.Value.ToString());
        }
        [Fact]
        public void TestAuthNonExisting()
        {
            string NotexistingRFID = "rfid109090090";
            var request = new DefaultHttpRequest(new DefaultHttpContext())
            {
                Query = new QueryCollection(new Dictionary<string, StringValues>() { { "RFID", NotexistingRFID } }),
                Method = "get"
            };
            var logger = NullLoggerFactory.Instance.CreateLogger("Null Logger");
            var response = Authentication.Run(request, logger);
            response.Wait();
            Assert.IsAssignableFrom<OkObjectResult>(response.Result);
            // Check that the contents of the response are the expected contents
            var result = (OkObjectResult)response.Result;
            string watchInfo = "false";
            Assert.Equal(watchInfo, result.Value.ToString());
        }
    }
}
