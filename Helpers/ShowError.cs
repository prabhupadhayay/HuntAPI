using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.ModelBinding;

namespace DiscoveryHuntApi.Helpers
{
    public static class ShowError
    {
        public static IEnumerable Errors(this ModelStateDictionary modelState)
        {
            if (!modelState.IsValid)
            {
                return modelState.ToDictionary(kvp => kvp.Key,
                    kvp => kvp.Value.Errors
                                    .Select(e => e.ErrorMessage).ToArray())
                                    .Where(m => m.Value.Any());
            }
            return null;
        }
    }
}



