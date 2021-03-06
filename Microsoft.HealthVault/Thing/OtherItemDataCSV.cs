// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Microsoft.HealthVault.Helpers;

namespace Microsoft.HealthVault.Thing
{
    /// <summary>
    /// The OtherItemDataCSV class is used to store and retrieve data stored in the HealthVault
    /// comma-separated format.
    /// </summary>
    ///
    /// <remarks>
    /// It is not used directly but through a derived class that is specific to a thing type -
    /// for example, the ExerciseSamplesData class.
    /// </remarks>
    public abstract class OtherItemDataCsv : OtherItemData
    {
        /// <summary>
        /// Creates a new instance of the <see cref="OtherItemDataCsv"/> class
        /// </summary>
        protected OtherItemDataCsv()
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="OtherItemDataCsv"/> class
        /// with the specified data, encoding, and content type.
        /// </summary>
        ///
        /// <param name="data">
        /// The data to store in the other data section of the health record
        /// item.
        /// </param>
        ///
        /// <param name="contentEncoding">
        /// The type of encoding that was done on the data. Usually this will
        /// be "base64" but other encodings are acceptable.
        /// </param>
        ///
        /// <param name="contentType">
        /// The MIME-content type of the data.
        /// </param>
        ///
        protected OtherItemDataCsv(
            string data,
            string contentEncoding,
            string contentType)
            : base(data, contentEncoding, contentType)
        {
        }

        /// <summary>
        /// Walk the other data string, keeping track of escapes, and break it at any point where
        /// there is an unescaped specified character.
        /// </summary>
        /// <param name="dataString">The string to search.</param>
        /// <param name="characterToBreakAt">The character to break apart at.</param>
        /// <returns>the list of strings that have been broken apart</returns>
        private static List<string> BreakStringAtCharacter(string dataString, char characterToBreakAt)
        {
            List<string> items = new List<string>();

            bool inEscape = false;
            int startLocation = 0;

            // simple FSM.
            // Walk the characters in the string, keeping track of \ escape characters. Break characters
            // are only significant if we're not in an escape...
            for (int i = 0; i < dataString.Length; i++)
            {
                if (!inEscape)
                {
                    // if it's a comma, break and save the string to the array...
                    if (dataString[i] == characterToBreakAt)
                    {
                        items.Add(dataString.Substring(startLocation, i - startLocation));
                        startLocation = i + 1;
                    }
                    else if (dataString[i] == '\\')
                    {
                        inEscape = true;
                    }
                }
                else
                {
                    // escapes only one character long, so we just flip back out again...
                    inEscape = false;
                }
            }

            // Add in last item...
            items.Add(dataString.Substring(startLocation, dataString.Length - startLocation));

            return items;
        }

        /// <summary>
        /// Parse the comma-separated representation into an array of strings.
        /// </summary>
        /// <remarks>
        /// When this method returns, the Escapes collection will contain any
        /// escapes encountered during the parsing.
        /// </remarks>
        /// <returns>A collection of the strings.</returns>
        /// <exception cref="ArgumentException">
        /// If the content type is not "text/csv".
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// If the Data section is null.
        /// </exception>
        protected Collection<OtherItemDataCsvItem> GetAsString()
        {
            if (ContentType != "text/csv")
            {
                throw new ArgumentException(Resources.OtherItemDataFormat, nameof(ContentType));
            }

            Validator.ThrowIfArgumentNull(Data, nameof(Data), Resources.OtherItemDataNull);

            Collection<OtherItemDataCsvItem> values = new Collection<OtherItemDataCsvItem>();

            List<string> stringValues = BreakStringAtCharacter(Data, ',');

            for (int i = 0; i < stringValues.Count; i++)
            {
                // Get current value, remove any comma escapes (no longer needed)...
                string current = stringValues[i].Replace(@"\,", ",");

                // See if this is a name=value escape...
                List<string> escapeParts = BreakStringAtCharacter(current, '=');

                for (int parts = 0; parts < escapeParts.Count; parts++)
                {
                    escapeParts[0] = escapeParts[0].Replace(@"\=", "=");
                    escapeParts[0] = escapeParts[0].Replace(@"\\", @"\");
                }

                if (escapeParts.Count >= 2)
                {
                    OtherItemDataCsvEscape escapeItem = new OtherItemDataCsvEscape(escapeParts[0], escapeParts[1]);

                    values.Add(escapeItem);
                }
                else
                {
                    values.Add(new OtherItemDataCsvString(escapeParts[0]));
                }
            }

            return values;
        }

        /// <summary>
        /// Parses the comma-delimited data into a series of double values.
        /// </summary>
        /// <remarks>
        /// The collection contains two kinds of values.
        /// OtherItemDataCsvDouble items contain double values
        /// OtherItemDataCsvEscape items contain escapes
        /// </remarks>
        /// <returns>A collection of OtherItemDataCsvItem</returns>
        protected Collection<OtherItemDataCsvItem> GetAsDouble()
        {
            Collection<OtherItemDataCsvItem> stringValues = GetAsString();
            Collection<OtherItemDataCsvItem> values = new Collection<OtherItemDataCsvItem>();

            foreach (OtherItemDataCsvItem item in stringValues)
            {
                OtherItemDataCsvString itemString = item as OtherItemDataCsvString;
                if (itemString != null)
                {
                    double value;
                    try
                    {
                        value = double.Parse(itemString.Value);
                    }
                    catch (FormatException)
                    {
                        throw new InvalidOperationException(Resources.OtherItemDataInvalidNumber);
                    }

                    values.Add(new OtherItemDataCsvDouble(value));
                }
                else
                {
                    values.Add(item);
                }
            }

            return values;
        }

        /// <summary>
        /// Create the comma-delimited representation of a set of data and escapes.
        /// </summary>
        /// <remarks>
        /// The escapes are inserted into the comma-delimited list in the appropriate places.
        /// Any escape that occurs after the last element is ignored.
        /// </remarks>
        /// <param name="values">The collection of values to store.</param>
        /// <exception cref="ArgumentNullException">
        /// If the parameter <paramref name="values"/> is null.
        /// </exception>
        protected void SetOtherData(IList<OtherItemDataCsvItem> values)
        {
            Validator.ThrowIfArgumentNull(values, nameof(values), Resources.OtherItemDataValuesNull);

            StringBuilder builder = new StringBuilder();

            int currentItemIndex = 0;
            foreach (OtherItemDataCsvItem item in values)
            {
                if (currentItemIndex != 0)
                {
                    builder.Append(",");
                }

                OtherItemDataCsvEscape itemEscape = item as OtherItemDataCsvEscape;
                if (itemEscape != null)
                {
                    string name = itemEscape.Name.Replace("=", @"\=");
                    string value = itemEscape.Value.Replace("=", @"\=");
                    builder.Append(name);
                    builder.Append("=");
                    builder.Append(value);
                }

                OtherItemDataCsvDouble itemDouble = item as OtherItemDataCsvDouble;
                if (itemDouble != null)
                {
                    builder.Append(itemDouble.Value.ToString());
                }

                OtherItemDataCsvString itemString = item as OtherItemDataCsvString;
                if (itemString != null)
                {
                    string value = itemString.Value.Replace("=", @"\=");
                    value = itemString.Value.Replace(",", @"\,");
                    builder.Append(value);
                }

                currentItemIndex++;
            }

            Data = builder.ToString();
            ContentType = "text/csv";
            ContentEncoding = string.Empty;
        }
    }
}
