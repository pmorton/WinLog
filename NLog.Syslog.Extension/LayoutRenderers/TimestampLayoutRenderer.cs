//   Copyright 2011 Paul Morton <paul.e.morton@gmail.com>
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0//
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

namespace NLog.LayoutRenderers
{
    using System;
    using System.ComponentModel;
    using System.Text;
    using NLog.Config;

    
    /// <summary>
    /// Render the syslog timestamp based on local time
    /// </summary>
    [LayoutRenderer("timestamp")]
    public class TimestampLayoutRenderer : LayoutRenderer
    {

        /// <summary>
        /// Gets or sets a value controlling the timestamp generation timezone
        /// </summary>
        /// <docgen category='Rendering Options' order='10' />
        [DefaultValue(false)]
        public bool UniversalTime { get; set; }

        /// <summary>
        /// Render the syslog timestamp based on local time <see cref="StringBuilder" />.
        /// </summary>
        /// <param name="builder">The <see cref="StringBuilder"/> to append the rendered data to.</param>
        /// <param name="logEvent">Logging event.</param>
        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            string timestamp;
            if (this.UniversalTime)
            {
                timestamp = logEvent.TimeStamp.ToUniversalTime().ToString("MMM dd HH:mm:ss");
            }
            else
            {
                timestamp = logEvent.TimeStamp.ToString("MMM dd HH:mm:ss");
            }
            builder.Append(timestamp);
        }
    }
}