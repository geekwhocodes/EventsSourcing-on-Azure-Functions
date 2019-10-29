﻿using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventSourcingOnAzureFunctions.Common
{
    /// <summary>
    /// Configuration options for the event sourcing on azure extension/library.
    /// </summary>
    public class EventSourcingOnAzureOptions
    {


        public const int MAX_RETRIES = 1000;
        public const int MIN_WAIT = 100;

        /// <summary>
        /// Should this function app raise notifications when a new entity instance
        /// is created
        /// </summary>
        public bool RaiseEntityCreationNotification { get; set; } = false;


        /// <summary>
        /// Should this function app raise notifications when an event is persisted
        /// to an event stream
        /// </summary>
        public bool RaiseEventNotification { get; set; } = false;

        /// <summary>
        /// The name of the eventgrid hug to send notifications via
        /// </summary>
        public string EventGridHubName { get; set; }

        /// <summary>
        /// The SAS key to use when communicating with event grid
        /// </summary>
        public string EventGridKeyValue { get; set; }

        /// <summary>
        /// The event grid topic endpoint used when communication notifications via event grid
        /// </summary>
        public string EventGridTopicEndpoint { get; set; }

        /// <summary>
        /// The name of the app setting containing the key used for authenticating with the Azure Event Grid custom topic at <see cref="EventGridTopicEndpoint"/>.
        /// </summary>
        public string EventGridKeySettingName { get; set; }


        /// <summary>
        /// Gets or sets the Event Grid publish request retry count.
        /// </summary>
        /// <value>The number of retry attempts.
        /// The default is 10
        /// </value>
        public int EventGridPublishRetryCount { get; set; } = 10;

        /// <summary>
        /// Gets orsets the Event Grid publish request retry interval.
        /// </summary>
        /// <value>A <see cref="TimeSpan"/> representing the retry interval. 
        /// The default value is 5 minutes.</value>
        public TimeSpan EventGridPublishRetryInterval { get; set; } = TimeSpan.FromMinutes(5);


        /// <summary>
        /// Empty constructor for serialisation
        /// </summary>
        public EventSourcingOnAzureOptions()
        {
        }

        

    }

    public static class EventSourcingOnAzureOptionsConfigExtensions
    {
        public const string DefaultConfigKey = "EventSourcingOnAzure";

        public static EventSourcingOnAzureOptions GetEventSourcingOnAzureOptionsConfig(this IConfiguration configuration)
        {

            EventSourcingOnAzureOptions ret = null;

            if (configuration.GetSection(DefaultConfigKey) != null)
            {
               ret  = configuration.GetSection(DefaultConfigKey).Get<EventSourcingOnAzureOptions>();
                if (null != ret)
                {
                    if (string.IsNullOrWhiteSpace(ret.EventGridKeyValue))
                    {
                        if (!string.IsNullOrWhiteSpace(ret.EventGridKeySettingName))
                        {
                            ret.EventGridKeyValue = Environment.GetEnvironmentVariable(ret.EventGridKeySettingName);
                        }
                    }
                    return ret;
                }

            }


            // Get an options class from the environment varialbles
            ret = new EventSourcingOnAzureOptions();

            ret.EventGridHubName = Environment.GetEnvironmentVariable(nameof(EventSourcingOnAzureOptions.EventGridHubName));
            ret.EventGridKeySettingName = Environment.GetEnvironmentVariable(nameof(EventSourcingOnAzureOptions.EventGridKeySettingName));
            ret.EventGridKeyValue = Environment.GetEnvironmentVariable(nameof(EventSourcingOnAzureOptions.EventGridKeyValue));
            ret.EventGridTopicEndpoint = Environment.GetEnvironmentVariable(nameof(EventSourcingOnAzureOptions.EventGridTopicEndpoint));


            string envEventGridPublishRetryCount = Environment.GetEnvironmentVariable(nameof(EventSourcingOnAzureOptions.EventGridPublishRetryCount));
            if (!string.IsNullOrWhiteSpace(envEventGridPublishRetryCount))
            {
                int envRetry;
                if (int.TryParse(envEventGridPublishRetryCount, out envRetry))
                {
                    if ((envRetry > 0) && (envRetry <= EventSourcingOnAzureOptions.MAX_RETRIES))
                    {
                        ret.EventGridPublishRetryCount = envRetry;
                    }
                }
            }

            // 
            string envEventGridPublishRetryInterval = Environment.GetEnvironmentVariable(nameof(EventSourcingOnAzureOptions.EventGridPublishRetryInterval));
            if (!string.IsNullOrWhiteSpace(envEventGridPublishRetryInterval))
            {
                TimeSpan tsenvEventGridPublishRetryInterval;
                if (TimeSpan.TryParse(envEventGridPublishRetryInterval, out tsenvEventGridPublishRetryInterval))
                {
                    if (tsenvEventGridPublishRetryInterval.TotalMilliseconds >= EventSourcingOnAzureOptions.MIN_WAIT)
                    {
                        ret.EventGridPublishRetryInterval = tsenvEventGridPublishRetryInterval;
                    }
                }
            }

            // boolean flags
            string envRaiseEntityCreationNotification = Environment.GetEnvironmentVariable(nameof(EventSourcingOnAzureOptions.RaiseEntityCreationNotification));
            if (!string.IsNullOrWhiteSpace(envRaiseEntityCreationNotification))
            {
                bool raiseEntityCreate = false;
                if (bool.TryParse(envRaiseEntityCreationNotification, out raiseEntityCreate))
                {
                    ret.RaiseEntityCreationNotification = raiseEntityCreate;
                }
            }

            string envRaiseEventNotification = Environment.GetEnvironmentVariable(nameof(EventSourcingOnAzureOptions.RaiseEventNotification));
            if (!string.IsNullOrWhiteSpace(envRaiseEventNotification))
            {
                bool raiseEvent = false;
                if (bool.TryParse(envRaiseEventNotification, out raiseEvent))
                {
                    ret.RaiseEventNotification = raiseEvent;
                }
            }

            return ret;
        }
    }
}