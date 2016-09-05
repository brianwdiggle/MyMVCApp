using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyMVCAppCS.Models
{
    using System.Collections.Specialized;
    using System.Web.Mvc;

    public static class CommonSelectLists
    {
        public static SelectList GtLt()
        {
            return new SelectList(new List<Object>
                            {
                                new { value="Gt", text=">"},
                                new { value="Lt", text="<"}
                            },
                           "value",
                           "text"
                                   );
        }

        public static SelectList BooleanOperators()
        {
            return new SelectList(new List<Object>
                            {
                                new { value="AND", text="AND"},
                                new { value="OR", text="OR"}
                            },
                           "value",
                           "text"
                                   );
        }

        public static SelectList Months()
        {
            return new SelectList( new List<Object>
                {
                  new { value="January", text="January"},
                  new { value="February", text="February"},
                  new { value="March", text="March"},
                  new { value="April", text="April"},
                  new { value="May", text="May"},
                  new { value="June", text="June"},
                  new { value="July", text="July"},
                  new { value="August", text="August"},
                  new { value="September", text="September"},
                  new { value="October", text="October"},
                  new { value="November", text="November"},
                  new { value="December", text="December"},
                },
                  "value",
                  "text"
                );
        }
    }
}