// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Globalization;
using System.Xml;
using System.Xml.XPath;
using Microsoft.HealthVault.Exceptions;
using Microsoft.HealthVault.Helpers;
using Microsoft.HealthVault.Thing;

namespace Microsoft.HealthVault.ItemTypes
{
    /// <summary>
    /// Percentage of oxygen saturation in the blood.
    /// </summary>
    ///
    public class BloodOxygenSaturation : ThingBase
    {
        /// <summary>
        /// Creates an instance of <see cref="BloodOxygenSaturation"/> with default values.
        /// </summary>
        ///
        public BloodOxygenSaturation()
            : base(TypeId)
        {
        }

        /// <summary>
        /// Creates an instance of <see cref="BloodOxygenSaturation"/> with specified parameters.
        /// </summary>
        ///
        /// <param name="when">The date and time the measurement was taken.</param>
        ///
        /// <param name="value">The percentage of oxygen saturation in the blood.</param>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="when"/> is <b>null</b>.
        /// </exception>
        ///
        /// <exception cref="ArgumentOutOfRangeException">
        /// If <paramref name="value"/> is less than 0.0 or greater than 1.0.
        /// </exception>
        ///
        public BloodOxygenSaturation(HealthServiceDateTime when, double value)
            : base(TypeId)
        {
            When = when;
            Value = value;
        }

        /// <summary>
        /// Retrieves the unique identifier for the blood oxygen saturation item type.
        /// </summary>
        public static new readonly Guid TypeId =
            new Guid("3a54f95f-03d8-4f62-815f-f691fc94a500");

        /// <summary>
        /// Populates this <see cref="BloodOxygenSaturation"/> instance from the data in the XML.
        /// </summary>
        ///
        /// <param name="typeSpecificXml">
        /// The XML to get the blood oxygen saturation data from.
        /// </param>
        ///
        /// <exception cref="InvalidOperationException">
        /// If the first node in <paramref name="typeSpecificXml"/> is not
        /// a "blood-oxygen-saturation" node.
        /// </exception>
        ///
        protected override void ParseXml(IXPathNavigable typeSpecificXml)
        {
            XPathNavigator itemNav =
                typeSpecificXml.CreateNavigator().SelectSingleNode("blood-oxygen-saturation");

            Validator.ThrowInvalidIfNull(itemNav, Resources.BloodOxygenSaturationUnexpectedNode);

            // when
            _when = new HealthServiceDateTime();
            _when.ParseXml(itemNav.SelectSingleNode("when"));

            // value
            _value = itemNav.SelectSingleNode("value").ValueAsDouble;

            // measurement-method
            _measurementMethod =
                XPathHelper.GetOptNavValue<CodableValue>(itemNav, "measurement-method");

            // measurement-flags
            _measurementFlags =
                XPathHelper.GetOptNavValue<CodableValue>(itemNav, "measurement-flags");
        }

        /// <summary>
        /// Writes the blood oxygen saturation data to the specified XmlWriter.
        /// </summary>
        ///
        /// <param name="writer">
        /// The XmlWriter to write the blood oxygen saturation data to.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="writer"/> is <b>null</b>.
        /// </exception>
        ///
        /// <exception cref="ThingSerializationException">
        /// If <see cref="When"/> is <b>null</b>.
        /// </exception>
        ///
        public override void WriteXml(XmlWriter writer)
        {
            Validator.ThrowIfWriterNull(writer);
            Validator.ThrowSerializationIfNull(_when, Resources.WhenNullValue);

            // <blood-oxygen-saturation>
            writer.WriteStartElement("blood-oxygen-saturation");

            // <when>
            _when.WriteXml("when", writer);

            // <value>
            writer.WriteElementString(
                "value",
                XmlConvert.ToString(_value));

            // <measurement-method>
            XmlWriterHelper.WriteOpt(
                writer,
                "measurement-method",
                _measurementMethod);

            // <measurement-flags>
            XmlWriterHelper.WriteOpt(
                writer,
                "measurement-flags",
                _measurementFlags);

            // </blood-oxygen-saturation>
            writer.WriteEndElement();
        }

        /// <summary>
        /// Gets or sets the date and time of this measurement.
        /// </summary>
        ///
        /// <value>
        /// A <see cref="HealthServiceDateTime"/> instance representing the date
        /// and time.
        /// </value>
        ///
        /// <exception cref="ArgumentNullException">
        /// If the <paramref name="value"/> parameter is <b>null</b>.
        /// </exception>
        ///
        public HealthServiceDateTime When
        {
            get { return _when; }

            set
            {
                Validator.ThrowIfArgumentNull(value, nameof(When), Resources.WhenNullValue);
                _when = value;
            }
        }

        private HealthServiceDateTime _when;

        /// <summary>
        /// Gets or sets the measured blood oxygen saturation percentage.
        /// </summary>
        ///
        /// <value>
        /// The measured blood oxygen saturation percentage.
        /// </value>
        ///
        /// <exception cref="ArgumentOutOfRangeException">
        /// If the <paramref name="value"/> parameter is less than 0.0 or greater than 1.0.
        /// </exception>
        ///
        public double Value
        {
            get { return _value; }

            set
            {
                if (value > 1.0 || value < 0.0)
                {
                    throw new ArgumentOutOfRangeException(nameof(Value), Resources.BloodOxygenSaturationValueOutOfRange);
                }

                _value = value;
            }
        }

        private double _value;

        /// <summary>
        /// Gets or sets the technique used to obtain the measurement.
        /// </summary>
        ///
        /// <value>
        /// The technique used to obtain the measurement.
        /// </value>
        ///
        /// <remarks>
        /// If the value is not known, it will be set to <b>null</b>. The preferred vocabulary
        /// for this value is "blood-oxygen-saturation-measurement-method".
        /// </remarks>
        ///
        public CodableValue MeasurementMethod
        {
            get { return _measurementMethod; }
            set { _measurementMethod = value; }
        }

        private CodableValue _measurementMethod;

        /// <summary>
        /// Gets or sets the additional information about the measurement.
        /// </summary>
        ///
        /// <value>
        /// The additional information about the measurement
        /// </value>
        ///
        /// <remarks>
        /// Examples: Incomplete measurement, irregular heartbeat. If the value is not known,
        /// it will be set to <b>null</b>. The preferred vocabulary for this value is
        /// "blood-oxygen-saturation-measurement-method".
        /// </remarks>
        ///
        public CodableValue MeasurementFlags
        {
            get { return _measurementFlags; }
            set { _measurementFlags = value; }
        }

        private CodableValue _measurementFlags;

        /// <summary>
        /// Gets the description of a blood oxygen instance.
        /// </summary>
        ///
        /// <returns>
        /// A string representation of the blood oxygen item.
        /// </returns>
        ///
        public override string ToString()
        {
            return
                string.Format(
                    Resources.Percent,
                    (_value * 100.0).ToString(CultureInfo.CurrentCulture));
        }
    }
}
