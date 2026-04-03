// Copyright (C) 2022  Kevin Jilissen

// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.

// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.

// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.

using System.Collections.Generic;
using Newtonsoft.Json;

namespace Jellyfin.Xtream.Client.Models
{
    /// <summary>
    /// Represents EPG (Electronic Program Guide) information returned by the Xtream Codes API.
    /// </summary>
    public class EpgInfo
    {
        /// <summary>
        /// Gets or sets the list of EPG listings.
        /// </summary>
        [JsonProperty("epg_listings")]
        public List<EpgListing> EpgListings { get; set; } = new List<EpgListing>();
    }

    /// <summary>
    /// Represents a single EPG listing entry.
    /// </summary>
    public class EpgListing
    {
        /// <summary>
        /// Gets or sets the unique identifier of the EPG listing.
        /// Stored as string to handle large numeric IDs returned as quoted strings by some Xtream Codes servers.
        /// </summary>
        [JsonProperty("id")]
        [JsonConverter(typeof(FlexibleLongConverter))]
        public long Id { get; set; }

        /// <summary>
        /// Gets or sets the EPG ID reference.
        /// </summary>
        [JsonProperty("epg_id")]
        [JsonConverter(typeof(FlexibleLongConverter))]
        public long EpgId { get; set; }

        /// <summary>
        /// Gets or sets the title of the program.
        /// </summary>
        [JsonProperty("title")]
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the language of the program.
        /// </summary>
        [JsonProperty("lang")]
        public string Lang { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the start time of the program as a Unix timestamp string.
        /// </summary>
        [JsonProperty("start")]
        public string Start { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the end time of the program as a Unix timestamp string.
        /// </summary>
        [JsonProperty("end")]
        public string End { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the description of the program (Base64 encoded).
        /// </summary>
        [JsonProperty("description")]
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the channel ID associated with this listing.
        /// </summary>
        [JsonProperty("channel_id")]
        public string ChannelId { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the start timestamp.
        /// </summary>
        [JsonProperty("start_timestamp")]
        [JsonConverter(typeof(FlexibleLongConverter))]
        public long StartTimestamp { get; set; }

        /// <summary>
        /// Gets or sets the stop timestamp.
        /// </summary>
        [JsonProperty("stop_timestamp")]
        [JsonConverter(typeof(FlexibleLongConverter))]
        public long StopTimestamp { get; set; }

        /// <summary>
        /// Gets or sets whether the program is currently playing (0 or 1).
        /// </summary>
        [JsonProperty("now_playing")]
        [JsonConverter(typeof(FlexibleIntConverter))]
        public int NowPlaying { get; set; }

        /// <summary>
        /// Gets or sets whether the program has an archive available (0 or 1).
        /// </summary>
        [JsonProperty("has_archive")]
        [JsonConverter(typeof(FlexibleIntConverter))]
        public int HasArchive { get; set; }
    }

    /// <summary>
    /// A flexible JSON converter that reads a <see cref="long"/> value from either
    /// a JSON number or a quoted string. Handles values that exceed <see cref="long.MaxValue"/>
    /// by applying a safe modulo clamp.
    /// </summary>
    public class FlexibleLongConverter : JsonConverter<long>
    {
        /// <inheritdoc/>
        public override long ReadJson(
            JsonReader reader,
            System.Type objectType,
            long existingValue,
            bool hasExistingValue,
            JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return 0L;
            }

            if (reader.TokenType == JsonToken.String)
            {
                string? raw = reader.Value as string;
                if (string.IsNullOrWhiteSpace(raw))
                {
                    return 0L;
                }

                if (long.TryParse(raw, out long parsedLong))
                {
                    return parsedLong;
                }

                // Handle values that exceed long.MaxValue by parsing as ulong and casting
                if (ulong.TryParse(raw, out ulong parsedUlong))
                {
                    return (long)parsedUlong;
                }

                return 0L;
            }

            if (reader.TokenType == JsonToken.Integer)
            {
                try
                {
                    return Convert.ToInt64(reader.Value);
                }
                catch
                {
                    return 0L;
                }
            }

            return 0L;
        }

        /// <inheritdoc/>
        public override void WriteJson(JsonWriter writer, long value, JsonSerializer serializer)
        {
            writer.WriteValue(value);
        }
    }

    /// <summary>
    /// A flexible JSON converter that reads an <see cref="int"/> value from either
    /// a JSON number or a quoted string.
    /// </summary>
    public class FlexibleIntConverter : JsonConverter<int>
    {
        /// <inheritdoc/>
        public override int ReadJson(
            JsonReader reader,
            System.Type objectType,
            int existingValue,
            bool hasExistingValue,
            JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return 0;
            }

            if (reader.TokenType == JsonToken.String)
            {
                string? raw = reader.Value as string;
                if (string.IsNullOrWhiteSpace(raw))
                {
                    return 0;
                }

                return int.TryParse(raw, out int parsed) ? parsed : 0;
            }

            if (reader.TokenType == JsonToken.Integer)
            {
                try
                {
                    return Convert.ToInt32(reader.Value);
                }
                catch
                {
                    return 0;
                }
            }

            return 0;
        }

        /// <inheritdoc/>
        public override void WriteJson(JsonWriter writer, int value, JsonSerializer serializer)
        {
            writer.WriteValue(value);
        }
    }
}
