/**
 * Copyright (c) 2018-present, NDLA.
 *
 * This source code is licensed under the GPLv3 license found in the
 * LICENSE file in the root directory of this source tree.
 *
 */
using ConceptsMicroservice.Models.ExternalApi.Image;

namespace ConceptsMicroservice.Services.ExternalApi
{
    public interface IImageService
    {
        ImageResponse GetImages(ImageQuery query);
        ImageResponse GetImage(SingleImageQuery query);

    }
}
