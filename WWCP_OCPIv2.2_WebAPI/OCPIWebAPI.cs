﻿/*
 * Copyright (c) 2015-2020 GraphDefined GmbH
 * This file is part of WWCP OCPI <https://github.com/OpenChargingCloud/WWCP_OCPI>
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

#region Usings

using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.DNS;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using org.GraphDefined.WWCP;

using social.OpenData.UsersAPI;

using cloud.charging.open.API;
using cloud.charging.open.protocols.OCPIv2_2.HTTP;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_2.WebAPI
{

    /// <summary>
    /// OCPI WebAPI extention methods.
    /// </summary>
    public static class ExtentionMethods
    {

        #region ParseRoamingNetwork(this HTTPRequest, HTTPServer, out RoamingNetwork, out HTTPResponse)

        /// <summary>
        /// Parse the given HTTP request and return the roaming network
        /// for the given HTTP hostname and HTTP query parameter
        /// or an HTTP error response.
        /// </summary>
        /// <param name="HTTPRequest">A HTTP request.</param>
        /// <param name="HTTPServer">A HTTP server.</param>
        /// <param name="RoamingNetwork">The roaming network.</param>
        /// <param name="HTTPResponse">A HTTP error response.</param>
        /// <returns>True, when roaming network was found; false else.</returns>
        public static Boolean ParseRoamingNetwork(this HTTPRequest                             HTTPRequest,
                                                  HTTPServer<RoamingNetworks, RoamingNetwork>  HTTPServer,
                                                  out RoamingNetwork                           RoamingNetwork,
                                                  out HTTPResponse                             HTTPResponse)
        {

            if (HTTPServer == null)
                Console.WriteLine("HTTPServer == null!");

            #region Initial checks

            if (HTTPRequest == null)
                throw new ArgumentNullException("HTTPRequest",  "The given HTTP request must not be null!");

            if (HTTPServer == null)
                throw new ArgumentNullException("HTTPServer",   "The given HTTP server must not be null!");

            #endregion

            RoamingNetwork_Id RoamingNetworkId;
                              RoamingNetwork    = null;
                              HTTPResponse      = null;

            if (HTTPRequest.ParsedURLParameters.Length < 1)
            {

                HTTPResponse = new HTTPResponse.Builder(HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = HTTPServer.DefaultServerName,
                    Date            = DateTime.UtcNow,
                };

                return false;

            }

            if (!RoamingNetwork_Id.TryParse(HTTPRequest.ParsedURLParameters[0], out RoamingNetworkId))
            {

                HTTPResponse = new HTTPResponse.Builder(HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = HTTPServer.DefaultServerName,
                    Date            = DateTime.UtcNow,
                    ContentType     = HTTPContentType.JSON_UTF8,
                    Content         = @"{ ""description"": ""Invalid RoamingNetworkId!"" }".ToUTF8Bytes()
                };

                return false;

            }

            RoamingNetwork  = HTTPServer.
                                  GetAllTenants(HTTPRequest.Host).
                                  FirstOrDefault(roamingnetwork => roamingnetwork.Id == RoamingNetworkId);

            if (RoamingNetwork == null) {

                HTTPResponse = new HTTPResponse.Builder(HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.NotFound,
                    Server          = HTTPServer.DefaultServerName,
                    Date            = DateTime.UtcNow,
                    ContentType     = HTTPContentType.JSON_UTF8,
                    Content         = @"{ ""description"": ""Unknown RoamingNetworkId!"" }".ToUTF8Bytes()
                };

                return false;

            }

            return true;

        }

        #endregion

    }


    /// <summary>
    /// A HTTP API providing advanced OCPI data structures.
    /// </summary>
    public class OCPIWebAPI : HTTPAPI
    {

        #region Data

        /// <summary>
        /// The default HTTP URI prefix.
        /// </summary>
        public static readonly HTTPPath                             DefaultURLPathPrefix        = HTTPPath.Parse("webapi");

        /// <summary>
        /// The default HTTP realm, if HTTP Basic Authentication is used.
        /// </summary>
        public const String DefaultHTTPRealm = "Open Charging Cloud OCPIPlus WebAPI";

        /// <summary>
        /// The HTTP root for embedded ressources.
        /// </summary>
        public const              String                            HTTPRoot                    = "cloud.charging.open.protocols.OCPIv2_2.WebAPI.HTTPRoot.";


        //ToDo: http://www.iana.org/form/media-types

        /// <summary>
        /// The HTTP content type for serving OCPI+ XML data.
        /// </summary>
        public static readonly HTTPContentType                      OCPIPlusJSONContentType     = new HTTPContentType("application", "vnd.OCPIPlus+json", "utf-8", null, null);

        /// <summary>
        /// The HTTP content type for serving OCPI+ HTML data.
        /// </summary>
        public static readonly HTTPContentType                      OCPIPlusHTMLContentType     = new HTTPContentType("application", "vnd.OCPIPlus+html", "utf-8", null, null);


        public static readonly HTTPEventSource_Id                   DebugLogId                  = HTTPEventSource_Id.Parse("OCPIDebugLog");

        #endregion

        #region Properties

        /// <summary>
        /// The HTTP URI prefix.
        /// </summary>
        public HTTPPath?                                    URLPathPrefix1      { get; }

        /// <summary>
        /// The HTTP realm, if HTTP Basic Authentication is used.
        /// </summary>
        public String                                       HTTPRealm           { get; }

        /// <summary>
        /// An enumeration of logins for an optional HTTP Basic Authentication.
        /// </summary>
        public IEnumerable<KeyValuePair<String, String>>    HTTPLogins          { get; }


        /// <summary>
        /// Send debug information via HTTP Server Sent Events.
        /// </summary>
        public HTTPEventSource<JObject>                     DebugLog            { get; }


        /// <summary>
        /// The DNS client to use.
        /// </summary>
        public DNSClient                                    DNSClient           { get; }


        public CommonAPI                                    CommonAPI          { get; set; }

        public CommonAPILogger                              CommonAPILogger    { get; set; }


        public CPOAPI                                       CPOAPI             { get; set; }

        public CPOAPILogger                                 CPOAPILogger       { get; set; }


        public EMSPAPI                                      EMSPAPI            { get; set; }

        public EMSPAPILogger                                EMSPAPILogger      { get; set; }


        public List<CPOClient>                              CPOClients         { get; }

        public List<EMSPClient>                             EMSPClients        { get; }

        #endregion

        #region Events

        #region Generic HTTP server logging

        /// <summary>
        /// An event called whenever a HTTP request came in.
        /// </summary>
        public HTTPRequestLogEvent   RequestLog    = new HTTPRequestLogEvent();

        /// <summary>
        /// An event called whenever a HTTP request could successfully be processed.
        /// </summary>
        public HTTPResponseLogEvent  ResponseLog   = new HTTPResponseLogEvent();

        /// <summary>
        /// An event called whenever a HTTP request resulted in an error.
        /// </summary>
        public HTTPErrorLogEvent     ErrorLog      = new HTTPErrorLogEvent();

        #endregion

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Attach the OCPI+ WebAPI to the given HTTP server.
        /// </summary>
        /// <param name="HTTPServer">A HTTP server.</param>
        /// <param name="URLPathPrefix">An optional prefix for the HTTP URIs.</param>
        /// <param name="HTTPRealm">The HTTP realm, if HTTP Basic Authentication is used.</param>
        /// <param name="HTTPLogins">An enumeration of logins for an optional HTTP Basic Authentication.</param>
        public OCPIWebAPI(HTTPServer                                 HTTPServer,
                          HTTPPath?                                  URLPathPrefix1   = null,
                          HTTPPath?                                  URLPathPrefix    = null,
                          String                                     HTTPRealm        = DefaultHTTPRealm,
                          IEnumerable<KeyValuePair<String, String>>  HTTPLogins       = null,
                          String                                     HTMLTemplate     = null)

            : base(HTTPServer,
                   null,
                   "",
                   URLPathPrefix ?? DefaultURLPathPrefix)

        {

            this.URLPathPrefix1      = URLPathPrefix1;
            this.HTTPRealm           = HTTPRealm.IsNotNullOrEmpty() ? HTTPRealm : DefaultHTTPRealm;
            this.HTTPLogins          = HTTPLogins    ?? new KeyValuePair<String, String>[0];
            this.DNSClient           = HTTPServer.DNSClient;

            this.CPOClients          = new List<CPOClient>();
            this.EMSPClients         = new List<EMSPClient>();

            // Link HTTP events...
            HTTPServer.RequestLog   += (HTTPProcessor, ServerTimestamp, Request)                                 => RequestLog. WhenAll(HTTPProcessor, ServerTimestamp, Request);
            HTTPServer.ResponseLog  += (HTTPProcessor, ServerTimestamp, Request, Response)                       => ResponseLog.WhenAll(HTTPProcessor, ServerTimestamp, Request, Response);
            HTTPServer.ErrorLog     += (HTTPProcessor, ServerTimestamp, Request, Response, Error, LastException) => ErrorLog.   WhenAll(HTTPProcessor, ServerTimestamp, Request, Response, Error, LastException);

            var LogfilePrefix        = "HTTPSSEs" + Path.DirectorySeparatorChar;

            //this.DebugLog            = HTTPServer.AddJSONEventSource(EventIdentification:      DebugLogId,
            //                                                         URLTemplate:              this.URLPathPrefix + "/DebugLog",
            //                                                         MaxNumberOfCachedEvents:  10000,
            //                                                         RetryIntervall:           TimeSpan.FromSeconds(5),
            //                                                         EnableLogging:            true,
            //                                                         LogfilePrefix:            LogfilePrefix);

            RegisterURITemplates();

            this.HTMLTemplate = HTMLTemplate ?? GetResourceString(typeof(OCPIWebAPI).Assembly, HTTPRoot + "template.html");

        }

        #endregion


        #region (private) MixWithHTMLTemplate    (ResourceName)

        protected String MixWithHTMLTemplate(String ResourceName)

            => MixWithHTMLTemplate(ResourceName,
                                   new Tuple<String, System.Reflection.Assembly>(OCPIWebAPI.          HTTPRoot, typeof(OCPIWebAPI).          Assembly),
                                   new Tuple<String, System.Reflection.Assembly>(OpenChargingCloudAPI.HTTPRoot, typeof(OpenChargingCloudAPI).Assembly),
                                   new Tuple<String, System.Reflection.Assembly>(UsersAPI.            HTTPRoot, typeof(UsersAPI).            Assembly));

        #endregion




        #region (private) RegisterURITemplates()

        private void RegisterURITemplates()
        {

            #region / (HTTPRoot)

            HTTPServer.RegisterResourcesFolder(HTTPHostname.Any,
                                               URLPathPrefix,
                                               "cloud.charging.open.protocols.OCPIv2_2.WebAPI.HTTPRoot",
                                               DefaultFilename: "index.html");

            if (URLPathPrefix1.HasValue)
                HTTPServer.AddMethodCallback(HTTPHostname.Any,
                                             HTTPMethod.GET,
                                             URLPathPrefix1.Value,
                                             HTTPContentType.HTML_UTF8,
                                             HTTPDelegate: Request => {

                                                 return Task.FromResult(
                                                     new HTTPResponse.Builder(Request) {
                                                         HTTPStatusCode             = HTTPStatusCode.OK,
                                                         //Server                     = DefaultHTTPServerName,
                                                         Date                       = DateTime.UtcNow,
                                                         AccessControlAllowOrigin   = "*",
                                                         AccessControlAllowMethods  = "OPTIONS, GET",
                                                         AccessControlAllowHeaders  = "Authorization",
                                                         ContentType                = HTTPContentType.HTML_UTF8,
                                                         Content                    = ("<html><body>" +
                                                                                          "This is an Open Charge Point Interface HTTP service!<br /><br />" +
                                                                                          "<ul>" +
                                                                                              "<li><a href=\"versions\">Versions</a></li>" +
                                                                                              "<li><a href=\"" + URLPathPrefix.ToString() + "/remoteParties\">Remote Parties</a></li>" +
                                                                                              "<li><a href=\"" + URLPathPrefix.ToString() + "/clients\">Clients</a></li>" +
                                                                                              "<li><a href=\"" + URLPathPrefix.ToString() + "/cpoclients\">CPO Clients</a></li>" +
                                                                                              "<li><a href=\"" + URLPathPrefix.ToString() + "/emspclients\">EMSP Clients</a></li>" +
                                                                                       "</ul><body></html>").ToUTF8Bytes(),
                                                         Connection                 = "close"
                                                     }.AsImmutable);

                                             });

            if (URLPathPrefix1.HasValue)
                HTTPServer.AddMethodCallback(HTTPHostname.Any,
                                             HTTPMethod.GET,
                                             URLPathPrefix1.Value + "versions",
                                             HTTPContentType.HTML_UTF8,
                                             HTTPDelegate: Request => {

                                                 return Task.FromResult(
                                                     new HTTPResponse.Builder(Request) {
                                                         HTTPStatusCode             = HTTPStatusCode.OK,
                                                         //Server                     = DefaultHTTPServerName,
                                                         Date                       = DateTime.UtcNow,
                                                         AccessControlAllowOrigin   = "*",
                                                         AccessControlAllowMethods  = "OPTIONS, GET",
                                                         AccessControlAllowHeaders  = "Authorization",
                                                         ContentType                = HTTPContentType.HTML_UTF8,
                                                         Content                    = MixWithHTMLTemplate("versions.versions.shtml").ToUTF8Bytes(),
                                                         Connection                 = "close",
                                                         Vary                       = "Accept"
                                                     }.AsImmutable);

                                             });


            if (URLPathPrefix1.HasValue)
                HTTPServer.AddMethodCallback(HTTPHostname.Any,
                                             HTTPMethod.GET,
                                             URLPathPrefix1.Value + "versions/{id}",
                                             HTTPContentType.HTML_UTF8,
                                             HTTPDelegate: Request => {

                                                 return Task.FromResult(
                                                     new HTTPResponse.Builder(Request) {
                                                         HTTPStatusCode             = HTTPStatusCode.OK,
                                                         //Server                     = DefaultHTTPServerName,
                                                         Date                       = DateTime.UtcNow,
                                                         AccessControlAllowOrigin   = "*",
                                                         AccessControlAllowMethods  = "OPTIONS, GET",
                                                         AccessControlAllowHeaders  = "Authorization",
                                                         ContentType                = HTTPContentType.HTML_UTF8,
                                                         Content                    = MixWithHTMLTemplate("versions.versionDetails.shtml").ToUTF8Bytes(),
                                                         Connection                 = "close",
                                                         Vary                       = "Accept"
                                                     }.AsImmutable);

                                             });


            #endregion


            #region GET      ~/remoteParties

            HTTPServer.AddMethodCallback(HTTPHostname.Any,
                                         HTTPMethod.GET,
                                         URLPathPrefix + "remoteParties",
                                         HTTPContentType.JSON_UTF8,
                                         HTTPDelegate: Request => {


                                             return Task.FromResult(
                                                 new HTTPResponse.Builder(Request) {
                                                     HTTPStatusCode             = HTTPStatusCode.OK,
                                                     ContentType                = HTTPContentType.JSON_UTF8,
                                                     Content                    = new JArray(CommonAPI.RemoteParties.Select(party => party.ToJSON())).ToUTF8Bytes(),
                                                     AccessControlAllowMethods  = "OPTIONS, GET",
                                                     AccessControlAllowHeaders  = "Authorization"
                                                     //LastModified               = Location.LastUpdated.ToIso8601(),
                                                     //ETag                       = Location.SHA256Hash
                                                 }.AsImmutable);

                                         });

            #endregion

            #region GET      ~/clients

            HTTPServer.AddMethodCallback(HTTPHostname.Any,
                                         HTTPMethod.GET,
                                         URLPathPrefix + "clients",
                                         HTTPContentType.JSON_UTF8,
                                         HTTPDelegate: Request => {

                                             var clients = new List<CommonClient>();
                                             clients.AddRange(CPOClients);
                                             clients.AddRange(EMSPClients);

                                             return Task.FromResult(
                                                 new HTTPResponse.Builder(Request) {
                                                     HTTPStatusCode             = HTTPStatusCode.OK,
                                                     ContentType                = HTTPContentType.JSON_UTF8,
                                                     Content                    = new JArray(clients.OrderBy(client => client.Description).Select(client => client.ToJSON())).ToUTF8Bytes(),
                                                     AccessControlAllowMethods  = "OPTIONS, GET",
                                                     AccessControlAllowHeaders  = "Authorization"
                                                     //LastModified               = Location.LastUpdated.ToIso8601(),
                                                     //ETag                       = Location.SHA256Hash
                                                 }.AsImmutable);

                                         });

            #endregion

            #region GET      ~/cpoclients

            HTTPServer.AddMethodCallback(HTTPHostname.Any,
                                         HTTPMethod.GET,
                                         URLPathPrefix + "cpoclients",
                                         HTTPContentType.JSON_UTF8,
                                         HTTPDelegate: Request => {


                                             return Task.FromResult(
                                                 new HTTPResponse.Builder(Request)
                                                 {
                                                     HTTPStatusCode = HTTPStatusCode.OK,
                                                     ContentType = HTTPContentType.JSON_UTF8,
                                                     Content = new JArray(CPOClients.OrderBy(client => client.Description).Select(client => client.ToJSON())).ToUTF8Bytes(),
                                                     AccessControlAllowMethods = "OPTIONS, GET",
                                                     AccessControlAllowHeaders = "Authorization"
                                                     //LastModified               = Location.LastUpdated.ToIso8601(),
                                                     //ETag                       = Location.SHA256Hash
                                                 }.AsImmutable);

                                         });

            #endregion

            #region GET      ~/emspclients

            HTTPServer.AddMethodCallback(HTTPHostname.Any,
                                         HTTPMethod.GET,
                                         URLPathPrefix + "emspclients",
                                         HTTPContentType.JSON_UTF8,
                                         HTTPDelegate: Request => {


                                             return Task.FromResult(
                                                 new HTTPResponse.Builder(Request) {
                                                     HTTPStatusCode             = HTTPStatusCode.OK,
                                                     ContentType                = HTTPContentType.JSON_UTF8,
                                                     Content                    = new JArray(EMSPClients.OrderBy(client => client.Description).Select(client => client.ToJSON())).ToUTF8Bytes(),
                                                     AccessControlAllowMethods  = "OPTIONS, GET",
                                                     AccessControlAllowHeaders  = "Authorization"
                                                     //LastModified               = Location.LastUpdated.ToIso8601(),
                                                     //ETag                       = Location.SHA256Hash
                                                 }.AsImmutable);

                                         });

            #endregion


        }

        #endregion




        #region GetEMSPClient(CountryCode, PartyId, Role = Roles.CPO)

        public EMSPClient GetEMSPClient(CountryCode  CountryCode,
                                        Party_Id     PartyId,
                                        Roles        Role = Roles.CPO)
        {

            var _party = CommonAPI.RemoteParties.FirstOrDefault(party => party.CountryCode == CountryCode &&
                                                                   party.PartyId     == PartyId &&
                                                                   party.Role        == Role);

            if (_party?.RemoteAccessInfos?.Any() == true)
                return EMSPClients.AddAndReturnElement(
                    new EMSPClient(_party.RemoteAccessInfos.First().AccessToken,
                                   _party.RemoteAccessInfos.First().VersionsURL,
                                   CommonAPI,
                                   RemoteCertificateValidator: (sender, certificate, chain, sslPolicyErrors) => true));

            return null;

        }

        #endregion

        #region GetCPOClient (CountryCode, PartyId, Role = Roles.EMSP)

        public CPOClient GetCPOClient(CountryCode  CountryCode,
                                      Party_Id     PartyId,
                                      Roles        Role = Roles.EMSP)
        {

            var _party = CommonAPI.RemoteParties.FirstOrDefault(party => party.CountryCode == CountryCode &&
                                                                   party.PartyId     == PartyId &&
                                                                   party.Role        == Role);

            if (_party?.RemoteAccessInfos?.Any() == true)
                return CPOClients.AddAndReturnElement(
                    new CPOClient(_party.RemoteAccessInfos.First().AccessToken,
                                  _party.RemoteAccessInfos.First().VersionsURL,
                                  CommonAPI,
                                  RemoteCertificateValidator: (sender, certificate, chain, sslPolicyErrors) => true));

            return null;

        }

        #endregion



        //public void Add(WWCPCPOAdapter CPOAdapter)
        //{

        //    _CPOAdapters.Add(CPOAdapter);

        //}


        //public void Add(WWCPEMPAdapter EMPAdapter)
        //{

        //    _EMPAdapters.Add(EMPAdapter);

        //}

    }

}
