// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using Microsoft.HealthVault.Clients;
using Microsoft.HealthVault.Exceptions;
using Microsoft.HealthVault.Helpers;
using Microsoft.HealthVault.Thing;

namespace Microsoft.HealthVault.ItemTypes
{
    /// <summary>
    /// Represents a thing type that encapsulates a healthcare proxy.
    /// </summary>
    ///
    public class HealthcareProxy : ThingBase
    {
        /// <summary>
        /// Creates a new instance of the <see cref="HealthcareProxy"/> class with default
        /// values.
        /// </summary>
        ///
        /// <remarks>
        /// The item isn't added to the health record until the <see cref="IThingClient.CreateNewThingsAsync{ThingBase}(Guid, ICollection{ThingBase})"/> method is called.
        /// </remarks>
        ///
        public HealthcareProxy()
            : base(TypeId)
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="HealthcareProxy"/> class
        /// with the specified date and time.
        /// </summary>
        ///
        /// <param name="when">
        /// The date/time for the healthcare proxy.
        /// </param>
        ///
        /// <exception cref="ArgumentException">
        /// The <paramref name="when"/> parameter is <b>null</b>.
        /// </exception>
        ///
        public HealthcareProxy(HealthServiceDateTime when)
            : base(TypeId)
        {
            When = when;
        }

        /// <summary>
        /// Retrieves the unique identifier for the item type.
        /// </summary>
        ///
        /// <value>
        /// A GUID.
        /// </value>
        ///
        public static new readonly Guid TypeId =
            new Guid("7EA47715-CBA4-47F0-99D2-EB0A9FB4A85C");

        /// <summary>
        /// Populates this healthcare proxy instance from the data in the XML.
        /// </summary>
        ///
        /// <param name="typeSpecificXml">
        /// The XML to get the healthcare proxy data from.
        /// </param>
        ///
        /// <exception cref="InvalidOperationException">
        /// The first node in <paramref name="typeSpecificXml"/> is not
        /// a healthcare proxy node.
        /// </exception>
        ///
        protected override void ParseXml(IXPathNavigable typeSpecificXml)
        {
            XPathNavigator itemNav =
                typeSpecificXml.CreateNavigator().SelectSingleNode("healthcare-proxy");

            Validator.ThrowInvalidIfNull(itemNav, Resources.HealthcareProxyUnexpectedNode);

            _when = new HealthServiceDateTime();
            _when.ParseXml(itemNav.SelectSingleNode("when"));

            // <proxy>
            _proxy =
                XPathHelper.GetOptNavValue<PersonItem>(
                    itemNav,
                    "proxy");

            // <alternate>
            _alternate =
                XPathHelper.GetOptNavValue<PersonItem>(
                    itemNav,
                    "alternate");

            // <primary-witness>
            _primaryWitness =
                XPathHelper.GetOptNavValue<PersonItem>(
                    itemNav,
                    "primary-witness");

            // <secondary-witness>
            _secondaryWitness =
                XPathHelper.GetOptNavValue<PersonItem>(
                    itemNav,
                    "secondary-witness");

            // <content>
            _content =
                XPathHelper.GetOptNavValue(itemNav, "content");
        }

        /// <summary>
        /// Writes the healthcare proxy data to the specified XmlWriter.
        /// </summary>
        ///
        /// <param name="writer">
        /// The XmlWriter to write the healthcare proxy data to.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="writer"/> is <b>null</b>.
        /// </exception>
        ///
        /// <exception cref="ThingSerializationException">
        /// The <see cref="When"/> property has not been set.
        /// </exception>
        ///
        public override void WriteXml(XmlWriter writer)
        {
            Validator.ThrowIfWriterNull(writer);
            Validator.ThrowSerializationIfNull(_when, Resources.HealthcareProxyWhenNotSet);

            // <healthcare-proxy>
            writer.WriteStartElement("healthcare-proxy");

            // <when>
            _when.WriteXml("when", writer);

            // <proxy>
            XmlWriterHelper.WriteOpt(
                writer,
                "proxy",
                _proxy);

            // <alternate>
            XmlWriterHelper.WriteOpt(
                writer,
                "alternate",
                _alternate);

            // <primary-witness>
            XmlWriterHelper.WriteOpt(
                writer,
                "primary-witness",
                _primaryWitness);

            // <secondary-witness>
            XmlWriterHelper.WriteOpt(
                writer,
                "secondary-witness",
                _secondaryWitness);

            // <content>
            XmlWriterHelper.WriteOptString(
                writer,
                "content",
                _content);

            // </healthcare-proxy>
            writer.WriteEndElement();
        }

