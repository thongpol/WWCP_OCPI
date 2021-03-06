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
    /// Charging tariff restrictions.
    /// </summary>
    public class TariffRestrictions : IEquatable<TariffRestrictions>
    {

        #region Properties

        /// <summary>
        /// Start time of day, for example "13:30", valid from this time of the day.
        /// </summary>
        [Optional]
        public Time?                         StartTime           { get; }

        /// <summary>
        /// End time of day, for example "19:45", valid from this time of the day.
        /// </summary>
        [Optional]
        public Time?                         EndTime             { get; }

        /// <summary>
        /// Start date, for example: 2015-12-24, valid from this day until that day (excluding that day).
        /// </summary>
        [Optional]
        public DateTime?                     StartDate           { get; }

        /// <summary>
        /// End date, for example: 2015-12-24, valid from this day until that day (excluding that day).
        /// </summary>
        [Optional]
        public DateTime?                     EndDate             { get; }

        /// <summary>
        /// Minimum consumed energy in kWh, for example 20, valid from this amount of energy
        /// (inclusive) being used.
        /// </summary>
        [Optional]
        public Decimal?                      MinkWh              { get; }

        /// <summary>
        /// Maximum consumed energy in kWh, for example 50, valid until this amount of energy
        /// (exclusive) being used.
        /// </summary>
        [Optional]
        public Decimal?                      MaxkWh              { get; }

        /// <summary>
        /// Sum of the minimum current (in Amperes) over all phases, for example 5. When the EV is
        /// charging with more than, or equal to, the defined amount of current, this tariff element
        /// is/becomes active. If the charging current is or becomes lower, this tariff element is
        /// not or no longer valid and becomes inactive. This describes NOT the minimum current over
        /// the entire charging session. This restriction can make a tariff element become active
        /// when the charging current is above the defined value, but the tariff element MUST no
        /// longer be active when the charging current drops below the defined value.
        /// </summary>
        [Optional]
        public Decimal?                      MinCurrent          { get; }

        /// <summary>
        /// Sum of the maximum current (in Amperes) over all phases, for example 20. When the EV is
        /// charging with less than the defined amount of current, this tariff element becomes/is
        /// active. If the charging current is or becomes higher, this tariff element is not or no
        /// longer valid and becomes inactive. This describes NOT the maximum current over the
        /// entire charging session. This restriction can make a tariff element become active when
        /// the charging current is below this value, but the tariff element MUST no longer be
        /// active when the charging current raises above the defined value.
        /// </summary>
        [Optional]
        public Decimal?                      MaxCurrent          { get; }

        /// <summary>
        /// Minimum power in kW, for example 5. When the EV is charging with more than, or equal to,
        /// the defined amount of power, this tariff element is/becomes active. If the charging power
        /// is or becomes lower, this tariff element is not or no longer valid and becomes inactive.
        /// This describes NOT the minimum power over the entire charging session. This restriction
        /// can make a tariff element become active when the charging power is above this value, but
        /// the tariff element MUST no longer be active when the charging power drops below the
        /// defined value.
        /// </summary>
        [Optional]
        public Decimal?                      MinPower            { get; }

        /// <summary>
        /// Maximum power in kW, for example 20. When the EV is charging with less than the defined
        /// amount of power, this tariff element becomes/is active. If the charging power is or
        /// becomes higher, this tariff element is not or no longer valid and becomes inactive. This
        /// describes NOT the maximum power over the entire Charging Session. This restriction can
        /// make a tariff element become active when the charging power is below this value, but the
        /// tariff element MUST no longer be active when the charging power raises above the defined
        /// value.
        /// </summary>
        [Optional]
        public Decimal?                      MaxPower            { get; }

        /// <summary>
        /// Minimum duration in seconds the charging session MUST last (inclusive). When the
        /// duration of a charging session is longer than the defined value, this tariff element is
        /// or becomes active. Before that moment, this tariff element is not yet active.
        /// </summary>
        [Optional]
        public TimeSpan?                     MinDuration         { get; }

        /// <summary>
        /// Maximum duration in seconds the charging session MUST last (exclusive). When the
        /// duration of a Charging Session is shorter than the defined value, this tariff element
        /// is or becomes active. After that moment, this tariff element is no longer active.
        /// </summary>
        [Optional]
        public TimeSpan?                     MaxDuration         { get; }

        /// <summary>
        /// Which day(s) of the week this tariff element is active.
        /// </summary>
        [Optional]
        public IEnumerable<DayOfWeek>        DayOfWeek           { get; }

        /// <summary>
        /// When this field is present, the tariff element describes reservation costs.
        /// A reservation starts when the reservation is made, and ends when the driver
        /// starts charging on the reserved EVSE/charging location, or when the reservation
        /// expires. A reservation can only have: FLAT and TIME tariff dimensions, where TIME is
        /// for the duration of the reservation.
        /// </summary>
        [Optional]
        public ReservationRestrictionTypes?  Reservation         { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new charging tariff restrictions.
        /// </summary>
        /// <param name="StartTime">Start time of day, for example "13:30", valid from this time of the day.</param>
        /// <param name="EndTime">End time of day, for example "19:45", valid from this time of the day.</param>
        /// <param name="StartDate">Start date, for example: 2015-12-24, valid from this day until that day (excluding that day).</param>
        /// <param name="EndDate">End date, for example: 2015-12-24, valid from this day until that day (excluding that day).</param>
        /// <param name="MinkWh">Minimum consumed energy in kWh, for example 20, valid from this amount of energy (inclusive) being used.</param>
        /// <param name="MaxkWh">Maximum consumed energy in kWh, for example 50, valid until this amount of energy (exclusive) being used.</param>
        /// <param name="MinCurrent">Sum of the minimum current (in Amperes) over all phases, for example 5.</param>
        /// <param name="MaxCurrent">Sum of the maximum current (in Amperes) over all phases, for example 20.</param>
        /// <param name="MinPower">Minimum power in kW, for example 5.</param>
        /// <param name="MaxPower">Maximum power in kW, for example 20.</param>
        /// <param name="MinDuration">Minimum duration in seconds the charging session MUST last (inclusive).</param>
        /// <param name="MaxDuration">Maximum duration in seconds the charging session MUST last (exclusive).</param>
        /// <param name="DayOfWeek">Which day(s) of the week this tariff element is active.</param>
        /// <param name="Reservation"> When this field is present, the tariff element describes reservation costs.</param>
        public TariffRestrictions(Time?                         StartTime     = null,
                                  Time?                         EndTime       = null,
                                  DateTime?                     StartDate     = null,
                                  DateTime?                     EndDate       = null,
                                  Decimal?                      MinkWh        = null,
                                  Decimal?                      MaxkWh        = null,
                                  Decimal?                      MinCurrent    = null,
                                  Decimal?                      MaxCurrent    = null,
                                  Decimal?                      MinPower      = null,
                                  Decimal?                      MaxPower      = null,
                                  TimeSpan?                     MinDuration   = null,
                                  TimeSpan?                     MaxDuration   = null,
                                  IEnumerable<DayOfWeek>        DayOfWeek     = null,
                                  ReservationRestrictionTypes?  Reservation   = null)
        {

            this.StartTime     = StartTime;
            this.EndTime       = EndTime;
            this.StartDate     = StartDate;
            this.EndDate       = EndDate;
            this.MinkWh        = MinkWh;
            this.MaxkWh        = MaxkWh;
            this.MinCurrent    = MinCurrent;
            this.MaxCurrent    = MaxCurrent;
            this.MinPower      = MinPower;
            this.MaxPower      = MaxPower;
            this.MinDuration   = MinDuration;
            this.MaxDuration   = MaxDuration;
            this.DayOfWeek     = DayOfWeek?.Distinct() ?? new DayOfWeek[0];
            this.Reservation   = Reservation;

        }

        #endregion


        #region (static) Parse   (JSON, CustomTariffRestrictionsParser = null)

        /// <summary>
        /// Parse the given JSON representation of tariff restrictions.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CustomTariffRestrictionsParser">A delegate to parse custom tariff restrictions JSON objects.</param>
        public static TariffRestrictions Parse(JObject                                          JSON,
                                               CustomJObjectParserDelegate<TariffRestrictions>  CustomTariffRestrictionsParser   = null)
        {

            if (TryParse(JSON,
                         out TariffRestrictions  tariffRestrictions,
                         out String              ErrorResponse,
                         CustomTariffRestrictionsParser))
            {
                return tariffRestrictions;
            }

            throw new ArgumentException("The given JSON representation of tariff restrictions is invalid: " + ErrorResponse, nameof(JSON));

        }

        #endregion

        #region (static) Parse   (Text, CustomTariffRestrictionsParser = null)

        /// <summary>
        /// Parse the given text representation of tariff restrictions.
        /// </summary>
        /// <param name="Text">The text to parse.</param>
        /// <param name="CustomTariffRestrictionsParser">A delegate to parse custom tariff restrictions JSON objects.</param>
        public static TariffRestrictions Parse(String                                           Text,
                                               CustomJObjectParserDelegate<TariffRestrictions>  CustomTariffRestrictionsParser   = null)
        {

            if (TryParse(Text,
                         out TariffRestrictions  tariffRestrictions,
                         out String              ErrorResponse,
                         CustomTariffRestrictionsParser))
            {
                return tariffRestrictions;
            }

            throw new ArgumentException("The given text representation of tariff restrictions is invalid: " + ErrorResponse, nameof(Text));

        }

        #endregion

        #region (static) TryParse(JSON, out TariffRestrictions, out ErrorResponse, CustomTariffRestrictionsParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of tariff restrictions.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="TariffRestrictions">The parsed tariff restrictions.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                 JSON,
                                       out TariffRestrictions  TariffRestrictions,
                                       out String              ErrorResponse)

            => TryParse(JSON,
                        out TariffRestrictions,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of tariff restrictions.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="TariffRestrictions">The parsed tariff restrictions.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomTariffRestrictionsParser">A delegate to parse custom tariff restrictions JSON objects.</param>
        public static Boolean TryParse(JObject                                          JSON,
                                       out TariffRestrictions                           TariffRestrictions,
                                       out String                                       ErrorResponse,
                                       CustomJObjectParserDelegate<TariffRestrictions>  CustomTariffRestrictionsParser   = null)
        {

            try
            {

                TariffRestrictions = default;

                if (JSON?.HasValues != true)
                {
                    ErrorResponse = null;
                    return true;
                }

                #region Parse StartTime         [optional]

                if (JSON.ParseOptional("start_time",
                                       "start time",
                                       Time.TryParse,
                                       out Time? StartTime,
                                       out ErrorResponse))
                {
                    if (ErrorResponse != null)
                        return false;
                }

                #endregion

                #region Parse EndTime           [optional]

                if (JSON.ParseOptional("end_time",
                                       "end time",
                                       Time.TryParse,
                                       out Time? EndTime,
                                       out ErrorResponse))
                {
                    if (ErrorResponse != null)
                        return false;
                }

                #endregion

                #region Parse StartDate         [optional]

                if (JSON.ParseOptional("start_date",
                                       "start date",
                                       out DateTime? StartDate,
                                       out ErrorResponse))
                {
                    if (ErrorResponse != null)
                        return false;
                }

                #endregion

                #region Parse EndDate           [optional]

                if (JSON.ParseOptional("end_date",
                                       "end date",
                                       out DateTime? EndDate,
                                       out ErrorResponse))
                {
                    if (ErrorResponse != null)
                        return false;
                }

                #endregion

                #region Parse MinKWh            [optional]

                if (JSON.ParseOptional("min_kwh",
                                       "minimum consumed kWh",
                                       out Decimal? MinKWh,
                                       out ErrorResponse))
                {
                    if (ErrorResponse != null)
                        return false;
                }

                #endregion

                #region Parse MaxKWh            [optional]

                if (JSON.ParseOptional("max_kwh",
                                       "maximum consumed kWh",
                                       out Decimal? MaxKWh,
                                       out ErrorResponse))
                {
                    if (ErrorResponse != null)
                        return false;
                }

                #endregion

                #region Parse MinCurrent        [optional]

                if (JSON.ParseOptional("min_current",
                                       "minimum current",
                                       out Decimal? MinCurrent,
                                       out ErrorResponse))
                {
                    if (ErrorResponse != null)
                        return false;
                }

                #endregion

                #region Parse MaxCurrent        [optional]

                if (JSON.ParseOptional("max_current",
                                       "maximum current",
                                       out Decimal? MaxCurrent,
                                       out ErrorResponse))
                {
                    if (ErrorResponse != null)
                        return false;
                }

                #endregion

                #region Parse MinPower          [optional]

                if (JSON.ParseOptional("min_power",
                                       "minimum power",
                                       out Decimal? MinPower,
                                       out ErrorResponse))
                {
                    if (ErrorResponse != null)
                        return false;
                }

                #endregion

                #region Parse MaxPower          [optional]

                if (JSON.ParseOptional("max_power",
                                       "maximum power",
                                       out Decimal? MaxPower,
                                       out ErrorResponse))
                {
                    if (ErrorResponse != null)
                        return false;
                }

                #endregion

                #region Parse MinDuration       [optional]

                if (JSON.ParseOptional("min_duration",
                                       "minimum duration",
                                       out Double? MinDurationSec,
                                       out ErrorResponse))
                {
                    if (ErrorResponse != null)
                        return false;
                }

                #endregion

                #region Parse MaxDuration       [optional]

                if (JSON.ParseOptional("max_duration",
                                       "maximum duration",
                                       out Double? MaxDurationSec,
                                       out ErrorResponse))
                {
                    if (ErrorResponse != null)
                        return false;
                }

                #endregion

                #region Parse DayOfWeek         [optional]

                // "day_of_week": ["MONDAY", "TUESDAY", "WEDNESDAY", "THURSDAY", "FRIDAY"]

                if (JSON.ParseOptionalEnums("day_of_week",
                                            "day of week",
                                            out IEnumerable<DayOfWeek> DayOfWeek,
                                            out ErrorResponse))
                {
                    if (ErrorResponse != null)
                        return false;
                }

                #endregion

                #region Parse Reservation       [optional]

                if (JSON.ParseOptionalEnum("reservation",
                                           "reservation restriction",
                                           out ReservationRestrictionTypes? Reservation,
                                           out ErrorResponse))
                {
                    if (ErrorResponse != null)
                        return false;
                }

                #endregion


                TariffRestrictions  = (StartTime.     HasValue  ||
                                       EndTime.       HasValue  ||
                                       StartDate.     HasValue  ||
                                       EndDate.       HasValue  ||
                                       MinKWh.        HasValue  ||
                                       MaxKWh.        HasValue  ||
                                       MinCurrent.    HasValue  ||
                                       MaxCurrent.    HasValue  ||
                                       MinPower.      HasValue  ||
                                       MaxPower.      HasValue  ||
                                       MinDurationSec.HasValue  ||
                                       MaxDurationSec.HasValue  ||
                                       DayOfWeek.     SafeAny() ||
                                       Reservation.   HasValue)

                                           ? new TariffRestrictions(StartTime,
                                                                    EndTime,
                                                                    StartDate,
                                                                    EndDate,
                                                                    MinKWh,
                                                                    MaxKWh,
                                                                    MinCurrent,
                                                                    MaxCurrent,
                                                                    MinPower,
                                                                    MaxPower,
                                                                    MinDurationSec.HasValue ? new TimeSpan?(TimeSpan.FromSeconds(MinDurationSec.Value)) : null,
                                                                    MaxDurationSec.HasValue ? new TimeSpan?(TimeSpan.FromSeconds(MaxDurationSec.Value)) : null,
                                                                    DayOfWeek,
                                                                    Reservation)

                                           : null;


                if (CustomTariffRestrictionsParser != null)
                    TariffRestrictions = CustomTariffRestrictionsParser(JSON,
                                                                        TariffRestrictions);

                return true;

            }
            catch (Exception e)
            {
                TariffRestrictions  = default;
                ErrorResponse       = "The given JSON representation of tariff restrictions is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region (static) TryParse(Text, out TariffRestrictions, out ErrorResponse, CustomTariffRestrictionsParser = null)

        /// <summary>
        /// Try to parse the given text representation of tariff restrictions.
        /// </summary>
        /// <param name="Text">The text to parse.</param>
        /// <param name="TariffRestrictions">The parsed tariffRestrictions.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomTariffRestrictionsParser">A delegate to parse custom tariff restrictions JSON objects.</param>
        public static Boolean TryParse(String                                           Text,
                                       out TariffRestrictions                           TariffRestrictions,
                                       out String                                       ErrorResponse,
                                       CustomJObjectParserDelegate<TariffRestrictions>  CustomTariffRestrictionsParser   = null)
        {

            try
            {

                return TryParse(JObject.Parse(Text),
                                out TariffRestrictions,
                                out ErrorResponse,
                                CustomTariffRestrictionsParser);

            }
            catch (Exception e)
            {
                TariffRestrictions  = default;
                ErrorResponse       = "The given text representation of tariff restrictions is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomTariffRestrictionsSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomTariffRestrictionsSerializer">A delegate to serialize custom tariff restrictions JSON objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<TariffRestrictions> CustomTariffRestrictionsSerializer = null)
        {

            var JSON = JSONObject.Create(

                           StartTime.  HasValue
                               ? new JProperty("start_time",    StartTime.  Value.ToString())
                               : null,

                           EndTime.    HasValue
                               ? new JProperty("end_time",      EndTime.    Value.ToString())
                               : null,

                           StartDate.  HasValue
                               ? new JProperty("start_date",    StartDate.  Value.ToString("yyyy-MM-dd"))
                               : null,

                           EndDate.    HasValue
                               ? new JProperty("end_date",      EndDate.    Value.ToString("yyyy-MM-dd"))
                               : null,

                           MinkWh.     HasValue
                               ? new JProperty("min_kwh",       MinkWh.     Value)
                               : null,

                           MaxkWh.     HasValue
                               ? new JProperty("max_kwh",       MaxkWh.     Value)
                               : null,

                           MinCurrent. HasValue
                               ? new JProperty("min_current",   MinCurrent. Value)
                               : null,

                           MaxCurrent. HasValue
                               ? new JProperty("max_current",   MaxCurrent. Value)
                               : null,

                           MinPower.   HasValue
                               ? new JProperty("min_power",     MinPower.   Value)
                               : null,

                           MaxPower.   HasValue
                               ? new JProperty("max_power",     MaxPower.   Value)
                               : null,

                           MinDuration.HasValue
                               ? new JProperty("min_duration",  MinDuration.Value.TotalSeconds)
                               : null,

                           MaxDuration.HasValue
                               ? new JProperty("max_duration",  MaxDuration.Value.TotalSeconds)
                               : null,

                           DayOfWeek.SafeAny()
                               ? new JProperty("day_of_week",   new JArray(DayOfWeek.Select(day => day.ToString().ToUpper())))
                               : null,

                           Reservation.HasValue
                               ? new JProperty("reservation",   Reservation.ToString())
                               : null

                       );

            var JSON2 = CustomTariffRestrictionsSerializer != null
                            ? CustomTariffRestrictionsSerializer(this, JSON)
                            : JSON;

            return JSON2.HasValues
                       ? JSON2
                       : null;

        }

        #endregion


        #region Operator overloading

        #region Operator == (TariffRestriction1, TariffRestriction2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TariffRestriction1">A specification of charging tariff restrictions.</param>
        /// <param name="TariffRestriction2">Another specification of charging tariff restrictions.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (TariffRestrictions TariffRestriction1,
                                           TariffRestrictions TariffRestriction2)
        {

            if (Object.ReferenceEquals(TariffRestriction1, TariffRestriction2))
                return true;

            if (TariffRestriction1 is null || TariffRestriction2 is null)
                return false;

            return TariffRestriction1.Equals(TariffRestriction2);

        }

        #endregion

        #region Operator != (TariffRestriction1, TariffRestriction2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TariffRestriction1">A specification of charging tariff restrictions.</param>
        /// <param name="TariffRestriction2">Another specification of charging tariff restrictions.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (TariffRestrictions TariffRestriction1,
                                           TariffRestrictions TariffRestriction2)

            => !(TariffRestriction1 == TariffRestriction2);

        #endregion

        #endregion

        #region IEquatable<TariffRestriction> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        /// <returns>true|false</returns>
        public override Boolean Equals(Object Object)

            => Object is TariffRestrictions tariffRestrictions &&
                   Equals(tariffRestrictions);

        #endregion

        #region Equals(TariffRestriction)

        /// <summary>
        /// Compares two charging tariff restrictions for equality.
        /// </summary>
        /// <param name="TariffRestrictions">A charging tariff restriction to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(TariffRestrictions TariffRestrictions)

            => !(TariffRestrictions is null) &&

                ((!StartTime.  HasValue && !TariffRestrictions.StartTime.  HasValue) ||
                  (StartTime.  HasValue &&  TariffRestrictions.StartTime.  HasValue && StartTime.  Value.Equals(TariffRestrictions.StartTime.  Value))) &&

                ((!EndTime.    HasValue && !TariffRestrictions.EndTime.    HasValue) ||
                  (EndTime.    HasValue &&  TariffRestrictions.EndTime.    HasValue && EndTime.    Value.Equals(TariffRestrictions.EndTime.    Value))) &&

                (( StartDate.  HasValue &&  TariffRestrictions.StartDate.  HasValue) ||
                 ( StartDate.  HasValue &&  TariffRestrictions.StartDate.  HasValue && StartDate.  Value.Equals(TariffRestrictions.StartDate.  Value))) &&

                (( EndDate.    HasValue &&  TariffRestrictions.EndDate.    HasValue) ||
                 ( EndDate.    HasValue &&  TariffRestrictions.EndDate.    HasValue && EndDate.    Value.Equals(TariffRestrictions.EndDate.    Value))) &&

                ((!MinkWh.     HasValue && !TariffRestrictions.MinkWh.     HasValue) ||
                  (MinkWh.     HasValue &&  TariffRestrictions.MinkWh.     HasValue && MinkWh.     Value.Equals(TariffRestrictions.MinkWh.     Value))) &&

                ((!MaxkWh.     HasValue && !TariffRestrictions.MaxkWh.     HasValue) ||
                  (MaxkWh.     HasValue &&  TariffRestrictions.MaxkWh.     HasValue && MaxkWh.     Value.Equals(TariffRestrictions.MaxkWh.     Value))) &&

                ((!MinCurrent. HasValue && !TariffRestrictions.MinCurrent. HasValue) ||
                  (MinCurrent. HasValue &&  TariffRestrictions.MinCurrent. HasValue && MinCurrent. Value.Equals(TariffRestrictions.MinCurrent. Value))) &&

                ((!MaxCurrent. HasValue && !TariffRestrictions.MaxCurrent. HasValue) ||
                  (MaxCurrent. HasValue &&  TariffRestrictions.MaxCurrent. HasValue && MaxCurrent. Value.Equals(TariffRestrictions.MaxCurrent. Value))) &&

                ((!MinPower.   HasValue && !TariffRestrictions.MinPower.   HasValue) ||
                  (MinPower.   HasValue &&  TariffRestrictions.MinPower.   HasValue && MinPower.   Value.Equals(TariffRestrictions.MinPower.   Value))) &&

                ((!MaxPower.   HasValue && !TariffRestrictions.MaxPower.   HasValue) ||
                  (MaxPower.   HasValue &&  TariffRestrictions.MaxPower.   HasValue && MaxPower.   Value.Equals(TariffRestrictions.MaxPower.   Value))) &&

                ((!MinDuration.HasValue && !TariffRestrictions.MinDuration.HasValue) ||
                  (MinDuration.HasValue &&  TariffRestrictions.MinDuration.HasValue && MinDuration.Value.Equals(TariffRestrictions.MinDuration.Value))) &&

                ((!MaxDuration.HasValue && !TariffRestrictions.MaxDuration.HasValue) ||
                  (MaxDuration.HasValue &&  TariffRestrictions.MaxDuration.HasValue && MaxDuration.Value.Equals(TariffRestrictions.MaxDuration.Value))) &&

                DayOfWeek.Count().Equals(TariffRestrictions.DayOfWeek.Count()) &&
                DayOfWeek.All(day => TariffRestrictions.DayOfWeek.Contains(day)) &&

                ((!Reservation.HasValue && !TariffRestrictions.Reservation.HasValue) ||
                  (Reservation.HasValue &&  TariffRestrictions.Reservation.HasValue && Reservation.Value.Equals(TariffRestrictions.Reservation.Value)));

        #endregion

        #endregion

        #region GetHashCode()

        /// <summary>
        /// Get the hashcode of this object.
        /// </summary>
        public override Int32 GetHashCode()
        {
            unchecked
            {

                return (StartTime.HasValue
                            ? StartTime.  GetHashCode() * 43
                            : 0) ^

                       (EndTime.HasValue
                            ? EndTime.    GetHashCode() * 41
                            : 0) ^

                       (StartDate.HasValue
                            ? StartDate.  GetHashCode() * 37
                            : 0) ^

                       (EndDate.HasValue
                            ? EndDate.    GetHashCode() * 31
                            : 0) ^

                       (MinkWh.HasValue
                            ? MinkWh.     GetHashCode() * 29
                            : 0) ^

                       (MaxkWh.HasValue
                            ? MaxkWh.     GetHashCode() * 23
                            : 0) ^

                       (MinCurrent.HasValue
                            ? MinCurrent. GetHashCode() * 19
                            : 0) ^

                       (MaxCurrent.HasValue
                            ? MaxCurrent. GetHashCode() * 17
                            : 0) ^

                       (MinPower.HasValue
                            ? MinPower.   GetHashCode() * 13
                            : 0) ^

                       (MaxPower.HasValue
                            ? MaxPower.   GetHashCode() * 11
                            : 0) ^

                       (MinDuration.HasValue
                            ? MinDuration.GetHashCode() *  7
                            : 0) ^

                       (MaxDuration.HasValue
                            ? MaxDuration.GetHashCode() *  5
                            : 0) ^

                       (DayOfWeek.SafeAny()
                            ? DayOfWeek.Aggregate(0, (hashCode, day) => hashCode ^ day.GetHashCode()) * 3
                            : 0) ^

                       (Reservation.HasValue
                            ? Reservation.GetHashCode()
                            : 0);

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Get a string representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(StartTime.  HasValue
                                 ?            StartTime.  Value.ToString()
                                 : "",
                             EndTime.    HasValue
                                 ? " - "    + EndTime.    Value.ToString()
                                 : "",
                             StartDate.  HasValue
                                 ? " from " + StartDate.  Value.ToString()
                                 : "",
                             EndDate.    HasValue
                                 ? " to "   + EndDate.    Value.ToString()
                                 : "",

                             MinkWh.     HasValue
                                 ? "; > "   + MinkWh.     Value.ToString() + " kWh"
                                 : "",

                             MaxkWh.     HasValue
                                 ? "; < "   + MaxkWh.     Value.ToString() + " kWh"
                                 : "",

                             MinCurrent. HasValue
                                 ? "; > "   + MinCurrent. Value.ToString() + " Current"
                                 : "",

                             MaxCurrent. HasValue
                                 ? "; < "   + MaxCurrent. Value.ToString() + " Current"
                                 : "",

                             MinPower.   HasValue
                                 ? "; > "   + MinPower.   Value.ToString() + "kW"
                                 : "",

                             MaxPower.   HasValue
                                 ? "; < "   + MaxPower.   Value.ToString() + "kW"
                                 : "",

                             MinDuration.HasValue
                                 ? "; > "   + MinDuration.Value.TotalMinutes.ToString("0.00") + " min"
                                 : "",

                             MaxDuration.HasValue
                                 ? "; < "   + MaxDuration.Value.TotalMinutes.ToString("0.00") + " min"
                                 : "",

                             DayOfWeek.  SafeAny()
                                 ? "; "     + DayOfWeek.AggregateWith("|")
                                 : "");

        #endregion

    }

}
