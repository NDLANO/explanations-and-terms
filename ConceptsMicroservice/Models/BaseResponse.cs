/**
 * Copyright (c) 2018-present, NDLA.
 *
 * This source code is licensed under the GPLv3 license found in the
 * LICENSE file in the root directory of this source tree.
 *
 */
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;

namespace ConceptsMicroservice.Models
{
    public class BaseResponse
    {
        [JsonIgnore]
        public ModelStateDictionary Errors { get; set; } = new ModelStateDictionary();

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "Errors")]
        public Dictionary<string, string> RenderedErrors
        {
            get
            {
                if (Errors == null)
                    return null;

                var errors = new Dictionary<string, string>();
                foreach (var error in Errors)
                {
                    try
                    {
                        var messageObject = error.Value.Errors.FirstOrDefault();
                        var message = "Field is not valid";
                        if (messageObject != null)
                            message = messageObject.ErrorMessage;

                        errors.Add(error.Key, message);
                    }
                    catch
                    {
                        //ignored
                    }
                }

                return errors.Any() ? errors : null;
            }
        }

        public bool HasErrors()
        {
            if (Errors == null)
                return false;
            return Errors.Count > 0;
        }
    }
}
