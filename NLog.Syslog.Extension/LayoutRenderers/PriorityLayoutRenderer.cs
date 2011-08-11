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
    using System.Text;
    using NLog.Config;
    using NLog.Syslog.Extension;

    /// <summary>
    /// Render the syslog priority base on the event level
    /// </summary>
    [LayoutRenderer("priority")]
    public class PriorityLayoutRenderer : LayoutRenderer
    {
       
        /// <summary>
        /// Renders the syslog priority based on the log event level <see cref="StringBuilder" />.
        /// </summary>
        /// <param name="builder">The <see cref="StringBuilder"/> to append the rendered data to.</param>
        /// <param name="logEvent">Logging event.</param>
        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            SyslogLevels syslogLevel;

            // Always use facility Local7
            int Facility = 23;
            
            if (logEvent.Level == LogLevel.Debug)
            {
                syslogLevel = SyslogLevels.Debug;
            }
            else if (logEvent.Level == LogLevel.Error)
            {
                syslogLevel = SyslogLevels.Error;
            }
            else if (logEvent.Level == LogLevel.Fatal)
            {
                syslogLevel = SyslogLevels.Critical;
            }
            else if (logEvent.Level == LogLevel.Info)
            {
                syslogLevel = SyslogLevels.Information;
            }
            else if (logEvent.Level == LogLevel.Trace)
            {
                syslogLevel = SyslogLevels.Debug;
            }
            else if (logEvent.Level == LogLevel.Warn)
            {
                syslogLevel = SyslogLevels.Warning;
            }
            else
            {
                syslogLevel = SyslogLevels.Information;
            }
           

            int Priority = (Facility*8) + (int) syslogLevel;

            builder.Append(Priority.ToString());
                
        }
    }
}