        /// <summary>
        /// Gets or sets the date/time for the medical equipment.
        /// </summary>
        ///
        /// <value>
        /// A <see cref="HealthServiceDateTime"/> instance representing
        /// the date. The default value is the current year, month, and day.
        /// </value>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="value"/> parameter is <b>null</b>.
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

        private HealthServiceDateTime _when = new HealthServiceDateTime();

        /// <summary>
        /// Gets or sets the proxy contact information.
        /// </summary>
        ///
        /// <value>
        /// A <see cref="PersonItem"/> representing the information.
        /// </value>
        ///
        /// <remarks>
        /// Set the value to <b>null</b> if the proxy contact information
        /// should not be stored.
        /// </remarks>
        ///
        public PersonItem Proxy
        {
            get { return _proxy; }
            set { _proxy = value; }
        }

        private PersonItem _proxy;

        /// <summary>
        /// Gets or sets the alternate contact information.
        /// </summary>
        ///
        /// <value>
        /// A <see cref="PersonItem"/> representing the information.
        /// </value>
        ///
        /// <remarks>
        /// Set the value to <b>null</b> if the alternate contact information
        /// should not be stored.
        /// </remarks>
        ///
        public PersonItem Alternate
        {
            get { return _alternate; }
            set { _alternate = value; }
        }

        private PersonItem _alternate;

        /// <summary>
        /// Gets or sets the primary witness contact information.
        /// </summary>
        ///
        /// <value>
        /// A <see cref="PersonItem"/> representing the information.
        /// </value>
        ///
        /// <remarks>
        /// Set the value to <b>null</b> if the primary witness contact information
        /// should not be stored.
        /// </remarks>
        ///
        public PersonItem PrimaryWitness
        {
            get { return _primaryWitness; }
            set { _primaryWitness = value; }
        }

        private PersonItem _primaryWitness;

        /// <summary>
        /// Gets or sets the secondary witness contact information.
        /// </summary>
        ///
        /// <value>
        /// A <see cref="PersonItem"/> representing the information.
        /// </value>
        ///
        /// <remarks>
        /// Set the value to <b>null</b> if the secondary witness contact information
        /// should not be stored.
        /// </remarks>
        ///
        public PersonItem SecondaryWitness
        {
            get { return _secondaryWitness; }
            set { _secondaryWitness = value; }
        }

        private PersonItem _secondaryWitness;

        /// <summary>
        /// Gets or sets the content of what is being proxied.
        /// </summary>
        ///
        /// <value>
        /// A string representing the content.
        /// </value>
        ///
        /// <remarks>
        /// Set the value to <b>null</b> if the content should not be
        /// stored.
        /// </remarks>
        ///
        /// <exception cref="ArgumentException">
        /// If <paramref name="value"/> contains only whitespace.
        /// </exception>
        ///
        public string Content
        {
            get { return _content; }

            set
            {
                Validator.ThrowIfStringIsWhitespace(value, "Content");
                _content = value;
            }
        }

        private string _content;

        /// <summary>
        /// Gets a string representation of the healthcare proxy item.
        /// </summary>
        ///
        /// <returns>
        /// A string representation of the healthcare proxy item.
        /// </returns>
        ///
        public override string ToString()
        {
            StringBuilder result = new StringBuilder(200);

            result.Append(When);

            if (Proxy != null)
            {
                result.AppendFormat(
                    Resources.ListFormat,
                    Proxy.ToString());
            }

            if (Content != null)
            {
                result.AppendFormat(
                    Resources.ListFormat,
                    Content);
            }

            return result.ToString();
        }
    }
}
