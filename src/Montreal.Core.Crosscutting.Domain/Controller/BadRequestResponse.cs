﻿using System.Collections.Generic;

namespace Montreal.Core.Crosscutting.Domain.Controller
{
    public class BadRequestResponse
    {
        public bool Success { get { return false; } }

        public IEnumerable<string> Errors { get; }

        public BadRequestResponse(IEnumerable<string> errors)
        {
            Errors = errors;
        }
    }
}