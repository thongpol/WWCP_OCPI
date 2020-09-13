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
using System.Reflection;

using Newtonsoft.Json.Linq;

using Org.BouncyCastle.Bcpg.OpenPgp;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Hermod.SMTP;
using org.GraphDefined.Vanaheimr.Hermod.DNS;
using org.GraphDefined.Vanaheimr.Hermod.Mail;
using org.GraphDefined.Vanaheimr.BouncyCastle;
using cloud.charging.open.protocols;
using org.GraphDefined.WWCP;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_2.HTTP
{

    /// <summary>
    /// The OCPI HTTP API for Charge Point Operators.
    /// </summary>
    public class CPOAPI : GenericAPI
    {

        #region Data

        private static readonly Random  _Random                = new Random();

        public  const           String  DefaultHTTPServerName  = "GraphDefined OCPI CPO HTTP API v0.1";
        public  static readonly IPPort  DefaultHTTPServerPort  = IPPort.Parse(8080);

        public  const           String  LogfileName            = "OICP_CPO_HTTPAPI.log";

        #endregion

        #region Constructor(s)

        #region CPOAPI(HTTPServerName, ...)

        /// <summary>
        /// Create an instance of the OCPI HTTP API for Charge Point Operators
        /// using a newly created HTTP server.
        /// </summary>
        public CPOAPI(RoamingNetwork    RoamingNetwork,
                      String            HTTPServerName    = DefaultHTTPServerName,
                      IPPort?           HTTPServerPort    = null,
                      HTTPPath?          URIPrefix         = null,

                      String            ServiceName       = DefaultHTTPServerName,
                      EMailAddress      APIEMailAddress   = null,
                      PgpSecretKeyRing  APISecretKeyRing  = null,
                      String            APIPassphrase     = null,
                      EMailAddressList  APIAdminEMail     = null,
                      SMTPClient        APISMTPClient     = null,

                      DNSClient         DNSClient         = null,
                      String            LogfileName       = DefaultLogfileName)

            : base(RoamingNetwork,
                   HTTPServerName,
                   HTTPServerPort ?? DefaultHTTPServerPort,
                   URIPrefix      ?? DefaultURIPrefix,
                   ResourceName => typeof(CPOAPI).Assembly.GetManifestResourceStream("cloud.charging.open.protocols.OCPIv2_2.HTTPAPI.CPOAPI.HTTPRoot." + ResourceName),

                   ServiceName,
                   APIEMailAddress,
                   null,//OpenPGP.ReadPublicKeyRing(typeof(CPOAPI).Assembly.GetManifestResourceStream("cloud.charging.open.protocols.OCPIv2_2.HTTPAPI.GenericAPI.HTTPRoot.robot@offenes-jena_pubring.gpg")),
                   APISecretKeyRing,
                   APIPassphrase,
                   APIAdminEMail,
                   APISMTPClient,

                   DNSClient,
                   LogfileName)

        {

            RegisterCPOURITemplates();

        }

        #endregion

        #region CPOAPI(HTTPServer, ...)

        /// <summary>
        /// Create an instance of the OCPI HTTP API for Charge Point Operators
        /// using the given HTTP server.
        /// </summary>
        public CPOAPI(RoamingNetwork                               RoamingNetwork,
                      HTTPServer<RoamingNetworks, RoamingNetwork>  HTTPServer,
                      HTTPPath?                                     URIPrefix         = null,

                      String                                       ServiceName       = DefaultHTTPServerName,
                      EMailAddress                                 APIEMailAddress   = null,
                      PgpSecretKeyRing                             APISecretKeyRing  = null,
                      String                                       APIPassphrase     = null,
                      EMailAddressList                             APIAdminEMail     = null,
                      SMTPClient                                   APISMTPClient     = null)

            : base(RoamingNetwork,
                   HTTPServer,
                   URIPrefix ?? DefaultURIPrefix,
                   ResourceName => typeof(CPOAPI).Assembly.GetManifestResourceStream("cloud.charging.open.protocols.OCPIv2_2.HTTPAPI.CPOAPI.HTTPRoot." + ResourceName),

                   ServiceName,
                   APIEMailAddress,
                   null, //OpenPGP.ReadPublicKeyRing(typeof(CPOAPI).Assembly.GetManifestResourceStream("cloud.charging.open.protocols.OCPIv2_2.HTTPAPI.CPOAPI.HTTPRoot.About.robot@offenes-jena_pubring.gpg")),
                   APISecretKeyRing,
                   APIPassphrase,
                   APIAdminEMail,
                   APISMTPClient,

                   LogfileName)

        {

            RegisterCPOURITemplates();

        }

        #endregion

        #endregion


        #region (private) RegisterCPOURITemplates()

        private void RegisterCPOURITemplates()
        {

            #region /cpo

            _HTTPServer.RegisterResourcesFolder(HTTPHostname.Any,
                                                URLPrefix + "/cpo", "cloud.charging.open.protocols.OCPIv2_2.HTTPAPI.CPOAPI.HTTPRoot",
                                                Assembly.GetCallingAssembly());

            _HTTPServer.AddMethodCallback(HTTPHostname.Any,
                                          HTTPMethod.GET,
                                          new HTTPPath[] {
                                              URLPrefix + "/cpo/index.html",
                                              URLPrefix + "/cpo/"
                                          },
                                          HTTPContentType.HTML_UTF8,
                                          HTTPDelegate: async Request => {

                                              var _MemoryStream = new MemoryStream();
                                              typeof(CPOAPI).Assembly.GetManifestResourceStream("cloud.charging.open.protocols.OCPIv2_2.HTTPAPI.CPOAPI.HTTPRoot._header.html").SeekAndCopyTo(_MemoryStream, 3);
                                              typeof(CPOAPI).Assembly.GetManifestResourceStream("cloud.charging.open.protocols.OCPIv2_2.HTTPAPI.CPOAPI.HTTPRoot._footer.html").SeekAndCopyTo(_MemoryStream, 3);

                                              return new HTTPResponse.Builder(Request) {
                                                  HTTPStatusCode  = HTTPStatusCode.OK,
                                                  Server          = DefaultHTTPServerName,
                                                  Date            = DateTime.Now,
                                                  ContentType     = HTTPContentType.HTML_UTF8,
                                                  Content         = _MemoryStream.ToArray(),
                                                  Connection      = "close"
                                              };

                                          });

            #endregion

            #region /cpo/versions

            _HTTPServer.AddMethodCallback(HTTPHostname.Any,
                                          HTTPMethod.GET,
                                          URLPrefix + "/cpo/versions",
                                          HTTPContentType.JSON_UTF8,
                                          HTTPDelegate: async Request => {

                                              return new HTTPResponse.Builder(Request) {
                                                  HTTPStatusCode  = HTTPStatusCode.OK,
                                                  Server          = DefaultHTTPServerName,
                                                  Date            = DateTime.Now,
                                                  ContentType     = HTTPContentType.HTML_UTF8,
                                                  Content         = new JArray(new JObject(
                                                                                   new JProperty("version",  "2.0"),
                                                                                   new JProperty("url",      "http://" + Request.Host + "/cpo/versions/2.0/")
                                                                    )).ToUTF8Bytes(),
                                                  Connection      = "close"
                                              };

                                          });

            #endregion

            #region /cpo/versions/2.2/

            _HTTPServer.AddMethodCallback(HTTPHostname.Any,
                                          HTTPMethod.GET,
                                          URLPrefix + "/cpo/versions/2.2/",
                                          HTTPContentType.JSON_UTF8,
                                          HTTPDelegate: async Request => {

                                              return new HTTPResponse.Builder(Request) {
                                                  HTTPStatusCode  = HTTPStatusCode.OK,
                                                  Server          = DefaultHTTPServerName,
                                                  Date            = DateTime.Now,
                                                  ContentType     = HTTPContentType.HTML_UTF8,
                                                  Content         = JSONObject.Create(
                                                                        new JProperty("version",  "2.2"),
                                                                        new JProperty("endpoints", new JArray(
                                                                            new JObject(
                                                                                new JProperty("identifier", "credentials"),
                                                                                new JProperty("url",        "http://" + Request.Host + "/cpo/versions/2.2/credentials/")
                                                                            ),
                                                                            new JObject(
                                                                                new JProperty("identifier", "locations"),
                                                                                new JProperty("url",        "http://" + Request.Host + "/cpo/versions/2.2/locations/")
                                                                            )
                                                                    ))).ToUTF8Bytes(),
                                                  Connection      = "close"
                                              };

                                          });

            #endregion



            #region /cpo/versions/2.2/locations

            _HTTPServer.AddMethodCallback(HTTPHostname.Any,
                                          HTTPMethod.GET,
                                          URLPrefix + "/cpo/versions/2.2/locations",
                                          HTTPContentType.JSON_UTF8,
                                          HTTPDelegate: async Request => {

                                              var from                         = Request.QueryString.GetDateTime("date_from");
                                              var to                           = Request.QueryString.GetDateTime("date_to");
                                              var offset                       = Request.QueryString.GetUInt64("offset");
                                              var limit                        = Request.QueryString.GetUInt64("limit");



                                              // X-Total-Count    The total number of objects available in the server system that match the given query (including the given query parameters.
                                              // X-Limit          The maximum number of objects that the server WILL return.

                                              return new HTTPResponse.Builder(Request) {
                                                  HTTPStatusCode  = HTTPStatusCode.OK,
                                                  Server          = DefaultHTTPServerName,
                                                  Date            = DateTime.Now,
                                                  ContentType     = HTTPContentType.HTML_UTF8,
                                                  Content         = JSONObject.Create(
                                                                        new JProperty("version",  "2.2"),
                                                                        new JProperty("endpoints", new JArray(
                                                                            new JObject(
                                                                                new JProperty("identifier", "credentials"),
                                                                                new JProperty("url",        "http://" + Request.Host + "/cpo/versions/2.2/credentials/")
                                                                            ),
                                                                            new JObject(
                                                                                new JProperty("identifier", "locations"),
                                                                                new JProperty("url",        "http://" + Request.Host + "/cpo/versions/2.2/locations/")
                                                                            )
                                                                    ))).ToUTF8Bytes(),
                                                  Connection      = "close"
                                              };

                                          });

            #endregion

            #region /cpo/versions/2.2/locations/{locationId}

            _HTTPServer.AddMethodCallback(HTTPHostname.Any,
                                          HTTPMethod.GET,
                                          URLPrefix + "/cpo/versions/2.2/locations/{locationId}",
                                          HTTPContentType.JSON_UTF8,
                                          HTTPDelegate: async Request => {

                                              return new HTTPResponse.Builder(Request) {
                                                  HTTPStatusCode  = HTTPStatusCode.OK,
                                                  Server          = DefaultHTTPServerName,
                                                  Date            = DateTime.Now,
                                                  ContentType     = HTTPContentType.HTML_UTF8,
                                                  Content         = JSONObject.Create(
                                                                        new JProperty("version",  "2.2"),
                                                                        new JProperty("endpoints", new JArray(
                                                                            new JObject(
                                                                                new JProperty("identifier", "credentials"),
                                                                                new JProperty("url",        "http://" + Request.Host + "/cpo/versions/2.2/credentials/")
                                                                            ),
                                                                            new JObject(
                                                                                new JProperty("identifier", "locations"),
                                                                                new JProperty("url",        "http://" + Request.Host + "/cpo/versions/2.2/locations/")
                                                                            )
                                                                    ))).ToUTF8Bytes(),
                                                  Connection      = "close"
                                              };

                                          });

            #endregion

            #region /cpo/versions/2.2/locations/{locationId}/{evseId}

            _HTTPServer.AddMethodCallback(HTTPHostname.Any,
                                          HTTPMethod.GET,
                                          URLPrefix + "/cpo/versions/2.2/locations/{locationId}/{evseId}",
                                          HTTPContentType.JSON_UTF8,
                                          HTTPDelegate: async Request => {


                                              return new HTTPResponse.Builder(Request) {
                                                  HTTPStatusCode  = HTTPStatusCode.OK,
                                                  Server          = DefaultHTTPServerName,
                                                  Date            = DateTime.Now,
                                                  ContentType     = HTTPContentType.HTML_UTF8,
                                                  Content         = JSONObject.Create(
                                                                        new JProperty("version",  "2.2"),
                                                                        new JProperty("endpoints", new JArray(
                                                                            new JObject(
                                                                                new JProperty("identifier", "credentials"),
                                                                                new JProperty("url",        "http://" + Request.Host + "/cpo/versions/2.2/credentials/")
                                                                            ),
                                                                            new JObject(
                                                                                new JProperty("identifier", "locations"),
                                                                                new JProperty("url",        "http://" + Request.Host + "/cpo/versions/2.2/locations/")
                                                                            )
                                                                    ))).ToUTF8Bytes(),
                                                  Connection      = "close"
                                              };

                                          });

            #endregion

            #region /cpo/versions/2.2/locations/{locationId}/{evseId}/{connectorId}

            _HTTPServer.AddMethodCallback(HTTPHostname.Any,
                                          HTTPMethod.GET,
                                          URLPrefix + "/cpo/versions/2.2/locations/{locationId}/{evseId}/{connectorId}",
                                          HTTPContentType.JSON_UTF8,
                                          HTTPDelegate: async Request => {


                                              return new HTTPResponse.Builder(Request) {
                                                  HTTPStatusCode  = HTTPStatusCode.OK,
                                                  Server          = DefaultHTTPServerName,
                                                  Date            = DateTime.Now,
                                                  ContentType     = HTTPContentType.HTML_UTF8,
                                                  Content         = JSONObject.Create(
                                                                        new JProperty("version",  "2.2"),
                                                                        new JProperty("endpoints", new JArray(
                                                                            new JObject(
                                                                                new JProperty("identifier", "credentials"),
                                                                                new JProperty("url",        "http://" + Request.Host + "/cpo/versions/2.2/credentials/")
                                                                            ),
                                                                            new JObject(
                                                                                new JProperty("identifier", "locations"),
                                                                                new JProperty("url",        "http://" + Request.Host + "/cpo/versions/2.2/locations/")
                                                                            )
                                                                    ))).ToUTF8Bytes(),
                                                  Connection      = "close"
                                              };

                                          });

            #endregion


        }

        #endregion


    }

}
