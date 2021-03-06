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
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.DNS;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using System.Linq;
using System.Threading.Tasks;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_2.HTTP
{




    /// <summary>
    /// A HTTP delegate.
    /// </summary>
    /// <param name="Request">The HTTP request.</param>
    /// <returns>A HTTP response task.</returns>
    public delegate Task<OCPIResponse.Builder> OCPIRequestDelegate(OCPIRequest Request);


    /// <summary>
    /// Extention methods for OCPI requests.
    /// </summary>
    public static class OCPIRequestExtentions
    {

        #region AddOCPIMethod(CommonAPI, Hostname, HTTPMethod, URLTemplate,  HTTPContentType = null, URLAuthentication = false, HTTPMethodAuthentication = false, ContentTypeAuthentication = false, HTTPDelegate = null)

        /// <summary>
        /// Add a method callback for the given URL template.
        /// </summary>
        /// <param name="CommonAPI">The OCPI Common API.</param>
        /// <param name="Hostname">The HTTP hostname.</param>
        /// <param name="HTTPMethod">The HTTP method.</param>
        /// <param name="URLTemplate">The URL template.</param>
        /// <param name="HTTPContentType">The HTTP content type.</param>
        /// <param name="URLAuthentication">Whether this method needs explicit uri authentication or not.</param>
        /// <param name="HTTPMethodAuthentication">Whether this method needs explicit HTTP method authentication or not.</param>
        /// <param name="ContentTypeAuthentication">Whether this method needs explicit HTTP content type authentication or not.</param>
        /// <param name="OCPIRequestLogger">A OCPI request logger.</param>
        /// <param name="OCPIResponseLogger">A OCPI response logger.</param>
        /// <param name="DefaultErrorHandler">The default error handler.</param>
        /// <param name="OCPIRequest">The method to call.</param>
        public static void AddOCPIMethod(this CommonAPI          CommonAPI,
                                         HTTPHostname            Hostname,
                                         HTTPMethod              HTTPMethod,
                                         HTTPPath                URLTemplate,
                                         HTTPContentType         HTTPContentType             = null,
                                         HTTPAuthentication      URLAuthentication           = null,
                                         HTTPAuthentication      HTTPMethodAuthentication    = null,
                                         HTTPAuthentication      ContentTypeAuthentication   = null,
                                         OCPIRequestLogHandler   OCPIRequestLogger           = null,
                                         OCPIResponseLogHandler  OCPIResponseLogger          = null,
                                         HTTPDelegate            DefaultErrorHandler         = null,
                                         OCPIRequestDelegate     OCPIRequest                 = null,
                                         URLReplacement          AllowReplacement            = URLReplacement.Fail)

        {

            CommonAPI.HTTPServer.
                      AddMethodCallback(Hostname,
                                        HTTPMethod,
                                        URLTemplate,
                                        HTTPContentType,
                                        URLAuthentication,
                                        HTTPMethodAuthentication,
                                        ContentTypeAuthentication,
                                        (timestamp, httpAPI, httpRequest)               => OCPIRequestLogger?. Invoke(timestamp, null, HTTP.OCPIRequest.Parse(httpRequest, CommonAPI)),
                                        (timestamp, httpAPI, httpRequest, httpResponse) => OCPIResponseLogger?.Invoke(timestamp, null, httpRequest. SubprotocolRequest  as OCPIRequest,
                                                                                                                                       httpResponse.SubprotocolResponse as OCPIResponse),
                                        DefaultErrorHandler,
                                        async httpRequest => {

                                            var OCPIResponseBuilder = await OCPIRequest(httpRequest.SubprotocolRequest is null
                                                                                ? HTTP.OCPIRequest.Parse(httpRequest, CommonAPI) // When no OCPIRequestLogger was used!
                                                                                : httpRequest.SubprotocolRequest as OCPIRequest);

                                            var httpResponseBuilder = OCPIResponseBuilder.ToHTTPResponseBuilder();
                                            httpResponseBuilder.SubprotocolResponse = new OCPIResponse(OCPIResponseBuilder.Request,
                                                                                                       OCPIResponseBuilder.StatusCode ?? 3000,
                                                                                                       OCPIResponseBuilder.StatusMessage,
                                                                                                       OCPIResponseBuilder.AdditionalInformation,
                                                                                                       OCPIResponseBuilder.Timestamp ?? DateTime.UtcNow,
                                                                                                       httpResponseBuilder.AsImmutable);

                                            return httpResponseBuilder;

                                        },
                                        AllowReplacement);

        }

        #endregion

    }


    /// <summary>
    /// An OCPI HTTP request.
    /// </summary>
    public class OCPIRequest // : HTTPRequest
    {

        public readonly struct DateAndPaginationFilters
        {

            public DateTime?  From     { get; }
            public DateTime?  To       { get; }
            public UInt64?    Offset   { get; }
            public UInt64?    Limit    { get; }


            public DateAndPaginationFilters(DateTime?  From,
                                            DateTime?  To,
                                            UInt64?    Offset,
                                            UInt64?    Limit)
            {

                this.From    = From;
                this.To      = To;
                this.Offset  = Offset;
                this.Limit   = Limit;

            }

        }


        #region Properties

        public CommonAPI        CommonAPI           { get; }

        public HTTPRequest      HTTPRequest         { get; }

        public Request_Id?      RequestId           { get; }
        public Correlation_Id?  CorrelationId       { get; }
        public CountryCode?     ToCountryCode       { get; }
        public Party_Id?        ToPartyId           { get; }
        public CountryCode?     FromCountryCode     { get; }
        public Party_Id?        FromPartyId         { get; }

        public AccessToken?     AccessToken         { get; }

        public AccessInfo?      AccessInfo          { get; }

        public AccessInfo2?     AccessInfo2         { get; }

        public RemoteParty            RemoteParty         { get; }


        /// <summary>
        /// The HTTP query string.
        /// </summary>
        public HTTPHostname     Host
            => HTTPRequest.Host;

        /// <summary>
        /// The parsed URL parameters of the best matching URL template.
        /// Set by the HTTP server.
        /// </summary>
        public String[]         ParsedURLParameters
            => HTTPRequest.ParsedURLParameters;

        /// <summary>
        /// The HTTP query string.
        /// </summary>
        public QueryString      QueryString
            => HTTPRequest.QueryString;

        #endregion

        protected OCPIRequest(HTTPRequest  Request,
                              CommonAPI    CommonAPI)
        {

            this.HTTPRequest      = Request ?? throw new ArgumentNullException(nameof(HTTPRequest), "The given HTTP request must not be null!");

            this.RequestId        = Request.TryParseHeaderField<Request_Id>    ("X-Request-ID",           Request_Id.    TryParse) ?? Request_Id.    Random(IsLocal: true);
            this.CorrelationId    = Request.TryParseHeaderField<Correlation_Id>("X-Correlation-ID",       Correlation_Id.TryParse) ?? Correlation_Id.Random(IsLocal: true);
            this.ToCountryCode    = Request.TryParseHeaderField<CountryCode>   ("OCPI-to-country-code",   CountryCode.   TryParse);
            this.ToPartyId        = Request.TryParseHeaderField<Party_Id>      ("OCPI-to-party-id",       Party_Id.      TryParse);
            this.FromCountryCode  = Request.TryParseHeaderField<CountryCode>   ("OCPI-from-country-code", CountryCode.   TryParse);
            this.FromPartyId      = Request.TryParseHeaderField<Party_Id>      ("OCPI-from-party-id",     Party_Id.      TryParse);


            if (Request.Authorization is HTTPTokenAuthentication TokenAuth &&
                TokenAuth.Token.TryDecodeBase64(out String DecodedToken)   &&
                OCPIv2_2.AccessToken.TryParse(DecodedToken, out AccessToken accessToken))
            {
                this.AccessToken = accessToken;
            }

            else if (Request.Authorization is HTTPBasicAuthentication BasicAuth &&
                OCPIv2_2.AccessToken.TryParse(BasicAuth.Username, out accessToken))
            {
                this.AccessToken = accessToken;
            }

            if (this.AccessToken.HasValue)
            {

                if (CommonAPI.TryGetRemoteParties(AccessToken.Value, out IEnumerable<RemoteParty> Parties))
                {

                    if (Parties.Count() == 1)
                    {

                        this.AccessInfo   = new AccessInfo(AccessToken.Value,
                                                           Parties.First().AccessInfo.First(accessInfo2 => accessInfo2.Token == AccessToken).Status);

                        this.AccessInfo2  = Parties.First().AccessInfo.First(accessInfo2 => accessInfo2.Token == AccessToken);

                        this.RemoteParty  = Parties.First();

                    }

                    else if (Parties.Count() > 1      &&
                             FromCountryCode.HasValue &&
                             FromPartyId.    HasValue)
                    {

                        var filteredParties = Parties.Where(party => party.CountryCode == FromCountryCode.Value &&
                                                                     party.PartyId     == FromPartyId.    Value).ToArray();

                        if (filteredParties.Count() == 1)
                        {

                            this.AccessInfo   = new AccessInfo(AccessToken.Value,
                                                               filteredParties.First().AccessInfo.First(accessInfo2 => accessInfo2.Token == AccessToken).Status);

                            this.AccessInfo2  = filteredParties.First().AccessInfo.First(accessInfo2 => accessInfo2.Token == AccessToken);

                            this.RemoteParty  = filteredParties.First();

                        }

                    }

                }


            //    if (CommonAPI.TryGetAccessInfo(this.AccessToken.Value, out AccessInfo accessInfo))
            //    {

            //        this.AccessInfo = accessInfo;

            ////        var allTheirCPORoles = this.AccessInfo.Value.Roles.Where(role => role.Role == Roles.CPO).ToArray();

            ////        if (!FromCountryCode.HasValue && allTheirCPORoles.Length == 1)
            ////            this.FromCountryCode = allTheirCPORoles[0].CountryCode;

            ////        if (!FromPartyId.    HasValue && allTheirCPORoles.Length == 1)
            ////            this.FromPartyId     = allTheirCPORoles[0].PartyId;

            //    }

            }


            //var allMyCPORoles = this.AccessInfo.Value.Roles.Where(role => role.Role == Roles.CPO).ToArray();

            //if (!ToCountryCode.HasValue && allMyCPORoles.Length == 1)
            //    this.ToCountryCode = allMyCPORoles[1].CountryCode;

            //if (!ToPartyId.HasValue && allMyCPORoles.Length == 1)
            //    this.ToPartyId = allMyCPORoles[1].PartyId;


            this.HTTPRequest.SubprotocolRequest = this;

        }

        public DateAndPaginationFilters GetDateAndPaginationFilters()

            => new DateAndPaginationFilters(HTTPRequest.QueryString.GetDateTime("date_from"),
                                            HTTPRequest.QueryString.GetDateTime("date_to"),
                                            HTTPRequest.QueryString.GetUInt64  ("offset"),
                                            HTTPRequest.QueryString.GetUInt64  ("limit"));




        public Boolean TryParseJObjectRequestBody(out JObject               JSON,
                                                  out OCPIResponse.Builder  OCPIResponseBuilder,
                                                  Boolean                   AllowEmptyHTTPBody = false,
                                                  String                    JSONLDContext      = null)
        {

            var result = HTTPRequest.TryParseJObjectRequestBody(out JSON,
                                                                out HTTPResponse.Builder HTTPResponseBuilder,
                                                                AllowEmptyHTTPBody,
                                                                JSONLDContext);

            if (HTTPResponseBuilder != null)
            {
                HTTPResponseBuilder.Set("X-Request-ID",      RequestId).
                                    Set("X-Correlation-ID",  CorrelationId);
            }

            OCPIResponseBuilder = new OCPIResponse.Builder(this) {
                StatusCode           = result ? 1000 : 2001,
                StatusMessage        = result ? ""   : "Could not parse JSON object in HTTP request body!",
                HTTPResponseBuilder  = HTTPResponseBuilder
            };

            return result;

        }

        public Boolean TryParseJArrayRequestBody(out JArray               JSON,
                                                 out OCPIResponse.Builder  OCPIResponseBuilder,
                                                 Boolean                   AllowEmptyHTTPBody = false,
                                                 String                    JSONLDContext      = null)
        {

            var result = HTTPRequest.TryParseJArrayRequestBody(out JSON,
                                                               out HTTPResponse.Builder HTTPResponseBuilder,
                                                               AllowEmptyHTTPBody,
                                                               JSONLDContext);

            if (HTTPResponseBuilder != null)
            {
                HTTPResponseBuilder.Set("X-Request-ID",      RequestId).
                                    Set("X-Correlation-ID",  CorrelationId);
            }

            OCPIResponseBuilder = new OCPIResponse.Builder(this) {
                StatusCode           = result ? 1000 : 2001,
                StatusMessage        = result ? ""   : "Could not parse JSON array in HTTP request body!",
                HTTPResponseBuilder  = HTTPResponseBuilder
            };

            return result;

        }


        public static OCPIRequest Parse(HTTPRequest  HTTPRequest,
                                        CommonAPI    CommonAPI)

            => new OCPIRequest(HTTPRequest,
                               CommonAPI);


    }

}
