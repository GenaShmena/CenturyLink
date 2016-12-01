using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Web.Http.ModelBinding;

namespace ServerTrack.Filters
{
    /// <summary>
    /// ValidateModelNullAttribute class
    /// </summary>
    /// <seealso cref="System.Web.Http.Filters.ActionFilterAttribute" />
    public class ValidateModelNullAttribute : ActionFilterAttribute
    {
        private readonly Func<Dictionary<string, object>, bool> _validate;

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidateModelNullAttribute"/> class.
        /// </summary>
        public ValidateModelNullAttribute() : this(arguments => arguments.ContainsValue(null))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidateModelNullAttribute"/> class.
        /// </summary>
        /// <param name="checkCondition">The check condition.</param>
        public ValidateModelNullAttribute(Func<Dictionary<string, object>, bool> checkCondition)
        {
            _validate = checkCondition;
        }

        /// <summary>
        /// Occurs before the action method is invoked.
        /// </summary>
        /// <param name="actionContext">The action context.</param>
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            if (_validate(actionContext.ActionArguments))
            {
                actionContext.Response = actionContext.Request.CreateErrorResponse(HttpStatusCode.BadRequest, "The argument cannot be null");
            }
        }
    }
}