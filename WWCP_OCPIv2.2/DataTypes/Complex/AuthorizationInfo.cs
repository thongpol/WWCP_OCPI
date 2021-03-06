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
using System.Linq;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_2
{

    /// <summary>
    /// An authorization information.
    /// </summary>
    public class AuthorizationInfo : IEquatable<AuthorizationInfo>
    {

        #region Properties

        /// <summary>
        /// Status of the token, and whether charging is allowed at the optionally given
        /// charging location.
        /// </summary>
        [Mandatory]
        public AllowedTypes              Allowed                   { get; }

        /// <summary>
        /// The complete Token object for which this authorization was requested.
        /// </summary>
        [Mandatory]
        public Token                     Token                     { get; }

        /// <summary>
        /// Optional reference to the location if it was included in the request, and if
        /// the EV driver is allowed to charge at that location. Only the EVSEs the EV
        /// driver is allowed to charge at are returned.
        /// </summary>
        [Optional]
        public LocationReference?        Location                  { get; }

        /// <summary>
        /// Reference to the authorization given by the eMSP, when given, this reference
        /// will be provided in the relevant charging session and/or charge detail record.
        /// </summary>
        [Optional]
        public AuthorizationReference?   AuthorizationReference    { get; }

        /// <summary>
        /// Optional display text, additional information to the EV driver.
        /// </summary>
        [Optional]
        public DisplayText?              Info                      { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// An authorization information consists of a start timestamp and a
        /// list of possible values that influence this period.
        /// </summary>
        public AuthorizationInfo(AllowedTypes              Allowed,
                                 Token                     Token,
                                 LocationReference?        Location                 = null,
                                 AuthorizationReference?   AuthorizationReference   = null,
                                 DisplayText?              Info                     = null)
        {

            this.Allowed                 = Allowed;
            this.Token                   = Token;
            this.Location                = Location;
            this.AuthorizationReference  = AuthorizationReference;
            this.Info                    = Info;

        }

        #endregion


        #region (static) Parse   (JSON, CustomAuthorizationInfoParser = null)

        /// <summary>
        /// Parse the given JSON representation of an authorization information.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CustomAuthorizationInfoParser">A delegate to parse custom authorization information JSON objects.</param>
        public static AuthorizationInfo Parse(JObject                                         JSON,
                                              CustomJObjectParserDelegate<AuthorizationInfo>  CustomAuthorizationInfoParser   = null)
        {

            if (TryParse(JSON,
                         out AuthorizationInfo  authorizationInfo,
                         out String             ErrorResponse,
                         CustomAuthorizationInfoParser))
            {
                return authorizationInfo;
            }

            throw new ArgumentException("The given JSON representation of an authorization information is invalid: " + ErrorResponse, nameof(JSON));

        }

        #endregion

        #region (static) Parse   (Text, CustomAuthorizationInfoParser = null)

        /// <summary>
        /// Parse the given text representation of an authorization information.
        /// </summary>
        /// <param name="Text">The text to parse.</param>
        /// <param name="CustomAuthorizationInfoParser">A delegate to parse custom authorization information JSON objects.</param>
        public static AuthorizationInfo Parse(String                                Text,
                                    CustomJObjectParserDelegate<AuthorizationInfo>  CustomAuthorizationInfoParser   = null)
        {

            if (TryParse(Text,
                         out AuthorizationInfo  authorizationInfo,
                         out String             ErrorResponse,
                         CustomAuthorizationInfoParser))
            {
                return authorizationInfo;
            }

            throw new ArgumentException("The given text representation of an authorization information is invalid: " + ErrorResponse, nameof(Text));

        }

        #endregion

        #region (static) TryParse(JSON, out AuthorizationInfo, out ErrorResponse, CustomAuthorizationInfoParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of an authorization information.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="AuthorizationInfo">The parsed authorization information.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                JSON,
                                       out AuthorizationInfo  AuthorizationInfo,
                                       out String             ErrorResponse)

            => TryParse(JSON,
                        out AuthorizationInfo,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of an authorization information.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="AuthorizationInfo">The parsed authorization information.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomAuthorizationInfoParser">A delegate to parse custom authorization information JSON objects.</param>
        public static Boolean TryParse(JObject                                         JSON,
                                       out AuthorizationInfo                           AuthorizationInfo,
                                       out String                                      ErrorResponse,
                                       CustomJObjectParserDelegate<AuthorizationInfo>  CustomAuthorizationInfoParser   = null)
        {

            try
            {

                AuthorizationInfo = default;

                if (JSON?.HasValues != true)
                {
                    ErrorResponse = "The given JSON object must not be null or empty!";
                    return false;
                }

                #region Parse Allowed                   [mandatory]

                if (!JSON.ParseMandatoryEnum("allowed",
                                             "allowed",
                                             out AllowedTypes Allowed,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse Token                     [mandatory]

                if (!JSON.ParseMandatoryJSON2("token",
                                              "token",
                                              OCPIv2_2.Token.TryParse,
                                              out Token Token,
                                              out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse LocationReference         [optional]

                if (JSON.ParseOptionalJSON("location",
                                           "location reference",
                                           OCPIv2_2.LocationReference.TryParse,
                                           out LocationReference? LocationReference,
                                           out ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                }

                #endregion

                #region Parse AuthorizationReference    [optional]

                if (JSON.ParseOptional("authorization_reference",
                                       "authorization reference",
                                       OCPIv2_2.AuthorizationReference.TryParse,
                                       out AuthorizationReference? AuthorizationReference,
                                       out ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                }

                #endregion

                #region Parse Info                      [optional]

                if (JSON.ParseOptionalJSON("info",
                                           "multi-language information",
                                           DisplayText.TryParse,
                                           out DisplayText? Info,
                                           out ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                }

                #endregion


                AuthorizationInfo = new AuthorizationInfo(Allowed,
                                                          Token,
                                                          LocationReference,
                                                          AuthorizationReference,
                                                          Info);


                if (CustomAuthorizationInfoParser != null)
                    AuthorizationInfo = CustomAuthorizationInfoParser(JSON,
                                                                      AuthorizationInfo);

                return true;

            }
            catch (Exception e)
            {
                AuthorizationInfo  = default;
                ErrorResponse      = "The given JSON representation of an authorization information is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region (static) TryParse(Text, out AuthorizationInfo, out ErrorResponse, CustomAuthorizationInfoParser = null)

        /// <summary>
        /// Try to parse the given text representation of an authorizationInfo.
        /// </summary>
        /// <param name="Text">The text to parse.</param>
        /// <param name="AuthorizationInfo">The parsed authorizationInfo.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomAuthorizationInfoParser">A delegate to parse custom authorizationInfo JSON objects.</param>
        public static Boolean TryParse(String                                          Text,
                                       out AuthorizationInfo                           AuthorizationInfo,
                                       out String                                      ErrorResponse,
                                       CustomJObjectParserDelegate<AuthorizationInfo>  CustomAuthorizationInfoParser   = null)
        {

            try
            {

                return TryParse(JObject.Parse(Text),
                                out AuthorizationInfo,
                                out ErrorResponse,
                                CustomAuthorizationInfoParser);

            }
            catch (Exception e)
            {
                AuthorizationInfo  = null;
                ErrorResponse      = "The given text representation of an authorization information is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomAuthorizationInfoSerializer = null, CustomTokenSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomAuthorizationInfoSerializer">A delegate to serialize custom authorizationInfo JSON objects.</param>
        /// <param name="CustomTokenSerializer">A delegate to serialize custom token JSON objects.</param>
        /// <param name="CustomLocationReferenceSerializer">A delegate to serialize custom location reference JSON objects.</param>
        /// <param name="CustomDisplayTextSerializer">A delegate to serialize custom multi-language text JSON objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<AuthorizationInfo>  CustomAuthorizationInfoSerializer   = null,
                              CustomJObjectSerializerDelegate<Token>              CustomTokenSerializer               = null,
                              CustomJObjectSerializerDelegate<LocationReference>  CustomLocationReferenceSerializer   = null,
                              CustomJObjectSerializerDelegate<DisplayText>        CustomDisplayTextSerializer         = null)
        {

            var JSON = JSONObject.Create(

                           new JProperty("allowed",                         Allowed.                     ToString()),
                           new JProperty("token",                           Token.                       ToJSON(CustomTokenSerializer)),

                           Location.HasValue
                               ? new JProperty("location",                  Location.              Value.ToJSON(CustomLocationReferenceSerializer))
                               : null,

                           AuthorizationReference.HasValue
                               ? new JProperty("authorization_reference",   AuthorizationReference.Value.ToString())
                               : null,

                           Info.HasValue
                               ? new JProperty("info",                      Info.                  Value.ToJSON(CustomDisplayTextSerializer))
                               : null

                       );

            return CustomAuthorizationInfoSerializer != null
                       ? CustomAuthorizationInfoSerializer(this, JSON)
                       : JSON;

        }

        #endregion


        #region Operator overloading

        #region Operator == (AuthorizationInfo1, AuthorizationInfo2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AuthorizationInfo1">An authorization information.</param>
        /// <param name="AuthorizationInfo2">Another authorization information.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (AuthorizationInfo AuthorizationInfo1,
                                           AuthorizationInfo AuthorizationInfo2)
        {

            if (Object.ReferenceEquals(AuthorizationInfo1, AuthorizationInfo2))
                return true;

            if (AuthorizationInfo1 is null || AuthorizationInfo2 is null)
                return false;

            return AuthorizationInfo1.Equals(AuthorizationInfo2);

        }

        #endregion

        #region Operator != (AuthorizationInfo1, AuthorizationInfo2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AuthorizationInfo1">An authorization information.</param>
        /// <param name="AuthorizationInfo2">Another authorization information.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (AuthorizationInfo AuthorizationInfo1,
                                           AuthorizationInfo AuthorizationInfo2)

            => !(AuthorizationInfo1 == AuthorizationInfo2);

        #endregion

        #endregion

        #region IEquatable<AuthorizationInfo> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        /// <returns>true|false</returns>
        public override Boolean Equals(Object Object)

            => Object is AuthorizationInfo authorizationReference &&
                   Equals(authorizationReference);

        #endregion

        #region Equals(AuthorizationInfo)

        /// <summary>
        /// Compares two authorization informations for equality.
        /// </summary>
        /// <param name="AuthorizationInfo">An authorization information to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(AuthorizationInfo AuthorizationInfo)

            => !(AuthorizationInfo is null) &&

               Allowed.               Equals(AuthorizationInfo.Allowed) &&
               Token.                 Equals(AuthorizationInfo.Token)   &&

               Location.              HasValue && AuthorizationInfo.Location.              HasValue && Location.              Value.Equals(AuthorizationInfo.Location.              Value) &&
               AuthorizationReference.HasValue && AuthorizationInfo.AuthorizationReference.HasValue && AuthorizationReference.Value.Equals(AuthorizationInfo.AuthorizationReference.Value) &&
               Info.                  HasValue && AuthorizationInfo.Info.                  HasValue && Info.                  Value.Equals(AuthorizationInfo.Info.                  Value);

        #endregion

        #endregion

        #region GetHashCode()

        /// <summary>
        /// Return the hash code of this object.
        /// </summary>
        /// <returns>The hash code of this object.</returns>
        public override Int32 GetHashCode()
        {
            unchecked
            {

                return Allowed.                            GetHashCode() * 11 ^
                       Token.                              GetHashCode() *  7 ^

                       (Location.HasValue
                            ? Location.              Value.GetHashCode() *  5
                            : 0) ^

                       (AuthorizationReference.HasValue
                            ? AuthorizationReference.Value.GetHashCode() *  3
                            : 0) ^

                       (Info.HasValue
                            ? Info.                  Value.GetHashCode()
                            : 0);

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(Token,
                             " -> ",
                             Allowed);

        #endregion

    }

}
