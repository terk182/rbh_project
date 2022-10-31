using BL;
using DataModel;
using TGBookingAPI.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace TGBookingAPI.Controllers
{
    public class FlightSearchController : ApiController
    {
        private readonly IFlightSearchServices _flightSearchServices;
        private readonly INamingServices _namingServices;
        private readonly IAPIConfigServices _apiConfigServices;
        private string officeID = ConfigurationManager.AppSettings["OfficeID"];
        public FlightSearchController(IFlightSearchServices flightSearchServices, INamingServices namingServices, IAPIConfigServices apiConfigServices)
        {
            this._flightSearchServices = flightSearchServices;
            this._namingServices = namingServices;
            this._apiConfigServices = apiConfigServices;
        }


        public HttpResponseMessage Post(SearchModel model)
        {
            var statusCode = System.Net.HttpStatusCode.OK;
            BL.Entities.RobinhoodFlight.FlightSearchResult result = new BL.Entities.RobinhoodFlight.FlightSearchResult();
            var header = Request.Headers;
            string webmode = ConfigurationManager.AppSettings["WEBMODE"];
            bool isLogin = HeaderAuth.Authorize(header, webmode, _apiConfigServices);

            string json = "";
            if (!isLogin)
            {
                json = "{\"isError\": true, \"errorCode\":\"1001\", \"errorMessage\": \"Invalid Account\"}";
                statusCode = HttpStatusCode.Unauthorized;
            }
            else
            {
                BL.Entities.MasterPricer.Request request = model.GetMPSearchRequest();
                request.bookingOID = officeID;
                result = this._flightSearchServices.Search(request);

                if (result == null)
                {
                    result = new BL.Entities.RobinhoodFlight.FlightSearchResult();
                    statusCode = HttpStatusCode.BadRequest;
                    json = "{\"isError\": true, \"type\":\"A\", \"errorCode\":\"2001\", \"errorMessage\": \"Flight not found\"}";
                }
                else if (!String.IsNullOrEmpty(result.errorMessage))
                {
                    result.isError = true;
                    statusCode = HttpStatusCode.InternalServerError;
                    json = "{\"isError\": true, \"type\":\"A\", \"errorCode\":\"2002\", \"errorMessage\": \"" + result.errorMessage + "\"}";
                }
                else
                {
                    json = result.toJson();
                }
            }

            HttpContext.Current.Response.Cache.VaryByHeaders["accept-encoding"] = true;

            var TheHTTPResponse = new HttpResponseMessage(statusCode);
            TheHTTPResponse.Content = new StringContent(json , Encoding.UTF8, "application/json");

            return TheHTTPResponse;
        }


        public class EncodingDelegateHandler : DelegatingHandler
        {
            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                return base.SendAsync(request, cancellationToken).ContinueWith<HttpResponseMessage>((responseToCompleteTask) =>
                {
                    HttpResponseMessage response = responseToCompleteTask.Result;

                    if (response.RequestMessage.Headers.AcceptEncoding != null &&
                        response.RequestMessage.Headers.AcceptEncoding.Count > 0)
                    {
                        string encodingType = response.RequestMessage.Headers.AcceptEncoding.First().Value;

                        response.Content = new CompressedContent(response.Content, encodingType);
                    }

                    return response;
                },
                TaskContinuationOptions.OnlyOnRanToCompletion);
            }
        }

        public class CompressedContent : HttpContent
        {
            private HttpContent originalContent;
            private string encodingType;

            public CompressedContent(HttpContent content, string encodingType)
            {
                if (content == null)
                {
                    throw new ArgumentNullException("content");
                }

                if (encodingType == null)
                {
                    throw new ArgumentNullException("encodingType");
                }

                originalContent = content;
                this.encodingType = encodingType.ToLowerInvariant();

                if (this.encodingType != "gzip" && this.encodingType != "deflate")
                {
                    throw new InvalidOperationException(string.Format("Encoding '{0}' is not supported. Only supports gzip or deflate encoding.", this.encodingType));
                }

                // copy the headers from the original content
                foreach (KeyValuePair<string, IEnumerable<string>> header in originalContent.Headers)
                {
                    this.Headers.TryAddWithoutValidation(header.Key, header.Value);
                }

                this.Headers.ContentEncoding.Add(encodingType);
            }

            protected override bool TryComputeLength(out long length)
            {
                length = -1;

                return false;
            }

            protected override Task SerializeToStreamAsync(Stream stream, TransportContext context)
            {
                Stream compressedStream = null;

                if (encodingType == "gzip")
                {
                    compressedStream = new GZipStream(stream, CompressionMode.Compress, leaveOpen: true);
                }
                else if (encodingType == "deflate")
                {
                    compressedStream = new DeflateStream(stream, CompressionMode.Compress, leaveOpen: true);
                }

                return originalContent.CopyToAsync(compressedStream).ContinueWith(tsk =>
                {
                    if (compressedStream != null)
                    {
                        compressedStream.Dispose();
                    }
                });
            }
        }


    }
}
