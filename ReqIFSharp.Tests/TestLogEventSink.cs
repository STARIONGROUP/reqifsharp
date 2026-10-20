// -------------------------------------------------------------------------------------------------
// <copyright file="TestLogEventSink.cs" company="Starion Group S.A.">
//
//   SPDX-FileCopyrightText: 2017-2026 Starion Group S.A.
//   SPDX-License-Identifier: Apache-2.0
//
// </copyright>
// ------------------------------------------------------------------------------------------------

namespace ReqIFSharp.Tests
{
    using System.Collections.Generic;

    using Serilog.Core;
    using Serilog.Events;

    /// <summary>
    /// An event log sink to test with that acts as a destination for log events.
    /// </summary>
    public class TestLogEventSink : ILogEventSink
    {
        public List<LogEvent> Events { get; } = new List<LogEvent>();

        /// <summary>
        /// Emit the provided log event to the sink.
        /// </summary>
        /// <param name="logEvent">The log event to write.</param>
        public void Emit(LogEvent logEvent)
        {
            Events.Add(logEvent);
        }
    }
}