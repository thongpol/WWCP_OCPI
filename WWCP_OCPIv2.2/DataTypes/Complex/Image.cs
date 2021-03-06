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

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_2
{

    /// <summary>
    /// This class references images related to a EVSE in terms of a file name or uri.
    /// </summary>
    public class Image : IEquatable<Image>,
                         IComparable<Image>,
                         IComparable
    {

        #region Properties

        /// <summary>
        /// URL from where the image data can be fetched through a web browser (about 512 pixels).
        /// </summary>
        [Mandatory]
        public URL              URL          { get; }

        /// <summary>
        /// Image type like: gif, jpeg, png, svg.
        /// </summary>
        [Mandatory]
        public ImageFileType    Type         { get; }

        /// <summary>
        /// Describes what the image is used for.
        /// </summary>
        [Mandatory]
        public ImageCategories  Category     { get; }

        /// <summary>
        /// The optional width of the full scale image.
        /// </summary>
        [Optional]
        public UInt32?          Width        { get; }

        /// <summary>
        /// The optional height of the full scale image.
        /// </summary>
        [Optional]
        public UInt32?          Height       { get; }

        /// <summary>
        /// An optional URL from where a thumbnail of the image can be fetched through a webbrowser (about 128 pixels).
        /// </summary>
        [Optional]
        public URL?             Thumbnail    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new image.
        /// </summary>
        /// <param name="URL">URL from where the image data can be fetched through a web browser (about 512 pixels).</param>
        /// <param name="Type">Image type like: gif, jpeg, png, svg.</param>
        /// <param name="Category">Describes what the image is used for.</param>
        /// <param name="Width">The optional width of the full scale image.</param>
        /// <param name="Height">The optional height of the full scale image.</param>
        /// <param name="Thumbnail">An optional URL from where a thumbnail of the image can be fetched through a webbrowser (about 128 pixels).</param>
        public Image(URL              URL,
                     ImageFileType    Type,
                     ImageCategories  Category,

                     UInt32?          Width       = null,
                     UInt32?          Height      = null,
                     URL?             Thumbnail   = null)
        {

            if (URL.IsNullOrEmpty)
                throw new ArgumentNullException(nameof(URL), "The given URL of the image must not be null or empty!");

            this.URL        = URL;
            this.Type       = Type;
            this.Category   = Category;

            this.Width      = Width;
            this.Height     = Height;
            this.Thumbnail  = Thumbnail;

        }

        #endregion


        #region (static) Parse   (JSON, CustomImageParser = null)

        /// <summary>
        /// Parse the given JSON representation of an image.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CustomImageParser">A delegate to parse custom images JSON objects.</param>
        public static Image Parse(JObject                             JSON,
                                  CustomJObjectParserDelegate<Image>  CustomImageParser   = null)
        {

            if (TryParse(JSON,
                         out Image  additionalGeoLocation,
                         out String ErrorResponse,
                         CustomImageParser))
            {
                return additionalGeoLocation;
            }

            throw new ArgumentException("The given JSON representation of an image is invalid: " + ErrorResponse, nameof(JSON));

        }

        #endregion

        #region (static) Parse   (Text, CustomImageParser = null)

        /// <summary>
        /// Parse the given text representation of an image.
        /// </summary>
        /// <param name="Text">The text to parse.</param>
        /// <param name="CustomImageParser">A delegate to parse custom images JSON objects.</param>
        public static Image Parse(String                                        Text,
                                            CustomJObjectParserDelegate<Image>  CustomImageParser   = null)
        {

            if (TryParse(Text,
                         out Image  additionalGeoLocation,
                         out String ErrorResponse,
                         CustomImageParser))
            {
                return additionalGeoLocation;
            }

            throw new ArgumentException("The given text representation of an image is invalid: " + ErrorResponse, nameof(Text));

        }

        #endregion

        #region (static) TryParse(JSON, out Image, out ErrorResponse, CustomImageParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of an image.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="Image">The parsed connector.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject     JSON,
                                       out Image   Image,
                                       out String  ErrorResponse)

            => TryParse(JSON,
                        out Image,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of an image.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="Image">The parsed connector.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomImageParser">A delegate to parse custom images JSON objects.</param>
        public static Boolean TryParse(JObject                             JSON,
                                       out Image                           Image,
                                       out String                          ErrorResponse,
                                       CustomJObjectParserDelegate<Image>  CustomImageParser)
        {

            try
            {

                Image = default;

                if (JSON?.HasValues != true)
                {
                    ErrorResponse = "The given JSON object must not be null or empty!";
                    return false;
                }

                #region Parse URL         [mandatory]

                if (!JSON.ParseMandatory("url",
                                         "url",
                                         OCPIv2_2.URL.TryParse,
                                         out URL URL,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse Category    [mandatory]

                if (!JSON.ParseMandatoryEnum("category",
                                             "image category",
                                             out ImageCategories Category,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse Type        [mandatory]

                if (!JSON.ParseMandatory("type",
                                         "image type",
                                         ImageFileType.TryParse,
                                         out ImageFileType Type,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse Width       [optional]

                if (JSON.ParseOptional("width",
                                       "image width",
                                       out UInt32? Width,
                                       out ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                }

                #endregion

                #region Parse Height      [optional]

                if (JSON.ParseOptional("height",
                                       "image height",
                                       out UInt32? Height,
                                       out ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                }

                #endregion

                #region Parse Thumbnail   [optional]

                if (JSON.ParseOptional("thumbnail",
                                       "image thumbnail",
                                       URL.TryParse,
                                       out URL? Thumbnail,
                                       out ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                }

                #endregion


                Image = new Image(URL,
                                  Type,
                                  Category,
                                  Width,
                                  Height,
                                  Thumbnail);


                if (CustomImageParser != null)
                    Image = CustomImageParser(JSON,
                                              Image);

                return true;

            }
            catch (Exception e)
            {
                Image          = default;
                ErrorResponse  = "The given JSON representation of an image is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region (static) TryParse(Text, out Image, out ErrorResponse, CustomImageParser = null)

        /// <summary>
        /// Try to parse the given text representation of an image.
        /// </summary>
        /// <param name="Text">The text to parse.</param>
        /// <param name="Image">The parsed connector.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomImageParser">A delegate to parse custom images JSON objects.</param>
        public static Boolean TryParse(String                              Text,
                                       out Image                           Image,
                                       out String                          ErrorResponse,
                                       CustomJObjectParserDelegate<Image>  CustomImageParser)
        {

            try
            {

                return TryParse(JObject.Parse(Text),
                                out Image,
                                out ErrorResponse,
                                CustomImageParser);

            }
            catch (Exception e)
            {
                Image = default;
                ErrorResponse  = "The given text representation of an image is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomImageSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomImageSerializer">A delegate to serialize custom image JSON objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<Image> CustomImageSerializer = null)
        {

            var JSON = JSONObject.Create(

                           new JProperty("url",                 URL.            ToString()),

                           Thumbnail.HasValue
                               ? new JProperty("thumbnail",     Thumbnail.Value.ToString())
                               : null,

                           new JProperty("category",            Category.       ToString()),
                           new JProperty("type",                Type.           ToString()),

                           Width.HasValue
                               ? new JProperty("width",         Width. Value)
                               : null,

                           Height.HasValue
                               ? new JProperty("height",        Height.Value)
                               : null

                       );

            return CustomImageSerializer != null
                       ? CustomImageSerializer(this, JSON)
                       : JSON;

        }

        #endregion


        #region Operator overloading

        #region Operator == (Image1, Image2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Image1">An image.</param>
        /// <param name="Image2">Another image.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (Image Image1,
                                           Image Image2)
        {

            if (Object.ReferenceEquals(Image1, Image2))
                return true;

            if (Image1 is null || Image2 is null)
                return false;

            return Image1.Equals(Image2);

        }

        #endregion

        #region Operator != (Image1, Image2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Image1">An image.</param>
        /// <param name="Image2">Another image.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (Image Image1,
                                           Image Image2)

            => !(Image1 == Image2);

        #endregion

        #region Operator <  (Image1, Image2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Image1">A image.</param>
        /// <param name="Image2">Another image.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (Image Image1,
                                          Image Image2)

            => Image1 is null
                   ? throw new ArgumentNullException(nameof(Image1), "The given image must not be null!")
                   : Image1.CompareTo(Image2) < 0;

        #endregion

        #region Operator <= (Image1, Image2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Image1">A image.</param>
        /// <param name="Image2">Another image.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (Image Image1,
                                           Image Image2)

            => !(Image1 > Image2);

        #endregion

        #region Operator >  (Image1, Image2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Image1">A image.</param>
        /// <param name="Image2">Another image.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (Image Image1,
                                          Image Image2)

            => Image1 is null
                   ? throw new ArgumentNullException(nameof(Image1), "The given image must not be null!")
                   : Image1.CompareTo(Image2) > 0;

        #endregion

        #region Operator >= (Image1, Image2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Image1">A image.</param>
        /// <param name="Image2">Another image.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (Image Image1,
                                           Image Image2)

            => !(Image1 < Image2);

        #endregion

        #endregion

        #region IComparable<Image> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object Object)

            => Object is Image chargingPeriod
                   ? CompareTo(chargingPeriod)
                   : throw new ArgumentException("The given object is not an image!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(Image)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Image">An object to compare with.</param>
        public Int32 CompareTo(Image Image)
        {

            if (Image is null)
                throw new ArgumentNullException(nameof(Image),
                                                "The given image must not be null!");

            var c = URL.     CompareTo(Image.URL);

            if (c == 0)
                c = Type.    CompareTo(Image.Type);

            if (c == 0)
                c = Category.CompareTo(Image.Category);

            if (c == 0)
                c = Width. HasValue && Image.Width. HasValue
                        ? Width. Value.CompareTo(Image.Width. Value)
                        : 0;

            if (c == 0)
                c = Height.HasValue && Image.Height.HasValue
                        ? Height.Value.CompareTo(Image.Height.Value)
                        : 0;

            if (c == 0)
                c = !Thumbnail.HasValue && !Image.Thumbnail.HasValue
                        ? Thumbnail.Value.CompareTo(Image.Thumbnail.Value)
                        : 0;

            return c;

        }

        #endregion

        #endregion

        #region IEquatable<Image> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        /// <returns>true|false</returns>
        public override Boolean Equals(Object Object)

            => Object is Image Image &&
                   Equals(Image);

        #endregion

        #region Equals(Image)

        /// <summary>
        /// Compares two images for equality.
        /// </summary>
        /// <param name="Image">A image to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(Image Image)

            => !(Image is null) &&

                 URL.     Equals(Image.URL)      &&
                 Type.    Equals(Image.Type)     &&
                 Category.Equals(Image.Category) &&

               ((!Width. HasValue   && !Image.Width.    HasValue) ||
                 (Width. HasValue   &&  Image.Width.    HasValue && Width.    Value.Equals(Image.Width.    Value))) &&

               ((!Height.HasValue   && !Image.Height.   HasValue) ||
                 (Height.HasValue   &&  Image.Height.   HasValue && Height.   Value.Equals(Image.Height.   Value))) &&

              ((!Thumbnail.HasValue && !Image.Thumbnail.HasValue) ||
                (Thumbnail.HasValue &&  Image.Thumbnail.HasValue && Thumbnail.Value.Equals(Image.Thumbnail.Value)));

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

                return URL.     GetHashCode() * 13 ^
                       Type.    GetHashCode() * 11 ^
                       Category.GetHashCode() *  7 ^

                      (Width.    HasValue
                           ? Width. GetHashCode() * 5
                           : 0) ^

                      (Height.   HasValue
                           ? Height.GetHashCode() * 3
                           : 0) ^

                      (Thumbnail.HasValue
                           ? Thumbnail.GetHashCode()
                           : 0);

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(URL, "; ", Type, "; ", Category, "; ",

                             Width.    HasValue &&
                             Height.   HasValue
                                 ? "; " + Width + " x " + Height + " px"
                                 : "",

                             Thumbnail.HasValue
                                 ? "; " + Thumbnail
                                 : "");

        #endregion

    }

}
