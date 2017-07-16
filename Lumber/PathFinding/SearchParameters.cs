using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Lumber.PathFinding
{
    public class SearchParameters
    {
        public Point StartLocation { get; private set; }

        public Point EndLocation { get; private set; }

        public List<Field> Fields { get; private set; }


        public SearchParameters(Point startLocation, Point endLocation, List<Field> Fields)
        {
            this.StartLocation = startLocation;
            this.EndLocation = endLocation;
            this.Fields = Fields;
        }
    }
}
