﻿// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.HealthVault.Client;
using Microsoft.HealthVault.Clients;
using Microsoft.HealthVault.Configuration;
using Microsoft.HealthVault.ItemTypes;
using Microsoft.HealthVault.Person;
using Microsoft.HealthVault.Record;
using Microsoft.HealthVault.RestApi;
using Microsoft.HealthVault.RestApi.Generated;
using Microsoft.HealthVault.Thing;
using NodaTime;
using Xamarin.Forms;

namespace SandboxXamarinForms
{
    public partial class MainPage : ContentPage
    {
        private IHealthVaultSodaConnection _connection;
        private IThingClient _thingClient;

        public MainPage()
        {
            InitializeComponent();
        }

        private async void Connect_OnClicked(object sender, EventArgs e)
        {
            OutputLabel.Text = "Connecting...";

            var configuration = new HealthVaultConfiguration
            {
                MasterApplicationId = Guid.Parse("d6318dff-5352-4a10-a140-6c82c6536a3b")
            };
            _connection = HealthVaultConnectionFactory.Current.GetOrCreateSodaConnection(configuration);

            await _connection.AuthenticateAsync();

            _thingClient = _connection.CreateThingClient();
            ConnectedButtons.IsVisible = true;

            OutputLabel.Text = "Connected.";
        }

        private async void SetBP_OnClicked(object sender, EventArgs e)
        {
            // Create a new blood pressure object with random values
            Random rand = new Random();
            BloodPressure bp = new BloodPressure
            {
                Diastolic = rand.Next(20, 100),
                Systolic = rand.Next(80, 120)
            };

            // use our thing client to creat the new blood pressure
            PersonInfo personInfo = await _connection.GetPersonInfoAsync();
            await _thingClient.CreateNewThingsAsync(personInfo.SelectedRecord.Id, new List<BloodPressure> { bp });

            OutputLabel.Text = "Added blood pressure";
        }

        private async void Add100BPs_OnClicked(object sender, EventArgs e)
        {
            OutputLabel.Text = "Adding 100 blood pressures...";

            PersonInfo personInfo = await _connection.GetPersonInfoAsync();
            HealthRecordInfo recordInfo = personInfo.SelectedRecord;
            IThingClient thingClient = _connection.CreateThingClient();

            var random = new Random();

            var pressures = new List<BloodPressure>();
            for (int i = 0; i < 100; i++)
            {
                pressures.Add(new BloodPressure(
                    new HealthServiceDateTime(SystemClock.Instance.GetCurrentInstant().InZone(DateTimeZoneProviders.Tzdb.GetSystemDefault()).LocalDateTime),
                    random.Next(110, 130),
                    random.Next(70, 90)));
            }

            await thingClient.CreateNewThingsAsync(
                recordInfo.Id,
                pressures);

            OutputLabel.Text = "Done adding blood pressures.";
        }

        private async void GetBP_OnClicked(object sender, EventArgs e)
        {
            // use our thing client to get all things of type blood pressure
            PersonInfo personInfo = await _connection.GetPersonInfoAsync();
            IReadOnlyCollection<BloodPressure> bloodPressures = await _thingClient.GetThingsAsync<BloodPressure>(personInfo.SelectedRecord.Id);
            BloodPressure firstBloodPressure = bloodPressures.FirstOrDefault();
            if (firstBloodPressure == null)
            {
                OutputLabel.Text = "No blood pressures.";
            }
            else
            {
                OutputLabel.Text = firstBloodPressure.Systolic + "/" + firstBloodPressure.Diastolic + ", " + bloodPressures.Count + " total";
            }
        }

        private async void MultiQuery_OnClicked(object sender, EventArgs e)
        {
            PersonInfo personInfo = await _connection.GetPersonInfoAsync();
            var resultSet = await _thingClient.GetThingsAsync(
                personInfo.SelectedRecord.Id,
                new List<ThingQuery>
                {
                    new ThingQuery(BloodPressure.TypeId),
                    new ThingQuery(Weight.TypeId)
                });

            var resultList = resultSet.ToList();

            OutputLabel.Text = $"There are {resultList[0].Count} blood pressure(s) and {resultList[1].Count} weight(s).";
        }

        private async void GetItemTypeDef_OnClicked(object sender, EventArgs e)
        {
            IPlatformClient platformClient = _connection.CreatePlatformClient();
            await platformClient.GetHealthRecordItemTypeDefinitionAsync(new List<Guid> { BloodPressure.TypeId }, ThingTypeSections.All, new List<string>(), SystemClock.Instance.GetCurrentInstant());
        }

        private async void GetActionPlans_OnClicked(object sender, EventArgs e)
        {
            PersonInfo personInfo = await _connection.GetPersonInfoAsync();
            IMicrosoftHealthVaultRestApi restClient = _connection.CreateMicrosoftHealthVaultRestApi(personInfo.SelectedRecord.Id);

            var actionPlansResponse = await restClient.ActionPlans.GetAsync();

            OutputLabel.Text = $"There are {actionPlansResponse.Plans.Count} action plans";
        }

        private async void AuthorizeAdditionalRecords_OnClicked(object sender, EventArgs e)
        {
            await _connection.AuthorizeAdditionalRecordsAsync();
        }

        private async void Disconnect_OnClicked(object sender, EventArgs e)
        {
            await _connection.DeauthorizeApplicationAsync();
            OutputLabel.Text = "Deleted connection information.";

            ConnectedButtons.IsVisible = false;
        }
    }
}
