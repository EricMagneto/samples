using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using FlightFinder.Shared;
using Microsoft.AspNetCore.Blazor;

namespace FlightFinder.Client.Services
{
    public class AppState
    {
        // Actual state
        public IReadOnlyList<FlightItinerary> FlightSearchResults { get; private set; }
        public IReadOnlyList<RentalCarItinerary> RentalCarSearchResults { get; private set; }

        public bool SearchInProgress { get; private set; }

        private readonly List<Components.Shortlist.Item> shortlist = new List<Components.Shortlist.Item>();
        public IReadOnlyList<Components.Shortlist.Item> Shortlist => shortlist;

        // Lets components receive change notifications
        // Could have whatever granularity you want (more events, hierarchy...)
        public event Action OnChange;

        // Receive 'http' instance from DI
        private readonly HttpClient http;
        public AppState(HttpClient httpInstance)
        {
            http = httpInstance;
        }

        public async Task SearchFlights(FlightSearchCriteria criteria)
        {
            SearchInProgress = true;
            NotifyStateChanged();

            FlightSearchResults = await http.PostJsonAsync<FlightItinerary[]>("/api/flightsearch", criteria);
            SearchInProgress = false;
            NotifyStateChanged();
        }

        public async Task SearchRentalCars(RentalCarSearchCriteria criteria)
        {
            SearchInProgress = true;
            NotifyStateChanged();

            RentalCarSearchResults = await http.PostJsonAsync<RentalCarItinerary[]>("/api/rentalcarsearch", criteria);
            SearchInProgress = false;
            NotifyStateChanged();
        }

        public void AddToShortlist(FlightItinerary itinerary)
        {
            var item = new Components.Shortlist.Item();
            item.Content = (builder) =>
            {
                builder.OpenComponent<Components.ShortlistFlightItem>(0);
                builder.AddAttribute(1, "Itinerary", itinerary);
                builder.AddAttribute(2, "OnRemoveItinerary", () => RemoveFromShortlist(item));
                builder.CloseComponent();
            };
            shortlist.Add(item);
            NotifyStateChanged();
        }

        public void AddToShortlist(RentalCarItinerary itinerary)
        {
            var item = new Components.Shortlist.Item();
            item.Content = (builder) =>
            {
                builder.OpenComponent<Components.ShortlistCarRentalItem>(0);
                builder.AddAttribute(1, "Itinerary", itinerary);
                builder.AddAttribute(2, "OnRemoveItinerary", () => RemoveFromShortlist(item));
                builder.CloseComponent();
            };
            shortlist.Add(item);
            NotifyStateChanged();
        }

        public void RemoveFromShortlist(Components.Shortlist.Item item)
        {
            shortlist.Remove(item);
            NotifyStateChanged();
        }

        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}
