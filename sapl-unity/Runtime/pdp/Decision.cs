using System;
using System.Collections.Generic;
using System.Text;

namespace Sapl.Pdp.Api
{
    /**
     * The different possible outcomes of a SAPL decision.
     *
     * PERMIT grants access to the resource, while respecting potential obligations,
     * advises, or resource transformation.
     *
     * DENY denies access to the resource.
     *
     * INDETERMINATE means that an error occurred during the decision process.
     * Access must be denied in this case.
     *
     * NOT_APPLICABLE means no policies were found matching the authorization
     * subscription. Access must be denied in this case.
     */
    public enum Decision
    {
        /**
         * PERMIT grants access to the resource, while respecting potential obligations,
         * advises, or resource transformation.
         */
        PERMIT,

        /**
         * DENY denies access to the resource.
         */
        DENY,

        /**
         * INDETERMINATE means that an error occurred during the decision process.
         * Access must be denied in this case.
         */
        INDETERMINATE,

        /**
         * NOT_APPLICABLE means no policies were found matching the authorization
         * subscription. Access must be denied in this case.
         */
        NOT_APPLICABLE
    }
}
