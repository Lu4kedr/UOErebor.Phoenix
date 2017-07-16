using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lumber.PathFinding
{
    public enum NodeState
    {
        /// <summary>
        /// The node has not yet been considered in any possible paths
        /// </summary>
        Untested,
        /// <summary>
        /// The node has been identified as a possible step in a path
        /// </summary>
        Open,
        /// <summary>
        /// The node has already been included in a path and will not be considered again
        /// </summary>
        Closed
    }
}
