﻿using IdentityModel;
using Montreal.Core.Crosscutting.Domain.Bus;
using Montreal.Core.Crosscutting.Domain.Notifications;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using Montreal.Core.Crosscutting.Common.Data;

namespace Montreal.Core.Crosscutting.Domain.Controller
{
    [Route("v{version:apiVersion}/[Controller]")]
    [ApiController]
    public abstract class ApiController : ControllerBase
    {
        private readonly DomainNotificationHandler _notifications;
        protected IMediatorHandler _mediator { get; }

        /// <summary>
        /// The <see cref="Common.Data.PaginationObject"/> containing <see cref="Page"/>, <see cref="Order"/> and <see cref="Filter"/> 
        /// extracted of request.
        /// </summary>
        /// <remarks>
        /// It is instantiated automatically, taking keywords from the request to assemble the object. 
        /// The keywords recognized in request is "page", "size", "orderProperties" and anything else is filter.
        /// </remarks>
        public PaginationObject PaginationObject { get; set;  }

        protected ApiController(IMediatorHandler mediator)
        {
            _notifications = (DomainNotificationHandler)mediator.GetNotificationHandler();
            _mediator = mediator;
        }

        protected IEnumerable<DomainNotification> Notifications => _notifications.GetNotifications();

        protected bool IsValidOperation()
        {
            return (!_notifications.HasNotifications());
        }

        protected new IActionResult Response(object result = null)
        {
            if (IsValidOperation())
                return Ok(new SuccessResponse<object>(result));

            return BadRequest(new BadRequestResponse(
                _notifications.GetNotifications().Select(n => n.Value)
            ));
        }

        protected IActionResult ModalStateResponse()
        {
            NotifyModelStateErrors();

            return Response();
        }

        protected IActionResult ResponseWithError(string error)
        {
            NotifyError(error);

            return Response();
        }

        protected void NotifyModelStateErrors()
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors);
            foreach (var error in errors)
            {
                var erroMsg = error.Exception == null ? error.ErrorMessage : error.Exception.Message;
                NotifyError(string.Empty, erroMsg);
            }
        }

        protected IEnumerable<string> GetNotificationErrors()
        {
            return _notifications.GetNotifications().Select(t => t.Value);
        }

        protected void NotifyError(string code, string message)
        {
            _mediator.RaiseEvent(new DomainNotification(code, message));
        }

        protected void NotifyError(string message) => NotifyError(string.Empty, message);

        protected bool IsNullRequest(object request)
        {
            if (request != null) return false;

            NotifyError("Objeto passado é inválido. Verifique os parâmetros passados e tente novamente.");

            return true;
        } 
    }
}