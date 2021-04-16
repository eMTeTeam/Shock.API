using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shock.API.DataModel
{
    public class Observation
    {
        public string ReferenceNo { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
        public DateTime DueDate { get; set; }
        public string Category { get; set; }
        public string TypeOfActivity { get; set; }
        public string AgreedAction { get; set; }
        public string Evidence { get; set; }
        public string ProjectName { get; set; }
        public string ProjectLocation { get; set; }
    }
}
